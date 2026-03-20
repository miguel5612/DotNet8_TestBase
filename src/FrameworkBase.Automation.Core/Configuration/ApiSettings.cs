namespace FrameworkBase.Automation.Core.Configuration;

public sealed class ApiSettings
{
    public string BaseUrl { get; init; } = "https://jsonplaceholder.typicode.com";

    public string? BearerToken { get; init; }

    public IDictionary<string, string> DefaultHeaders { get; init; } = new Dictionary<string, string>();
}
