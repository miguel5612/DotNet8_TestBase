using FrameworkBase.Automation.Api.Clients;
using FrameworkBase.Automation.Api.Models;
using FrameworkBase.Automation.Api.Tests.Support;
using FrameworkBase.Automation.Tests.Common;
using TechTalk.SpecFlow;

namespace FrameworkBase.Automation.Api.Tests.Steps;

/// <summary>
/// Implements BDD steps for the business mock API scenarios.
/// Input: high-level banking and retail business actions described in feature files.
/// Output: executable step definitions that call the local business mock service.
/// Business case: analysts, QA engineers, and developers can read the same scenarios and verify the same business behavior.
/// </summary>
[Binding]
public sealed class BusinessScenarioSteps
{
    private BusinessScenarioApiClient? client;
    private ApiResponse<BankTransferDecision>? bankingResponse;
    private ApiResponse<RetailPriceQuote>? retailResponse;
    private LocalBusinessApiStubServer? server;

    /// <summary>
    /// Starts the local business mock API for the scenario.
    /// Output: a running local server and an API client configured to use it.
    /// Business case: each BDD scenario executes against an isolated mock service.
    /// </summary>
    [Given("the business mock API is available")]
    public void GivenTheBusinessMockApiIsAvailable()
    {
        var settings = AutomationTestSession.LoadSettings();
        server = LocalBusinessApiStubServer.Start(settings.Api);
        client = new BusinessScenarioApiClient(server.ApiSettings);
    }

    /// <summary>
    /// Sends a banking request that should be approved as an instant payment.
    /// Output: a stored API response for later assertions.
    /// Business case: the scenario represents a customer with enough funds and no compliance blockers.
    /// </summary>
    [When("I simulate a banking transfer with sufficient funds")]
    public async Task WhenISimulateABankingTransferWithSufficientFunds()
    {
        bankingResponse = await client!.SimulateBankTransferAsync(new BankTransferRequest
        {
            SourceAccountId = "CHK-901",
            DestinationAccountId = "SAV-777",
            Amount = 1200m,
            AvailableBalance = 5000m,
            DailyTransferLimit = 8000m,
            Currency = "USD",
            CustomerTier = "Gold",
        });
    }

    /// <summary>
    /// Validates that the banking response reflects an approved instant transfer.
    /// Business case: the API should expose both the successful HTTP result and the business decision for the transfer.
    /// </summary>
    [Then("the banking response should approve the transfer as an instant payment")]
    public void ThenTheBankingResponseShouldApproveTheTransferAsAnInstantPayment()
    {
        ApiBusinessAssertions.AssertApprovedTransfer(
            bankingResponse!,
            new BankTransferRequest
            {
                SourceAccountId = "CHK-901",
                DestinationAccountId = "SAV-777",
                Amount = 1200m,
                AvailableBalance = 5000m,
                DailyTransferLimit = 8000m,
                Currency = "USD",
                CustomerTier = "Gold",
            },
            "Instant");
    }

    /// <summary>
    /// Sends a retail request for a gold customer cart.
    /// Output: a stored API response for later assertions.
    /// Business case: the scenario represents a loyalty customer eligible for premium pricing benefits.
    /// </summary>
    [When("I simulate a retail quote for a gold customer")]
    public async Task WhenISimulateARetailQuoteForAGoldCustomer()
    {
        retailResponse = await client!.SimulateRetailQuoteAsync(new RetailPriceQuoteRequest
        {
            OrderId = "ORDER-902",
            CustomerSegment = "Gold",
            Subtotal = 300m,
            ItemsCount = 5,
            ShippingCountry = "CO",
        });
    }

    /// <summary>
    /// Validates that the retail response applied the loyalty promotion and free shipping.
    /// Business case: the API should show the commercial benefit exactly as the campaign requires.
    /// </summary>
    [Then("the retail response should apply the loyalty promotion and free shipping")]
    public void ThenTheRetailResponseShouldApplyTheLoyaltyPromotionAndFreeShipping()
    {
        ApiBusinessAssertions.AssertRetailQuote(retailResponse!, 45m, 0m, 255m, "Gold15", "HomeDelivery");
    }

    /// <summary>
    /// Disposes the local business mock API after each scenario.
    /// Output: the server is stopped and the network resources are released.
    /// Business case: BDD scenarios should remain isolated and repeatable across executions.
    /// </summary>
    [AfterScenario]
    public void AfterScenario()
    {
        server?.Dispose();
    }
}
