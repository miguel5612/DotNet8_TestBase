namespace FrameworkBase.Automation.Core.Configuration;

public sealed class AutomationSettings
{
    public ExecutionSettings Execution { get; init; } = new();

    public WebSettings Web { get; init; } = new();

    public ApiSettings Api { get; init; } = new();

    public DesktopSettings Desktop { get; init; } = new();
}
