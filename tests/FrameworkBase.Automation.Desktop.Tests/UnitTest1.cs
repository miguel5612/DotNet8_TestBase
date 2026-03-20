using FrameworkBase.Automation.Desktop.Drivers;
using FrameworkBase.Automation.Desktop.Screens;
using FrameworkBase.Automation.Tests.Common;
using FluentAssertions;

namespace FrameworkBase.Automation.Desktop.Tests;

public sealed class CalculatorDesktopTests
{
    [Test]
    [Category("Desktop")]
    [Explicit("Requires WinAppDriver or Appium with Windows support running locally.")]
    public void Should_add_two_numbers_in_windows_calculator()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);

        using var session = new WindowsApplicationSessionFactory(settings.Desktop).Create();
        var calculator = new CalculatorScreen(session);

        var result = calculator.Add(1, 2);

        artifactLogger.WriteText(nameof(CalculatorDesktopTests), "desktop-summary.txt", $"Calculator result: {result}");
        result.Should().Be("3");
    }
}
