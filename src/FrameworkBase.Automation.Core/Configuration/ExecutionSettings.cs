namespace FrameworkBase.Automation.Core.Configuration;

public sealed class ExecutionSettings
{
    public string ArtifactsDirectory { get; init; } = "artifacts";

    public int DefaultTimeoutSeconds { get; init; } = 15;

    public int PollingIntervalMilliseconds { get; init; } = 250;
}
