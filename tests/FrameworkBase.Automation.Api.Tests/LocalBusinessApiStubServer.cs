using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Api.Rules;
using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Api.Tests;

/// <summary>
/// Hosts a lightweight local HTTP server that simulates banking and retail APIs.
/// Input: base API settings used to clone headers and generate a local base URL.
/// Output: a disposable server that returns deterministic business responses for automated tests.
/// Business case: the framework can validate API contracts and business outcomes without depending on external services.
/// </summary>
public sealed class LocalBusinessApiStubServer : IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly BankTransferDecisionEngine bankTransferDecisionEngine = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly TcpListener listener;
    private readonly RetailPricingEngine retailPricingEngine = new();
    private readonly Task serverTask;

    private LocalBusinessApiStubServer(ApiSettings baseSettings, string baseUrl)
    {
        ApiSettings = new ApiSettings
        {
            BaseUrl = baseUrl,
            BearerToken = baseSettings.BearerToken,
            DefaultHeaders = new Dictionary<string, string>(baseSettings.DefaultHeaders),
        };

        var uri = new Uri(baseUrl);
        listener = new TcpListener(IPAddress.Loopback, uri.Port);
        listener.Start();
        serverTask = Task.Run(() => ProcessRequestsAsync(cancellationTokenSource.Token));
    }

    /// <summary>
    /// Gets the API settings configured to point clients to the local server.
    /// Output: a settings object with the local base URL and inherited headers.
    /// Business case: tests can reuse existing API clients without hardcoding localhost details.
    /// </summary>
    public ApiSettings ApiSettings { get; }

    /// <summary>
    /// Starts the local business API stub server.
    /// Input: the base API settings defined for the framework.
    /// Output: a running local server bound to an available port.
    /// Business case: each test run receives an isolated mock endpoint that behaves like a business API.
    /// </summary>
    /// <param name="baseSettings">The base API settings used as the template for the local endpoint.</param>
    /// <returns>A running local business API stub server.</returns>
    public static LocalBusinessApiStubServer Start(ApiSettings baseSettings)
    {
        var port = GetAvailablePort();
        var baseUrl = $"http://127.0.0.1:{port}/";
        return new LocalBusinessApiStubServer(baseSettings, baseUrl);
    }

    /// <summary>
    /// Stops the server and releases network resources.
    /// Output: the listener and background processing task are terminated.
    /// Business case: local mock servers should be isolated per test to avoid state leakage across runs.
    /// </summary>
    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        listener.Stop();

        try
        {
            serverTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }

        cancellationTokenSource.Dispose();
    }

    private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client;

            try
            {
                client = await listener.AcceptTcpClientAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            _ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using var tcpClient = client;
        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            NewLine = "\r\n",
        };

        var requestLine = await reader.ReadLineAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(requestLine))
        {
            return;
        }

        var segments = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var method = segments[0];
        var path = segments[1].Trim('/');
        var contentLength = 0;
        string? line;

        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync(cancellationToken)))
        {
            if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            {
                _ = int.TryParse(line["Content-Length:".Length..].Trim(), out contentLength);
            }
        }

        var body = contentLength > 0
            ? await ReadBodyAsync(reader, contentLength, cancellationToken)
            : string.Empty;

        try
        {
            switch ((method, path))
            {
                case ("GET", "health"):
                    await WriteJsonResponseAsync(writer, 200, new
                    {
                        status = "Up",
                        component = "LocalBusinessApiStubServer",
                    });
                    return;
                case ("POST", "business/banking/transfers/simulate"):
                    await HandleBankingSimulationAsync(writer, body);
                    return;
                case ("POST", "business/retail/pricing/quote"):
                    await HandleRetailQuoteAsync(writer, body);
                    return;
                default:
                    await WriteJsonResponseAsync(writer, 404, new
                    {
                        message = "Route not configured in the local business stub server.",
                    });
                    return;
            }
        }
        catch (ArgumentException exception)
        {
            await WriteJsonResponseAsync(writer, 400, new
            {
                message = exception.Message,
            });
        }
        catch (JsonException exception)
        {
            await WriteJsonResponseAsync(writer, 400, new
            {
                message = $"Invalid JSON body. {exception.Message}",
            });
        }
    }

    private async Task HandleBankingSimulationAsync(StreamWriter writer, string body)
    {
        var request = JsonSerializer.Deserialize<BankTransferRequest>(body, SerializerOptions)
            ?? throw new JsonException("Bank transfer request could not be deserialized.");
        var decision = bankTransferDecisionEngine.Evaluate(request);
        await WriteJsonResponseAsync(writer, 200, decision);
    }

    private async Task HandleRetailQuoteAsync(StreamWriter writer, string body)
    {
        var request = JsonSerializer.Deserialize<RetailPriceQuoteRequest>(body, SerializerOptions)
            ?? throw new JsonException("Retail quote request could not be deserialized.");
        var quote = retailPricingEngine.CalculateQuote(request);
        await WriteJsonResponseAsync(writer, 200, quote);
    }

    private static async Task<string> ReadBodyAsync(
        StreamReader reader,
        int contentLength,
        CancellationToken cancellationToken)
    {
        var buffer = new char[contentLength];
        var totalRead = 0;

        while (totalRead < contentLength)
        {
            var read = await reader.ReadAsync(buffer.AsMemory(totalRead, contentLength - totalRead), cancellationToken);

            if (read == 0)
            {
                break;
            }

            totalRead += read;
        }

        return new string(buffer, 0, totalRead);
    }

    private static async Task WriteJsonResponseAsync(StreamWriter writer, int statusCode, object body)
    {
        var json = JsonSerializer.Serialize(body);
        var payload = Encoding.UTF8.GetBytes(json);
        await writer.WriteLineAsync($"HTTP/1.1 {statusCode} {GetReasonPhrase(statusCode)}");
        await writer.WriteLineAsync("Content-Type: application/json; charset=utf-8");
        await writer.WriteLineAsync($"Content-Length: {payload.Length}");
        await writer.WriteLineAsync("Connection: close");
        await writer.WriteLineAsync();
        await writer.FlushAsync();
        await writer.BaseStream.WriteAsync(payload);
        await writer.BaseStream.FlushAsync();
    }

    private static int GetAvailablePort()
    {
        using var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        return ((IPEndPoint)probe.LocalEndpoint).Port;
    }

    private static string GetReasonPhrase(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            400 => "Bad Request",
            404 => "Not Found",
            _ => "OK",
        };
    }
}
