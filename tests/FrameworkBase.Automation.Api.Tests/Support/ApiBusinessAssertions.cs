using System.Net;
using FrameworkBase.Automation.Api.Models;
using FluentAssertions;

namespace FrameworkBase.Automation.Api.Tests.Support;

/// <summary>
/// Centralizes API assertions that combine HTTP status validation with business validation.
/// Input: API responses plus the request or expectation context for the scenario.
/// Output: FluentAssertions checks that fail with business-oriented messages.
/// Business case: keep acceptance tests readable while still verifying contract and business rules together.
/// </summary>
public static class ApiBusinessAssertions
{
    /// <summary>
    /// Validates that the banking API approved the transfer and preserved the expected business outcome.
    /// Input: the API response, the original request, and the expected processing channel.
    /// Output: assertions covering HTTP status, business status, approved amount, remaining balance, and explanatory message.
    /// Business case: a successful transfer must prove both the transport result and the operational decision returned by the banking service.
    /// </summary>
    /// <param name="response">The API response returned by the banking simulation endpoint.</param>
    /// <param name="request">The original banking transfer request.</param>
    /// <param name="expectedChannel">The expected operational channel such as Instant or ManualReview.</param>
    public static void AssertApprovedTransfer(
        ApiResponse<BankTransferDecision> response,
        BankTransferRequest request,
        string expectedChannel)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Body.Should().NotBeNull();
        response.RawBody.Should().Contain("\"status\"");

        var body = response.Body!;
        body.Status.Should().Be("Approved");
        body.ApprovedAmount.Should().Be(request.Amount);
        body.AvailableBalanceAfterTransfer.Should().Be(request.AvailableBalance - request.Amount);
        body.ProcessingChannel.Should().Be(expectedChannel);
        body.RejectionReason.Should().BeEmpty();
        body.BusinessMessage.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Validates that the banking API rejected the transfer with the expected business reason.
    /// Input: the API response, the original request, and the expected rejection reason.
    /// Output: assertions covering HTTP status, rejection state, approved amount, balance preservation, and rejection reason.
    /// Business case: business rejections often still return HTTP 200, so the body must be validated carefully.
    /// </summary>
    /// <param name="response">The API response returned by the banking simulation endpoint.</param>
    /// <param name="request">The original banking transfer request.</param>
    /// <param name="expectedReason">The structured rejection reason expected for the scenario.</param>
    public static void AssertRejectedTransfer(
        ApiResponse<BankTransferDecision> response,
        BankTransferRequest request,
        string expectedReason)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Body.Should().NotBeNull();

        var body = response.Body!;
        body.Status.Should().Be("Rejected");
        body.ApprovedAmount.Should().Be(0m);
        body.AvailableBalanceAfterTransfer.Should().Be(request.AvailableBalance);
        body.ProcessingChannel.Should().Be("Blocked");
        body.RejectionReason.Should().Be(expectedReason);
        body.BusinessMessage.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Validates that the retail API returned the expected pricing outcome for the scenario.
    /// Input: the API response plus the expected commercial values.
    /// Output: assertions covering HTTP status, quote status, discount, shipping fee, final total, and promotion label.
    /// Business case: retail automation must prove that the final amount charged to the customer matches the promotion strategy.
    /// </summary>
    /// <param name="response">The API response returned by the retail pricing endpoint.</param>
    /// <param name="expectedDiscount">The discount amount expected for the quote.</param>
    /// <param name="expectedShippingFee">The shipping fee expected for the quote.</param>
    /// <param name="expectedFinalTotal">The final amount expected to be charged to the customer.</param>
    /// <param name="expectedPromotion">The promotion label expected for the quote.</param>
    /// <param name="expectedFulfillmentType">The expected fulfillment type such as HomeDelivery or StorePickup.</param>
    public static void AssertRetailQuote(
        ApiResponse<RetailPriceQuote> response,
        decimal expectedDiscount,
        decimal expectedShippingFee,
        decimal expectedFinalTotal,
        string expectedPromotion,
        string expectedFulfillmentType)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Body.Should().NotBeNull();
        response.RawBody.Should().Contain("\"finalTotal\"");

        var body = response.Body!;
        body.Status.Should().Be("Quoted");
        body.DiscountAmount.Should().Be(expectedDiscount);
        body.ShippingFee.Should().Be(expectedShippingFee);
        body.FinalTotal.Should().Be(expectedFinalTotal);
        body.PromotionApplied.Should().Be(expectedPromotion);
        body.FulfillmentType.Should().Be(expectedFulfillmentType);
        body.BusinessMessage.Should().NotBeNullOrWhiteSpace();
    }
}
