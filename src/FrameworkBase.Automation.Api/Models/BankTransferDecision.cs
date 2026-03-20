using System.Text.Json.Serialization;

namespace FrameworkBase.Automation.Api.Models;

/// <summary>
/// Represents the simulated outcome of a banking transfer.
/// Input: produced by the transfer rule engine after evaluating a request.
/// Output: a body that explains transport success and business behavior.
/// Business case: API tests must verify why a transfer was approved or rejected, not only whether the endpoint replied.
/// </summary>
public sealed class BankTransferDecision
{
    /// <summary>
    /// Gets the server-generated identifier for the simulated transfer.
    /// Output: a stable identifier for reporting and audit evidence.
    /// Business case: banking systems need a reference that can be logged and traced.
    /// </summary>
    [JsonPropertyName("transferId")]
    public string TransferId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the resulting business status of the transfer.
    /// Output: values such as Approved or Rejected.
    /// Business case: the consumer needs an explicit business decision to continue or stop the payment flow.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amount approved by the rule engine.
    /// Output: the approved monetary amount or zero when the transfer is rejected.
    /// Business case: partial or full approvals must be observable in the API response.
    /// </summary>
    [JsonPropertyName("approvedAmount")]
    public decimal ApprovedAmount { get; init; }

    /// <summary>
    /// Gets the remaining balance after the simulation.
    /// Output: the post-decision available balance.
    /// Business case: customer-facing channels usually display the remaining funds right after a payment decision.
    /// </summary>
    [JsonPropertyName("availableBalanceAfterTransfer")]
    public decimal AvailableBalanceAfterTransfer { get; init; }

    /// <summary>
    /// Gets the operational channel used for the transfer.
    /// Output: a value such as Instant or ManualReview.
    /// Business case: large transfers often require a slower processing path even when they are approved.
    /// </summary>
    [JsonPropertyName("processingChannel")]
    public string ProcessingChannel { get; init; } = string.Empty;

    /// <summary>
    /// Gets the business message returned to the consumer.
    /// Output: a human-readable explanation of the result.
    /// Business case: support and product teams use this message to explain the business outcome to end users.
    /// </summary>
    [JsonPropertyName("businessMessage")]
    public string BusinessMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the rejection reason when the transfer is denied.
    /// Output: a machine-readable reason or an empty string when approved.
    /// Business case: downstream systems can branch on structured business reasons instead of parsing free text.
    /// </summary>
    [JsonPropertyName("rejectionReason")]
    public string RejectionReason { get; init; } = string.Empty;
}
