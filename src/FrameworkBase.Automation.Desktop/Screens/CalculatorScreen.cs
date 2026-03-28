using FrameworkBase.Automation.Core.Extensions;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace FrameworkBase.Automation.Desktop.Screens;

public sealed class CalculatorScreen
{
    private readonly WindowsDriver session;

    public CalculatorScreen(WindowsDriver session)
    {
        this.session = session;
    }

    public string Add(int left, int right)
    {
        Clear();
        InputNumber(left);
        session.FindElement(MobileBy.AccessibilityId("plusButton")).Click();
        InputNumber(right);
        session.FindElement(MobileBy.AccessibilityId("equalButton")).Click();
        return ReadResult();
    }

    public string ReadResult()
    {
        var rawText = session.FindElement(MobileBy.AccessibilityId("CalculatorResults")).Text;
        var normalizedText = rawText.NormalizeWhitespace();
        var numericSuffix = Regex.Match(normalizedText, @"[-+]?\d+(?:[.,]\d+)*$");

        return numericSuffix.Success
            ? numericSuffix.Value
            : normalizedText;
    }

    private void Clear()
    {
        var clearButtons = new[]
        {
            "clearButton",
            "clearEntryButton",
        };

        foreach (var clearButtonId in clearButtons)
        {
            var buttons = session.FindElements(MobileBy.AccessibilityId(clearButtonId));

            if (buttons.Count > 0)
            {
                buttons[0].Click();
                return;
            }
        }
    }

    private void InputNumber(int value)
    {
        foreach (var digit in value.ToString())
        {
            session.FindElement(MobileBy.AccessibilityId($"num{digit}Button")).Click();
        }
    }
}
