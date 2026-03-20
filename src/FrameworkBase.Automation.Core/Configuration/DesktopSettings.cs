namespace FrameworkBase.Automation.Core.Configuration;

public sealed class DesktopSettings
{
    public string DriverUri { get; init; } = "http://127.0.0.1:4723";

    public string ApplicationId { get; init; } = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";

    public string DeviceName { get; init; } = "WindowsPC";
}
