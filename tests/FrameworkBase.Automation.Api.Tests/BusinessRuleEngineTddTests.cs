using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Api.Rules;
using FluentAssertions;

namespace FrameworkBase.Automation.Api.Tests;

/// <summary>
/// Demonstrates TDD-style tests for pure business rules before the HTTP layer is involved.
/// Input: deterministic banking and retail requests.
/// Output: direct assertions over rule-engine results.
/// Business case: fast unit tests validate core business decisions without the overhead of network or BDD layers.
/// </summary>
public sealed class BusinessRuleEngineTddTests
{
    /// <summary>
    /// Verifies that a banking transfer is rejected when the account balance is insufficient.
    /// Business case: a bank must never approve a transfer that exceeds the available funds.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("TDD")]
    public void Bank_transfer_should_be_rejected_when_balance_is_insufficient()
    {
        var engine = new BankTransferDecisionEngine();
        var request = new BankTransferRequest
        {
            SourceAccountId = "CHK-001",
            DestinationAccountId = "SAV-002",
            Amount = 1500m,
            AvailableBalance = 900m,
            DailyTransferLimit = 5000m,
        };

        var result = engine.Evaluate(request);

        result.Status.Should().Be("Rejected");
        result.RejectionReason.Should().Be("InsufficientFunds");
        result.AvailableBalanceAfterTransfer.Should().Be(900m);
        result.ApprovedAmount.Should().Be(0m);
    }

    /// <summary>
    /// Verifies that a large approved transfer is routed to manual review instead of instant processing.
    /// Business case: banks often require additional operational control for high-value payments.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("TDD")]
    public void Bank_transfer_should_route_large_approved_operations_to_manual_review()
    {
        var engine = new BankTransferDecisionEngine();
        var request = new BankTransferRequest
        {
            SourceAccountId = "CHK-003",
            DestinationAccountId = "INV-010",
            Amount = 3500m,
            AvailableBalance = 7000m,
            DailyTransferLimit = 6000m,
            CustomerTier = "Gold",
        };

        var result = engine.Evaluate(request);

        result.Status.Should().Be("Approved");
        result.ProcessingChannel.Should().Be("ManualReview");
        result.AvailableBalanceAfterTransfer.Should().Be(3500m);
        result.BusinessMessage.Should().Contain("manual review");
    }

    /// <summary>
    /// Verifies that a gold retail customer receives the loyalty discount and free shipping.
    /// Business case: loyalty benefits are a common retail acceptance criterion that must stay stable across releases.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("TDD")]
    public void Retail_quote_should_apply_gold_discount_and_free_shipping()
    {
        var engine = new RetailPricingEngine();
        var request = new RetailPriceQuoteRequest
        {
            OrderId = "ORDER-100",
            CustomerSegment = "Gold",
            Subtotal = 300m,
            ItemsCount = 3,
            ShippingCountry = "CO",
        };

        var result = engine.CalculateQuote(request);

        result.DiscountAmount.Should().Be(45m);
        result.ShippingFee.Should().Be(0m);
        result.FinalTotal.Should().Be(255m);
        result.PromotionApplied.Should().Be("Gold15");
    }

    /// <summary>
    /// Verifies that a standard retail customer can use a welcome coupon while still paying shipping below the free-shipping threshold.
    /// Business case: promotions and shipping should be calculated independently when the campaign rules require it.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("TDD")]
    public void Retail_quote_should_apply_coupon_without_free_shipping_below_threshold()
    {
        var engine = new RetailPricingEngine();
        var request = new RetailPriceQuoteRequest
        {
            OrderId = "ORDER-101",
            CustomerSegment = "Standard",
            Subtotal = 80m,
            ItemsCount = 2,
            CouponCode = "WELCOME10",
            ShippingCountry = "CO",
        };

        var result = engine.CalculateQuote(request);

        result.DiscountAmount.Should().Be(8m);
        result.ShippingFee.Should().Be(12.50m);
        result.FinalTotal.Should().Be(84.50m);
        result.PromotionApplied.Should().Be("Welcome10");
    }
}
