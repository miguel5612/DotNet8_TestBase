using FrameworkBase.Automation.Desktop.Drivers;
using FrameworkBase.Automation.Desktop.Screens;
using FrameworkBase.Automation.Tests.Common;
using FluentAssertions;
using OpenQA.Selenium.Appium.Windows;
using TechTalk.SpecFlow;

namespace FrameworkBase.Automation.Desktop.Tests.Steps;

[Binding]
public sealed class CalculatorSteps : IDisposable
{
    private WindowsDriver? session;
    private string? result;

    [Given("the Windows Calculator desktop app is available")]
    public void GivenTheWindowsCalculatorDesktopAppIsAvailable()
    {
    }

    [When("I add one and two in Calculator")]
    public void WhenIAddOneAndTwoInCalculator()
    {
        var settings = AutomationTestSession.LoadSettings();
        session = new WindowsApplicationSessionFactory(settings.Desktop).Create();
        var calculator = new CalculatorScreen(session);
        result = calculator.Add(1, 2);
    }

    [Then("Calculator should display three")]
    public void ThenCalculatorShouldDisplayThree()
    {
        result.Should().Be("3");
    }

    public void Dispose()
    {
        session?.Quit();
        session?.Dispose();
    }
}
