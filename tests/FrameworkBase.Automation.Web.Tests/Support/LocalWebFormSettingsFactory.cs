using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Web.Tests.Support;

public static class LocalWebFormSettingsFactory
{
    public static WebSettings Create(WebSettings baseSettings)
    {
        var htmlPath = Path.Combine(AppContext.BaseDirectory, "TestAssets", "web-form.html");
        var browserOverride = Environment.GetEnvironmentVariable("BROWSER");
        var headlessOverride = Environment.GetEnvironmentVariable("HEADLESS");
        var isHeadless = baseSettings.Headless;

        if (bool.TryParse(headlessOverride, out var parsedHeadless))
        {
            isHeadless = parsedHeadless;
        }

        return new WebSettings
        {
            BaseUrl = new Uri(htmlPath).AbsoluteUri,
            Browser = string.IsNullOrWhiteSpace(browserOverride) ? baseSettings.Browser : browserOverride,
            Headless = isHeadless,
        };
    }
}
