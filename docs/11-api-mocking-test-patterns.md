# API Mocking and Test Patterns

This document extends the original PDF topics around API testing, response validation, and BDD with a practical implementation inside this framework.

## Why this extension exists

The first implementation already proved basic API automation with `HttpClient`, `RestSharp`, and a simple local stub. The missing piece was a richer simulation layer that validates:

- HTTP status codes,
- full response bodies,
- business decisions,
- and how different testing patterns fit into the same framework.

The new implementation closes that gap with local banking and retail scenarios.

## Patterns covered inside the framework

### TDD

`TDD` is represented by fast tests that execute pure business rules before any HTTP interaction exists.

- `BusinessRuleEngineTddTests`
- `BankTransferDecisionEngine`
- `RetailPricingEngine`

These tests validate domain rules in isolation:

- insufficient funds,
- daily transfer limits,
- loyalty discounts,
- shipping rules.

### ATDD

`ATDD` is represented by executable acceptance tests that call the mock API through the client layer and validate both transport and business outcomes.

- `BusinessAcceptanceApiTests`
- `BusinessScenarioApiClient`
- `LocalBusinessApiStubServer`

These tests answer acceptance questions such as:

- Should a bank approve a transfer when funds are available?
- Should the API reject a transfer that exceeds the daily limit even if the status code is technically successful?
- Should a gold retail customer receive both loyalty discount and free shipping?

### BDD

`BDD` is represented with `SpecFlow` feature files and step definitions written in business language.

- `Features/BusinessScenarios.feature`
- `Steps/BusinessScenarioSteps.cs`

These scenarios describe the behavior in a way that business and technical profiles can read together.

## Mock API scenarios

### Banking endpoint

`POST /business/banking/transfers/simulate`

Input:

- source account,
- destination account,
- amount,
- available balance,
- daily transfer limit,
- customer tier,
- compliance hold flag.

Output:

- HTTP `200 OK` for valid requests,
- `status` in body as `Approved` or `Rejected`,
- approved amount,
- remaining balance,
- processing channel,
- business message,
- rejection reason when applicable.

Business cases:

- approve a transfer when funds are sufficient,
- reject a transfer under regulatory hold,
- reject a transfer that exceeds the daily limit,
- route high-value approved transfers to manual review.

### Retail endpoint

`POST /business/retail/pricing/quote`

Input:

- order id,
- customer segment,
- subtotal,
- item count,
- shipping country,
- coupon code,
- store pickup flag.

Output:

- HTTP `200 OK` for valid requests,
- `Quoted` status in body,
- discount amount,
- shipping fee,
- final total,
- promotion applied,
- fulfillment type,
- business message.

Business cases:

- apply `Gold15` loyalty pricing,
- apply `WELCOME10` coupon when eligible,
- grant free shipping to gold customers,
- remove shipping fee for store pickup,
- keep shipping fee when the cart does not meet the free-shipping rule.

## Function reference

### `BusinessScenarioApiClient`

| Function | Input | Output | Business case |
| --- | --- | --- | --- |
| `BusinessScenarioApiClient(ApiSettings, HttpClient?)` | API settings and optional HTTP client | Configured API client | Reuse framework configuration while pointing tests to the local mock service |
| `SimulateBankTransferAsync(BankTransferRequest, CancellationToken)` | Banking transfer request | `ApiResponse<BankTransferDecision>` | Validate status code, raw payload, and bank decision in the same test |
| `SimulateRetailQuoteAsync(RetailPriceQuoteRequest, CancellationToken)` | Retail pricing request | `ApiResponse<RetailPriceQuote>` | Validate totals, promotions, and fulfillment behavior through the API contract |

### `BankTransferDecisionEngine`

| Function | Input | Output | Business case |
| --- | --- | --- | --- |
| `Evaluate(BankTransferRequest)` | Transfer request with balances, limits, and compliance flags | `BankTransferDecision` | Simulate the core bank rules before the HTTP layer |

### `RetailPricingEngine`

| Function | Input | Output | Business case |
| --- | --- | --- | --- |
| `CalculateQuote(RetailPriceQuoteRequest)` | Retail cart request | `RetailPriceQuote` | Simulate promotions, shipping, and checkout totals |

### `LocalBusinessApiStubServer`

| Function | Input | Output | Business case |
| --- | --- | --- | --- |
| `Start(ApiSettings)` | Base framework API settings | Running local mock server | Isolate API tests from third-party systems |
| `Dispose()` | No external input | Stops the server | Keep scenarios independent and repeatable |

### `ApiBusinessAssertions`

| Function | Input | Output | Business case |
| --- | --- | --- | --- |
| `AssertApprovedTransfer(...)` | Banking API response and original request | FluentAssertions checks | Prove that approval logic and remaining balance are correct |
| `AssertRejectedTransfer(...)` | Banking API response and original request | FluentAssertions checks | Prove that business rejection is explicit in the body even with HTTP `200` |
| `AssertRetailQuote(...)` | Retail API response and expected commercial values | FluentAssertions checks | Prove that quote totals match campaign rules |

## Why the framework now goes beyond status code validation

A status code alone cannot answer the business question.

Examples:

- A banking transfer can return HTTP `200` and still be rejected because of `InsufficientFunds` or `DailyLimitExceeded`.
- A retail quote can return HTTP `200` and still be wrong if the discount, shipping fee, or final total do not match the promotion policy.

That is why the client returns:

- `StatusCode`,
- `RawBody`,
- `Body`,
- `IsSuccessStatusCode`.

This makes the framework suitable for both technical and business assertions.

## How to execute the new examples

Run the API project with:

```powershell
dotnet test tests/FrameworkBase.Automation.Api.Tests/FrameworkBase.Automation.Api.Tests.csproj --filter "Category=Api"
```

Run only TDD examples:

```powershell
dotnet test tests/FrameworkBase.Automation.Api.Tests/FrameworkBase.Automation.Api.Tests.csproj --filter "Category=TDD"
```

Run only ATDD examples:

```powershell
dotnet test tests/FrameworkBase.Automation.Api.Tests/FrameworkBase.Automation.Api.Tests.csproj --filter "Category=ATDD"
```

Run only BDD examples:

```powershell
dotnet test tests/FrameworkBase.Automation.Api.Tests/FrameworkBase.Automation.Api.Tests.csproj --filter "Category=BDD"
```

## Recommended explanation in an interview

When someone asks how this framework validates APIs, the strongest answer is:

1. We isolate the service with a local mock API.
2. We validate status code, payload contract, and business meaning together.
3. We demonstrate the same capability through TDD, ATDD, and BDD layers.
4. We keep the business rules deterministic so the suite is stable and repeatable.
