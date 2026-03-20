using FrameworkBase.Automation.Tests.Common;
using FrameworkBase.Automation.Web.Drivers;
using FrameworkBase.Automation.Web.Flows;
using FrameworkBase.Automation.Web.Pages;
using FluentAssertions;

namespace FrameworkBase.Automation.Web.Tests;

public sealed class WebFormSmokeTests
{
    [Test]
    [Category("Web")]
    [Retry(2)]
    [Explicit("Requires a local browser driver or internet access for Selenium Manager.")]
    public void Should_submit_the_selenium_demo_form()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);
        using var server = LocalWebFormServer.Start(settings.Web);

        using var driver = new WebDriverFactory(server.WebSettings).Create();
        var flow = new WebFormFlow(new SeleniumWebFormPage(driver, server.WebSettings));

        var confirmationMessage = flow.SubmitCandidateForm(
            name: "Framework Base Candidate",
            password: "StrongPassword!23",
            comments: "This web automation example was created from the PDF interview guide.");

        artifactLogger.WriteText(nameof(WebFormSmokeTests), "web-summary.txt", confirmationMessage);
        confirmationMessage.Should().Be("Received!");
    }
}
