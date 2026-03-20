using System.Text.Json.Serialization;

namespace FrameworkBase.Automation.Api.Models;

public sealed class PostDto
{
    [JsonPropertyName("userId")]
    public int UserId { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;
}
