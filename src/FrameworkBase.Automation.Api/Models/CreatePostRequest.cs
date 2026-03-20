namespace FrameworkBase.Automation.Api.Models;

public sealed class CreatePostRequest
{
    public int UserId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;
}
