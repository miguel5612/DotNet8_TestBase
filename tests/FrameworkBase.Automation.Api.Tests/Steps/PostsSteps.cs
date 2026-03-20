using FrameworkBase.Automation.Api.Clients;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Tests.Common;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace FrameworkBase.Automation.Api.Tests.Steps;

[Binding]
public sealed class PostsSteps
{
    private PostDto? response;
    private LocalJsonPlaceholderStubServer? server;

    [Given("the JSONPlaceholder API is available")]
    public void GivenTheJsonPlaceholderApiIsAvailable()
    {
    }

    [When("I request a known post by id")]
    public async Task WhenIRequestAKnownPostById()
    {
        var settings = AutomationTestSession.LoadSettings();
        server = LocalJsonPlaceholderStubServer.Start(settings.Api);
        var client = new JsonPlaceholderHttpClient(server.ApiSettings);
        response = await client.GetPostAsync(1);
    }

    [Then("the API should return the expected post data")]
    public void ThenTheApiShouldReturnTheExpectedPostData()
    {
        response.Should().NotBeNull();
        response!.Id.Should().Be(1);
        response.UserId.Should().BePositive();
        response.Title.Should().NotBeNullOrWhiteSpace();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        server?.Dispose();
    }
}
