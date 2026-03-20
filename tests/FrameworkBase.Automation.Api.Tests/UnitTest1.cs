using FrameworkBase.Automation.Api.Clients;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Tests.Common;
using FluentAssertions;

namespace FrameworkBase.Automation.Api.Tests;

public sealed class JsonPlaceholderApiTests
{
    [Test]
    [Category("Api")]
    public async Task Should_get_a_post_using_httpclient()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);
        using var server = LocalJsonPlaceholderStubServer.Start(settings.Api);
        var client = new JsonPlaceholderHttpClient(server.ApiSettings);

        var response = await client.GetPostAsync(1);

        artifactLogger.WriteText(
            nameof(JsonPlaceholderApiTests),
            "httpclient-get-post.txt",
            $"Id={response?.Id}, Title={response?.Title}");

        response.Should().NotBeNull();
        response!.Id.Should().Be(1);
        response.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    [Category("Api")]
    public async Task Should_create_a_post_using_restsharp()
    {
        var settings = AutomationTestSession.LoadSettings();
        using var server = LocalJsonPlaceholderStubServer.Start(settings.Api);
        var client = new JsonPlaceholderRestSharpClient(server.ApiSettings);

        var response = await client.CreatePostAsync(new CreatePostRequest
        {
            UserId = 99,
            Title = "Automation architecture",
            Body = "RestSharp example created from the PDF topics.",
        });

        response.Should().NotBeNull();
        response!.Title.Should().Be("Automation architecture");
        response.Body.Should().Contain("PDF topics");
    }
}
