Feature: API posts
  As an automation engineer
  I want to validate API contracts through different .NET clients
  So that I can prove the API layer and BDD layer work together

  Scenario: Retrieve a post
    Given the JSONPlaceholder API is available
    When I request a known post by id
    Then the API should return the expected post data
