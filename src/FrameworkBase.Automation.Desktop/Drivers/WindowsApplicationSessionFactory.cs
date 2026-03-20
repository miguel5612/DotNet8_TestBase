using FrameworkBase.Automation.Core.Configuration;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace FrameworkBase.Automation.Desktop.Drivers;

public sealed class WindowsApplicationSessionFactory
{
    private readonly DesktopSettings settings;

    public WindowsApplicationSessionFactory(DesktopSettings settings)
    {
        this.settings = settings;
    }

    public WindowsDriver Create()
    {
        var options = new AppiumOptions();
        options.PlatformName = "Windows";
        options.AutomationName = "Windows";
        options.App = settings.ApplicationId;
        options.DeviceName = settings.DeviceName;

        return new WindowsDriver(new Uri(settings.DriverUri), options);
    }
}
