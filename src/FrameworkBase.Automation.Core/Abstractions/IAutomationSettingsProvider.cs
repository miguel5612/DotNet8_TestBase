using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Core.Abstractions;

public interface IAutomationSettingsProvider
{
    AutomationSettings Load();
}
