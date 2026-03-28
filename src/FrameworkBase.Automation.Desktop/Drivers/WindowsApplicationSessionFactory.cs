using FrameworkBase.Automation.Core.Configuration;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace FrameworkBase.Automation.Desktop.Drivers;

public sealed class WindowsApplicationSessionFactory
{
    private const int DefaultWaitForAppLaunchSeconds = 5;
    private static readonly TimeSpan SessionReadinessPollingInterval = TimeSpan.FromMilliseconds(500);

    private readonly DesktopSettings settings;

    public WindowsApplicationSessionFactory(DesktopSettings settings)
    {
        this.settings = settings;
    }

    public WindowsDriver Create()
    {
        if (CanUseTopLevelWindowFallback())
        {
            try
            {
                var appTopLevelWindow = LaunchUwpApplicationAndGetTopLevelWindow();
                return CreateValidatedSession(BuildOptions(appTopLevelWindow));
            }
            catch (Exception bootstrapException)
            {
                try
                {
                    return CreateValidatedSession(BuildOptions());
                }
                catch (Exception fallbackException)
                {
                    throw new InvalidOperationException(
                        $"Failed to initialize a desktop session for '{settings.ApplicationId}' using automationName '{settings.AutomationName}'.",
                        new AggregateException(bootstrapException, fallbackException));
                }
            }
        }

        return CreateValidatedSession(BuildOptions());
    }

    private WindowsDriver CreateValidatedSession(AppiumOptions options)
    {
        WindowsDriver? session = null;

        try
        {
            session = new WindowsDriver(new Uri(settings.DriverUri), options);
            EnsureSessionIsReady(session);
            return session;
        }
        catch
        {
            DisposeSession(session);
            throw;
        }
    }

    private AppiumOptions BuildOptions(string? appTopLevelWindow = null)
    {
        var options = new AppiumOptions();
        var automationName = settings.AutomationName;

        options.PlatformName = "Windows";
        options.AutomationName = automationName;
        options.DeviceName = settings.DeviceName;

        if (string.IsNullOrWhiteSpace(appTopLevelWindow))
        {
            options.App = settings.ApplicationId;
        }
        else
        {
            options.AddAdditionalAppiumOption("appTopLevelWindow", appTopLevelWindow);
        }

        if (UsesWinAppDriverBackend(automationName) &&
            settings.CreateSessionTimeoutMilliseconds > 0)
        {
            options.AddAdditionalAppiumOption("createSessionTimeout", settings.CreateSessionTimeoutMilliseconds);
        }

        var waitForAppLaunchSeconds = ResolveWaitForAppLaunchSeconds(automationName);

        if (SupportsWaitForAppLaunch(automationName) &&
            waitForAppLaunchSeconds > 0)
        {
            options.AddAdditionalAppiumOption("ms:waitForAppLaunch", waitForAppLaunchSeconds);
        }

        if (UsesWinAppDriverBackend(automationName) &&
            settings.SystemPort is > 0)
        {
            options.AddAdditionalAppiumOption("systemPort", settings.SystemPort.Value);
        }

        if (UsesWinAppDriverBackend(automationName) &&
            !string.IsNullOrWhiteSpace(settings.WinAppDriverUrl))
        {
            options.AddAdditionalAppiumOption("wadUrl", settings.WinAppDriverUrl);
        }

        return options;
    }

    private void EnsureSessionIsReady(WindowsDriver session)
    {
        var timeout = TimeSpan.FromSeconds(Math.Max(ResolveWaitForAppLaunchSeconds(settings.AutomationName), 5));
        var startedAt = DateTime.UtcNow;
        Exception? lastDriverError = null;

        while (DateTime.UtcNow - startedAt < timeout)
        {
            try
            {
                if (HasAttachedApplicationWindow(session))
                {
                    return;
                }
            }
            catch (WebDriverException exception)
            {
                lastDriverError = exception;
            }

            Thread.Sleep(SessionReadinessPollingInterval);
        }

        throw new InvalidOperationException(
            $"The desktop session was created, but '{settings.ApplicationId}' never exposed a usable application root window.",
            lastDriverError);
    }

