using FrameworkBase.Automation.Api.Clients;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Tests.Common;
using FrameworkBase.Automation.Api.Tests.Support;

namespace FrameworkBase.Automation.Api.Tests;

/// <summary>
/// Demonstrates ATDD-style API scenarios against the local business mock server.
/// Input: business-facing banking and retail requests sent through the HTTP client layer.
/// Output: end-to-end assertions over status code, payload body, and business meaning.
/// Business case: acceptance tests prove that the mock API behaves the way stakeholders expect at contract level.
/// </summary>
public sealed class BusinessAcceptanceApiTests
{
    /// <summary>
    /// Verifies that a consumer banking transfer is approved as an instant payment when the account has enough balance.
    /// Business case: the bank should confirm the payment and show the remaining balance in the same response.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("ATDD")]
    public async Task Banking_transfer_should_be_approved_with_business_context()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);
        using var server = LocalBusinessApiStubServer.Start(settings.Api);
        var client = new BusinessScenarioApiClient(server.ApiSettings);
        var request = new BankTransferRequest
        {
            SourceAccountId = "CHK-200",
            DestinationAccountId = "SAV-300",
            Amount = 850m,
            AvailableBalance = 2400m,
            DailyTransferLimit = 5000m,
            Currency = "USD",
            CustomerTier = "Standard",
        };

        var response = await client.SimulateBankTransferAsync(request);

        artifactLogger.WriteText(
            nameof(BusinessAcceptanceApiTests),
            "banking-transfer-approved.json",
            response.RawBody);

        ApiBusinessAssertions.AssertApprovedTransfer(response, request, "Instant");
    }

    /// <summary>
    /// Verifies that the banking API rejects a transfer when the daily limit is exceeded even though funds are available.
    /// Business case: regulatory or operational caps should be explicit in the response body returned to the consumer.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("ATDD")]
    public async Task Banking_transfer_should_be_rejected_when_daily_limit_is_exceeded()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);
        using var server = LocalBusinessApiStubServer.Start(settings.Api);
        var client = new BusinessScenarioApiClient(server.ApiSettings);
        var request = new BankTransferRequest
        {
            SourceAccountId = "CHK-201",
            DestinationAccountId = "BROKER-001",
            Amount = 6000m,
            AvailableBalance = 20000m,
            DailyTransferLimit = 5000m,
            Currency = "USD",
            CustomerTier = "Gold",
        };

        var response = await client.SimulateBankTransferAsync(request);

        artifactLogger.WriteText(
            nameof(BusinessAcceptanceApiTests),
            "banking-transfer-limit-rejection.json",
            response.RawBody);

        ApiBusinessAssertions.AssertRejectedTransfer(response, request, "DailyLimitExceeded");
    }

    /// <summary>
    /// Verifies that a gold retail customer receives a loyalty discount and free shipping in the pricing response.
    /// Business case: the shopping cart should reflect the campaign benefit directly in the API contract.
    /// </summary>
    [Test]
    [Category("Api")]
    [Category("ATDD")]
    public async Task Retail_quote_should_apply_loyalty_promotion_and_free_shipping()
    {
        var settings = AutomationTestSession.LoadSettings();
        var artifactLogger = AutomationTestSession.CreateArtifactLogger(settings);
        using var server = LocalBusinessApiStubServer.Start(settings.Api);
        var client = new BusinessScenarioApiClient(server.ApiSettings);
        var request = new RetailPriceQuoteRequest
        {
            OrderId = "ORDER-500",
            CustomerSegment = "Gold",
            Subtotal = 300m,
            ItemsCount = 4,
            ShippingCountry = "CO",
        };

        var response = await client.SimulateRetailQuoteAsync(request);

        artifactLogger.WriteText(
            nameof(BusinessAcceptanceApiTests),
            "retail-quote-gold-customer.json",
            response.RawBody);

        ApiBusinessAssertions.AssertRetailQuote(response, 45m, 0m, 255m, "Gold15", "HomeDelivery");
    }
}
