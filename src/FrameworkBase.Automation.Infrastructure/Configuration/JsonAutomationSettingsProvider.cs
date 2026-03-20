using FrameworkBase.Automation.Core.Abstractions;
using FrameworkBase.Automation.Core.Configuration;
using FrameworkBase.Automation.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FrameworkBase.Automation.Infrastructure.Configuration;

public sealed class JsonAutomationSettingsProvider : IAutomationSettingsProvider
{
    private readonly string settingsFilePath;

    public JsonAutomationSettingsProvider(string settingsFilePath)
    {
        this.settingsFilePath = settingsFilePath;
    }

    public AutomationSettings Load()
    {
        if (!File.Exists(settingsFilePath))
        {
            throw new AutomationConfigurationException(
                $"The settings file '{settingsFilePath}' was not found.");
        }

        using var stream = File.OpenRead(settingsFilePath);

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        var settings = configuration.Get<AutomationSettings>();

        if (settings is null)
        {
            throw new AutomationConfigurationException(
                $"The settings file '{settingsFilePath}' could not be mapped to {nameof(AutomationSettings)}.");
        }

        return settings;
    }
}
