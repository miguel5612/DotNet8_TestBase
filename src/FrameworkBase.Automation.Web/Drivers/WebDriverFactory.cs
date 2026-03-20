using FrameworkBase.Automation.Core.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System.IO;

namespace FrameworkBase.Automation.Web.Drivers;

public sealed class WebDriverFactory
{
    private readonly WebSettings settings;

    public WebDriverFactory(WebSettings settings)
    {
        this.settings = settings;
    }

    public IWebDriver Create()
    {
        return settings.Browser.Trim().ToLowerInvariant() switch
        {
            "chrome" => CreateChromeDriver(),
            "firefox" => CreateFirefoxDriver(),
            _ => CreateEdgeDriver(),
        };
    }

    private IWebDriver CreateChromeDriver()
    {
        var options = new ChromeOptions();
        ApplyCommonArguments(options);
        var driverPath = Environment.GetEnvironmentVariable("CHROMEDRIVER_PATH");

        if (!string.IsNullOrWhiteSpace(driverPath) && File.Exists(driverPath))
        {
            return new ChromeDriver(
                Path.GetDirectoryName(driverPath)!,
                options);
        }

        return new ChromeDriver(options);
    }

    private IWebDriver CreateEdgeDriver()
    {
        var options = new EdgeOptions();
        ApplyCommonArguments(options);
        var driverPath = Environment.GetEnvironmentVariable("EDGEDRIVER_PATH");

        if (!string.IsNullOrWhiteSpace(driverPath) && File.Exists(driverPath))
        {
            return new EdgeDriver(
                Path.GetDirectoryName(driverPath)!,
                options);
        }

        return new EdgeDriver(options);
    }

    private IWebDriver CreateFirefoxDriver()
    {
        var options = new FirefoxOptions();
        ApplyFirefoxArguments(options);
        var driverPath = Environment.GetEnvironmentVariable("GECKODRIVER_PATH");

        if (!string.IsNullOrWhiteSpace(driverPath) && File.Exists(driverPath))
        {
            return new FirefoxDriver(
                Path.GetDirectoryName(driverPath)!,
                options);
        }

        return new FirefoxDriver(options);
    }

    private void ApplyCommonArguments(ChromiumOptions options)
    {
        if (settings.Headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--window-size=1600,1000");
    }

    private void ApplyFirefoxArguments(FirefoxOptions options)
    {
        if (settings.Headless)
        {
            options.AddArgument("-headless");
        }

        options.AddArgument("--width=1600");
        options.AddArgument("--height=1000");
    }
}
