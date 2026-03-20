using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Core.Configuration;
using RestSharp;
using System.Text.Json;

namespace FrameworkBase.Automation.Api.Clients;

public sealed class JsonPlaceholderRestSharpClient
{
    private readonly RestClient restClient;
    private readonly ApiSettings settings;

    public JsonPlaceholderRestSharpClient(ApiSettings settings)
    {
        this.settings = settings;
        var httpClient = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
        })
        {
            BaseAddress = new Uri(settings.BaseUrl),
        };

        restClient = new RestClient(
            httpClient,
            new RestClientOptions
            {
                BaseUrl = new Uri(settings.BaseUrl),
                Proxy = null,
            },
            disposeHttpClient: true);
    }

    public async Task<PostDto?> GetPostAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = BuildRequest($"posts/{id}", Method.Get);
        var response = await restClient.ExecuteAsync<PostDto>(request, cancellationToken);
        EnsureSuccess(response);
        return ExtractResponseData(response);
    }

    public async Task<PostDto?> CreatePostAsync(CreatePostRequest requestBody, CancellationToken cancellationToken = default)
    {
        var request = BuildRequest("posts", Method.Post);
        request.AddJsonBody(requestBody);
        var response = await restClient.ExecuteAsync<PostDto>(request, cancellationToken);
        EnsureSuccess(response);
        return ExtractResponseData(response);
    }

    private RestRequest BuildRequest(string resource, Method method)
    {
        var request = new RestRequest(resource, method);

        foreach (var header in settings.DefaultHeaders)
        {
            request.AddHeader(header.Key, header.Value);
        }

        if (!string.IsNullOrWhiteSpace(settings.BearerToken))
        {
            request.AddOrUpdateHeader("Authorization", $"Bearer {settings.BearerToken}");
        }

        return request;
    }

    private static void EnsureSuccess(RestResponse response)
    {
        if (!response.IsSuccessful)
        {
            throw new HttpRequestException(
                $"The API request failed with status code {(int?)response.StatusCode} and content '{response.Content}'.");
        }
    }

    private static PostDto? ExtractResponseData(RestResponse<PostDto> response)
    {
        if (response.Data is not null && !string.IsNullOrWhiteSpace(response.Data.Title))
        {
            return response.Data;
        }

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            return response.Data;
        }

        return JsonSerializer.Deserialize<PostDto>(response.Content);
    }
}
