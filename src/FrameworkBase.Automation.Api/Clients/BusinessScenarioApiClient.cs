using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Core.Configuration;

namespace FrameworkBase.Automation.Api.Clients;

/// <summary>
/// Calls the local mock API used to simulate business-oriented banking and retail scenarios.
/// Input: API settings plus typed request bodies.
/// Output: typed responses that preserve HTTP status code and raw payload evidence.
/// Business case: let the framework validate transport behavior, payload contracts, and business rules in the same assertion flow.
/// </summary>
public sealed class BusinessScenarioApiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessScenarioApiClient"/> class.
    /// Input: API settings and an optional HTTP client.
    /// Output: a configured client ready to call the local mock service.
    /// Business case: tests can inject a shared client while still keeping environment configuration centralized.
    /// </summary>
    /// <param name="settings">The API configuration containing the base URL and default headers.</param>
    /// <param name="httpClient">An optional HTTP client supplied by the caller.</param>
    public BusinessScenarioApiClient(ApiSettings settings, HttpClient? httpClient = null)
    {
        this.httpClient = httpClient ?? CreateDefaultHttpClient();
        this.httpClient.BaseAddress = new Uri(settings.BaseUrl);
        ApplyHeaders(settings);
    }

    /// <summary>
    /// Sends a banking transfer simulation request to the mock API.
    /// Input: a <see cref="BankTransferRequest"/> containing the transfer context.
    /// Output: an <see cref="ApiResponse{TBody}"/> with the HTTP result and the simulated transfer decision.
    /// Business case: acceptance tests can verify banking approval and rejection paths without a real backend.
    /// </summary>
    /// <param name="request">The banking transfer request to simulate.</param>
    /// <param name="cancellationToken">The token used to cancel the HTTP operation.</param>
    /// <returns>The API response containing the banking transfer decision.</returns>
    public Task<ApiResponse<BankTransferDecision>> SimulateBankTransferAsync(
        BankTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<BankTransferRequest, BankTransferDecision>(
            "business/banking/transfers/simulate",
            request,
            cancellationToken);
    }

    /// <summary>
    /// Sends a retail pricing simulation request to the mock API.
    /// Input: a <see cref="RetailPriceQuoteRequest"/> describing the cart and customer context.
    /// Output: an <see cref="ApiResponse{TBody}"/> with the HTTP result and the simulated quote.
    /// Business case: retail acceptance tests can verify totals, promotions, and fulfillment outcomes in a realistic contract.
    /// </summary>
    /// <param name="request">The retail price quote request to simulate.</param>
    /// <param name="cancellationToken">The token used to cancel the HTTP operation.</param>
    /// <returns>The API response containing the retail quote.</returns>
    public Task<ApiResponse<RetailPriceQuote>> SimulateRetailQuoteAsync(
        RetailPriceQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<RetailPriceQuoteRequest, RetailPriceQuote>(
            "business/retail/pricing/quote",
            request,
            cancellationToken);
    }

    private async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string resource,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, resource)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, SerializerOptions),
                Encoding.UTF8,
                "application/json"),
        };

        using var response = await httpClient.SendAsync(message, cancellationToken);
        var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var typedBody = string.IsNullOrWhiteSpace(rawBody)
            ? default
            : JsonSerializer.Deserialize<TResponse>(rawBody, SerializerOptions);

        return new ApiResponse<TResponse>(response.StatusCode, typedBody, rawBody);
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
