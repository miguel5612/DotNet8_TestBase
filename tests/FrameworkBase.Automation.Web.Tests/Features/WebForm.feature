Feature: Web form submission
  As an automation engineer
  I want to validate a stable browser flow
  So that I can prove the layered framework works end to end

  Scenario: Submit the Selenium demo form
    Given the Selenium sample form is available
    When I submit valid candidate data in the form
    Then the page should confirm the submission
