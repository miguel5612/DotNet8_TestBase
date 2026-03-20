using System.Text.Json.Serialization;

namespace FrameworkBase.Automation.Api.Models;

/// <summary>
/// Describes the input required to simulate a banking transfer.
/// Input: account identifiers, available balance, transfer amount, customer tier, and compliance flags.
/// Output: a request payload consumed by the banking mock API.
/// Business case: model the conditions that determine whether a transfer is approved, rejected, or routed for review.
/// </summary>
public sealed class BankTransferRequest
{
    /// <summary>
    /// Gets the source account identifier.
    /// Input: the account that sends the money.
    /// Output: a string used by the API and business rules.
    /// Business case: the rule engine needs a traceable source account for the simulated transfer.
    /// </summary>
    [JsonPropertyName("sourceAccountId")]
    public string SourceAccountId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the destination account identifier.
    /// Input: the account that receives the money.
    /// Output: a string used by the API and business rules.
    /// Business case: transfer simulations should preserve the target account expected by the business scenario.
    /// </summary>
    [JsonPropertyName("destinationAccountId")]
    public string DestinationAccountId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amount requested for the transfer.
    /// Input: a positive monetary amount.
    /// Output: the amount to evaluate against balance and limits.
    /// Business case: banking workflows depend on the transferred amount to detect approval or rejection conditions.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    /// <summary>
    /// Gets the currency associated with the transfer.
    /// Input: a three-letter currency code such as USD or COP.
    /// Output: the currency echoed by the mock service.
    /// Business case: financial responses must be explicit about the currency being processed.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Gets the available balance before the transfer is evaluated.
    /// Input: the account balance available to the transfer engine.
    /// Output: a value used to calculate funds sufficiency and the remaining balance.
    /// Business case: the main acceptance rule in a transfer is whether the customer has enough money to cover the requested amount.
    /// </summary>
    [JsonPropertyName("availableBalance")]
    public decimal AvailableBalance { get; init; }

    /// <summary>
    /// Gets the daily limit configured for the account.
    /// Input: the maximum amount allowed for the current business day.
    /// Output: a threshold evaluated by the rule engine.
    /// Business case: banks routinely reject operations that exceed operational or regulatory daily limits.
    /// </summary>
    [JsonPropertyName("dailyTransferLimit")]
    public decimal DailyTransferLimit { get; init; } = 5000m;

    /// <summary>
    /// Gets the customer tier used to route the transfer.
    /// Input: a segment such as Standard, Gold, or Platinum.
    /// Output: a value echoed in the rule outcome.
    /// Business case: premium customers may qualify for faster operational channels in some flows.
    /// </summary>
    [JsonPropertyName("customerTier")]
    public string CustomerTier { get; init; } = "Standard";

    /// <summary>
    /// Gets a value indicating whether the account is under regulatory hold.
    /// Input: <c>true</c> when a compliance block is active.
    /// Output: a flag evaluated by the decision engine.
    /// Business case: compliance restrictions should reject the transfer regardless of the available balance.
    /// </summary>
    [JsonPropertyName("hasRegulatoryHold")]
    public bool HasRegulatoryHold { get; init; }
}
