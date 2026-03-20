using FrameworkBase.Automation.Core.Abstractions;
using FrameworkBase.Automation.Core.Configuration;
using FrameworkBase.Automation.Infrastructure.Configuration;
using FrameworkBase.Automation.Infrastructure.Logging;

namespace FrameworkBase.Automation.Tests.Common;

public static class AutomationTestSession
{
    private const string SettingsFileName = "automation.settings.json";

    public static AutomationSettings LoadSettings()
    {
        var settingsFilePath = Path.Combine(FindRepositoryRoot(), SettingsFileName);
        var settingsProvider = new JsonAutomationSettingsProvider(settingsFilePath);
        return settingsProvider.Load();
    }

    public static IArtifactLogger CreateArtifactLogger(AutomationSettings settings)
    {
        var repositoryRoot = FindRepositoryRoot();
        var artifactRoot = Path.Combine(repositoryRoot, settings.Execution.ArtifactsDirectory);
        return new ArtifactLogger(artifactRoot);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var settingsFilePath = Path.Combine(directory.FullName, SettingsFileName);

            if (File.Exists(settingsFilePath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find the repository root because '{SettingsFileName}' was not found.");
    }
}
