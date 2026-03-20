using System.Net;
using System.Net.Sockets;
using System.Text;
using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Web.Tests;

public sealed class LocalWebFormServer : IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly TcpListener listener;
    private readonly Task serverTask;
    private readonly string htmlContent;

    private LocalWebFormServer(WebSettings baseSettings, string htmlContent, int port)
    {
        this.htmlContent = htmlContent;
        listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        serverTask = Task.Run(() => ProcessRequestsAsync(cancellationTokenSource.Token));

        var browserOverride = Environment.GetEnvironmentVariable("BROWSER");
        var headlessOverride = Environment.GetEnvironmentVariable("HEADLESS");
        var isHeadless = baseSettings.Headless;

        if (bool.TryParse(headlessOverride, out var parsedHeadless))
        {
            isHeadless = parsedHeadless;
        }

        WebSettings = new WebSettings
        {
            BaseUrl = $"http://127.0.0.1:{port}/",
            Browser = string.IsNullOrWhiteSpace(browserOverride) ? baseSettings.Browser : browserOverride,
            Headless = isHeadless,
        };
    }

    public WebSettings WebSettings { get; }

    public static LocalWebFormServer Start(WebSettings baseSettings)
    {
        var htmlPath = Path.Combine(AppContext.BaseDirectory, "TestAssets", "web-form.html");
        var htmlContent = File.ReadAllText(htmlPath);
        var port = GetAvailablePort();
        return new LocalWebFormServer(baseSettings, htmlContent, port);
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

            _ = Task.Run(() => HandleClientAsync(client), cancellationToken);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            NewLine = "\r\n",
        };

        var requestLine = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(requestLine))
        {
            return;
        }

        var segments = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var method = segments[0];
        var path = segments[1];

        while (!string.IsNullOrEmpty(await reader.ReadLineAsync()))
        {
        }

        if (method == "GET" && (path == "/" || path.StartsWith("/?")))
        {
            await WriteResponseAsync(writer, 200, "text/html; charset=utf-8", htmlContent);
            return;
        }

        if (method == "GET" && path == "/favicon.ico")
        {
            await WriteResponseAsync(writer, 204, "text/plain", string.Empty);
            return;
        }

        await WriteResponseAsync(writer, 404, "text/plain; charset=utf-8", "Not Found");
    }

    private static async Task WriteResponseAsync(StreamWriter writer, int statusCode, string contentType, string body)
    {
        var payload = Encoding.UTF8.GetBytes(body);
        await writer.WriteLineAsync($"HTTP/1.1 {statusCode} {GetReasonPhrase(statusCode)}");
        await writer.WriteLineAsync($"Content-Type: {contentType}");
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
            204 => "No Content",
            404 => "Not Found",
            _ => "OK",
        };
    }
}
