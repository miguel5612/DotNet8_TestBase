using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Api.Tests;

public sealed class LocalJsonPlaceholderStubServer : IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly TcpListener listener;
    private readonly Task serverTask;

    private LocalJsonPlaceholderStubServer(ApiSettings baseSettings, string baseUrl)
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

    public ApiSettings ApiSettings { get; }

    public static LocalJsonPlaceholderStubServer Start(ApiSettings baseSettings)
    {
        var port = GetAvailablePort();
        var baseUrl = $"http://127.0.0.1:{port}/";
        return new LocalJsonPlaceholderStubServer(baseSettings, baseUrl);
    }

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

    private static async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
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

        if (method == "GET" && path == "posts/1")
        {
            await WriteJsonResponseAsync(
                writer,
                200,
                new PostDto
                {
                    Id = 1,
                    UserId = 7,
                    Title = "Stubbed post",
                    Body = "Local JSONPlaceholder stub response.",
                });
            return;
        }

        if (method == "POST" && path == "posts")
        {
            if (contentLength > 0)
            {
                var buffer = new char[contentLength];
                _ = await reader.ReadBlockAsync(buffer, 0, contentLength);
            }

            await WriteJsonResponseAsync(
                writer,
                201,
                new PostDto
                {
                    Id = 101,
                    UserId = 99,
                    Title = "Automation architecture",
                    Body = "RestSharp example created from the PDF topics.",
                });
            return;
        }

        await WriteJsonResponseAsync(
            writer,
            404,
            new
            {
                message = "Route not configured in the local stub server.",
            });
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
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string GetReasonPhrase(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            201 => "Created",
            404 => "Not Found",
            _ => "OK",
        };
    }
}
