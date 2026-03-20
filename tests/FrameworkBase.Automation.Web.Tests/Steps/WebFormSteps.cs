using FrameworkBase.Automation.Tests.Common;
using FrameworkBase.Automation.Web.Drivers;
using FrameworkBase.Automation.Web.Flows;
using FrameworkBase.Automation.Web.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace FrameworkBase.Automation.Web.Tests.Steps;

[Binding]
public sealed class WebFormSteps
{
    private string? confirmationMessage;
    private LocalWebFormServer? server;

    [Given("the Selenium sample form is available")]
    public void GivenTheSeleniumSampleFormIsAvailable()
    {
    }

    [When("I submit valid candidate data in the form")]
    public void WhenISubmitValidCandidateDataInTheForm()
    {
        var settings = AutomationTestSession.LoadSettings();
        server = LocalWebFormServer.Start(settings.Web);

        using var driver = new WebDriverFactory(server.WebSettings).Create();
        var flow = new WebFormFlow(new SeleniumWebFormPage(driver, server.WebSettings));

        confirmationMessage = flow.SubmitCandidateForm(
            "BDD Candidate",
            "Password!45",
            "This scenario demonstrates SpecFlow on top of the page object flow.");
    }

    [Then("the page should confirm the submission")]
    public void ThenThePageShouldConfirmTheSubmission()
    {
        confirmationMessage.Should().Be("Received!");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        server?.Dispose();
    }
}
