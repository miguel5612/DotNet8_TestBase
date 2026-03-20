namespace FrameworkBase.Automation.Core.Configuration;

public sealed class WebSettings
{
    public string BaseUrl { get; init; } = "https://www.selenium.dev/selenium/web/web-form.html";

    public string Browser { get; init; } = "Edge";

    public bool Headless { get; init; }
}
