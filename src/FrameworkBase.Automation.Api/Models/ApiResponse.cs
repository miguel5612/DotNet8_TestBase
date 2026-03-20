using System.Net;

namespace FrameworkBase.Automation.Api.Models;

/// <summary>
/// Represents the HTTP result returned by an API client.
/// Input: a typed response body, the HTTP status code, and the raw payload.
/// Output: a value object that allows tests to validate transport and business details together.
/// Business case: API automation should assert not only success codes but also the business response that the consumer receives.
/// </summary>
/// <typeparam name="TBody">The typed body deserialized from the HTTP payload.</typeparam>
public sealed class ApiResponse<TBody>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse{TBody}"/> class.
    /// Input: an HTTP status code, a typed body, and the raw payload.
    /// Output: an immutable response object for API assertions.
    /// Business case: preserve transport evidence and deserialized data in the same test artifact.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the server.</param>
    /// <param name="body">The typed body deserialized from the payload.</param>
    /// <param name="rawBody">The raw payload returned by the server.</param>
    public ApiResponse(HttpStatusCode statusCode, TBody? body, string rawBody)
    {
        StatusCode = statusCode;
        Body = body;
        RawBody = rawBody;
    }

    /// <summary>
    /// Gets the HTTP status code returned by the server.
    /// Output: the transport-level result for the request.
    /// Business case: acceptance tests can verify whether the endpoint behaved as expected from an HTTP perspective.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the body deserialized into the expected domain type.
    /// Output: a typed representation of the business payload.
    /// Business case: tests can validate business rules without manual JSON parsing.
    /// </summary>
    public TBody? Body { get; }

    /// <summary>
    /// Gets the raw response payload returned by the server.
    /// Output: the original payload string.
    /// Business case: the raw body can be logged as evidence when a test fails or when traceability is required.
    /// </summary>
    public string RawBody { get; }

    /// <summary>
    /// Gets a value indicating whether the response was successful according to the HTTP status code.
    /// Output: <c>true</c> for 2xx codes; otherwise <c>false</c>.
    /// Business case: smoke checks can quickly identify technical failures before deeper business assertions run.
    /// </summary>
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;
}
