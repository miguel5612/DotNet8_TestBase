@Api
@BDD
Feature: Business API simulations
  As an automation engineer
  I want to validate business-oriented API responses with a local mock server
  So that the framework proves BDD scenarios beyond simple transport checks

  Scenario: Approve an instant banking transfer
    Given the business mock API is available
    When I simulate a banking transfer with sufficient funds
    Then the banking response should approve the transfer as an instant payment

  Scenario: Apply loyalty pricing for a gold retail customer
    Given the business mock API is available
    When I simulate a retail quote for a gold customer
    Then the retail response should apply the loyalty promotion and free shipping