    private static bool HasAttachedApplicationWindow(WindowsDriver session)
    {
        var pageSource = session.PageSource;

        return !string.IsNullOrWhiteSpace(pageSource) &&
               !pageSource.Contains("<DummyRoot", StringComparison.OrdinalIgnoreCase);
    }

    private int ResolveWaitForAppLaunchSeconds(string automationName)
    {
        if (settings.WaitForAppLaunchSeconds is > 0)
        {
            return settings.WaitForAppLaunchSeconds.Value;
        }

        return SupportsWaitForAppLaunch(automationName)
            ? DefaultWaitForAppLaunchSeconds
            : 0;
    }

    private static bool SupportsWaitForAppLaunch(string automationName)
    {
        return automationName.Equals("Windows", StringComparison.OrdinalIgnoreCase) ||
               automationName.Equals("NovaWindows", StringComparison.OrdinalIgnoreCase);
    }

    private static bool UsesWinAppDriverBackend(string automationName)
    {
        return automationName.Equals("Windows", StringComparison.OrdinalIgnoreCase);
    }

    private bool CanUseTopLevelWindowFallback()
    {
        return settings.AutomationName.Equals("NovaWindows", StringComparison.OrdinalIgnoreCase) &&
               IsUwpApplicationId(settings.ApplicationId);
    }

    private string LaunchUwpApplicationAndGetTopLevelWindow()
    {
        var previousWindow = GetApplicationFrameHostMainWindow();

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"shell:AppsFolder\\{settings.ApplicationId}",
            UseShellExecute = true,
        });

        var timeout = TimeSpan.FromSeconds(Math.Max(ResolveWaitForAppLaunchSeconds(settings.AutomationName), 5));
        var startedAt = DateTime.UtcNow;

        while (DateTime.UtcNow - startedAt < timeout)
        {
            var currentWindow = GetApplicationFrameHostMainWindow();

            if (currentWindow.Handle != IntPtr.Zero &&
                (!currentWindow.Handle.Equals(previousWindow.Handle) ||
                 !string.Equals(currentWindow.Title, previousWindow.Title, StringComparison.Ordinal)))
            {
                return currentWindow.Handle.ToInt64().ToString(CultureInfo.InvariantCulture);
            }

            Thread.Sleep(SessionReadinessPollingInterval);
        }

        throw new InvalidOperationException(
            $"Failed to detect the top-level window for the UWP app '{settings.ApplicationId}'.");
    }

    private static ApplicationFrameHostWindow GetApplicationFrameHostMainWindow()
    {
        foreach (var process in Process.GetProcessesByName("ApplicationFrameHost"))
        {
            process.Refresh();

            if (process.MainWindowHandle != IntPtr.Zero &&
                !string.IsNullOrWhiteSpace(process.MainWindowTitle))
            {
                return new ApplicationFrameHostWindow(process.MainWindowHandle, process.MainWindowTitle);
            }
        }

        return ApplicationFrameHostWindow.Empty;
    }

    private static bool IsUwpApplicationId(string applicationId)
    {
        return applicationId.Contains('!') &&
               applicationId.Contains('_') &&
               !applicationId.Contains(Path.DirectorySeparatorChar) &&
               !applicationId.Contains(Path.AltDirectorySeparatorChar);
    }

    private static void DisposeSession(WindowsDriver? session)
    {
        if (session is null)
        {
            return;
        }

        try
        {
            session.Quit();
        }
        catch
        {
        }

        session.Dispose();
    }

    private readonly record struct ApplicationFrameHostWindow(IntPtr Handle, string Title)
    {
        public static ApplicationFrameHostWindow Empty { get; } = new(IntPtr.Zero, string.Empty);
    }
}
