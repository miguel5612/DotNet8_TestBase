using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Api.Clients;

public sealed class JsonPlaceholderHttpClient
{
    private readonly HttpClient httpClient;

    public JsonPlaceholderHttpClient(ApiSettings settings, HttpClient? httpClient = null)
    {
        this.httpClient = httpClient ?? CreateDefaultHttpClient();
        this.httpClient.BaseAddress = new Uri(settings.BaseUrl);
        ApplyHeaders(settings);
    }

    public async Task<PostDto?> GetPostAsync(int id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<PostDto>($"posts/{id}", cancellationToken);
    }

    public async Task<PostDto?> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("posts", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PostDto>(cancellationToken: cancellationToken);
    }

    private void ApplyHeaders(ApiSettings settings)
    {
        foreach (var header in settings.DefaultHeaders)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (!string.IsNullOrWhiteSpace(settings.BearerToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", settings.BearerToken);
        }
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        return new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
        });
    }
}
