@desktop
Feature: Windows calculator
  As an automation engineer
  I want to validate a desktop flow in Windows
  So that I can extend the same framework principles beyond web and API

  Scenario: Add two numbers in Calculator
    Given the Windows Calculator desktop app is available
    When I add one and two in Calculator
    Then Calculator should display three
