using FrameworkBase.Automation.Api.Models;

namespace FrameworkBase.Automation.Api.Rules;

/// <summary>
/// Evaluates banking transfer requests against deterministic mock business rules.
/// Input: a <see cref="BankTransferRequest"/> with customer balance, limits, and compliance flags.
/// Output: a <see cref="BankTransferDecision"/> that the mock API can return to automated tests.
/// Business case: this engine lets the framework exercise realistic approval logic without relying on an external banking service.
/// </summary>
public sealed class BankTransferDecisionEngine
{
    /// <summary>
    /// Evaluates the incoming transfer request and returns a deterministic business decision.
    /// Input: a banking transfer request.
    /// Output: an approval or rejection response with business details.
    /// Business case: simulate the most common banking paths for funds validation, compliance blocks, and operational routing.
    /// </summary>
    /// <param name="request">The transfer request to evaluate.</param>
    /// <returns>The simulated transfer decision.</returns>
    public BankTransferDecision Evaluate(BankTransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        if (request.HasRegulatoryHold)
        {
            return BuildRejectedDecision(
                request,
                "RegulatoryHold",
                "The transfer was rejected because the account is under regulatory hold.");
        }

        if (request.Amount > request.AvailableBalance)
        {
            return BuildRejectedDecision(
                request,
                "InsufficientFunds",
                "The transfer was rejected because the available balance is lower than the requested amount.");
        }

        if (request.Amount > request.DailyTransferLimit)
        {
            return BuildRejectedDecision(
                request,
                "DailyLimitExceeded",
                "The transfer was rejected because it exceeds the daily transfer limit.");
        }

        var processingChannel = request.Amount <= 2000m ? "Instant" : "ManualReview";
        var balanceAfterTransfer = request.AvailableBalance - request.Amount;

        return new BankTransferDecision
        {
            TransferId = $"TRF-{request.SourceAccountId}-{request.DestinationAccountId}",
            Status = "Approved",
            ApprovedAmount = request.Amount,
            AvailableBalanceAfterTransfer = balanceAfterTransfer,
            ProcessingChannel = processingChannel,
            BusinessMessage = processingChannel == "Instant"
                ? "Transfer approved for instant execution."
                : "Transfer approved and queued for manual review due to amount threshold.",
            RejectionReason = string.Empty,
        };
    }

    private static void ValidateRequest(BankTransferRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceAccountId))
        {
            throw new ArgumentException("The source account identifier is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.DestinationAccountId))
        {
            throw new ArgumentException("The destination account identifier is required.", nameof(request));
        }

        if (request.Amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The transfer amount must be greater than zero.");
        }

        if (request.AvailableBalance < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The available balance cannot be negative.");
        }

        if (request.DailyTransferLimit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The daily transfer limit must be greater than zero.");
        }
    }

    private static BankTransferDecision BuildRejectedDecision(
        BankTransferRequest request,
        string rejectionReason,
        string businessMessage)
    {
        return new BankTransferDecision
        {
            TransferId = $"TRF-{request.SourceAccountId}-{request.DestinationAccountId}",
            Status = "Rejected",
            ApprovedAmount = 0m,
            AvailableBalanceAfterTransfer = request.AvailableBalance,
            ProcessingChannel = "Blocked",
            BusinessMessage = businessMessage,
            RejectionReason = rejectionReason,
        };
    }
}
