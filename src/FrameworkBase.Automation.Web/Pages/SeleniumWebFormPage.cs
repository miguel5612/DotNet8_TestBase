using FrameworkBase.Automation.Core.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FrameworkBase.Automation.Web.Pages;

public sealed class SeleniumWebFormPage
{
    private readonly IWebDriver driver;
    private readonly WebSettings settings;

    public SeleniumWebFormPage(IWebDriver driver, WebSettings settings)
    {
        this.driver = driver;
        this.settings = settings;
    }

    public void Open()
    {
        driver.Navigate().GoToUrl(settings.BaseUrl);
    }

    public void EnterText(string text)
    {
        var textInput = driver.FindElement(By.Id("my-text-id"));
        textInput.Clear();
        textInput.SendKeys(text);
    }

    public void EnterPassword(string password)
    {
        var passwordInput = driver.FindElement(By.Name("my-password"));
        passwordInput.Clear();
        passwordInput.SendKeys(password);
    }

    public void EnterComments(string comments)
    {
        var textArea = driver.FindElement(By.Name("my-textarea"));
        textArea.Clear();
        textArea.SendKeys(comments);
    }

    public void SelectOption(string optionText)
    {
        var select = new SelectElement(driver.FindElement(By.Name("my-select")));
        select.SelectByText(optionText);
    }

    public void AcceptDefaultCheckBox()
    {
        var checkBox = driver.FindElement(By.Id("my-check-1"));

        if (!checkBox.Selected)
        {
            checkBox.Click();
        }
    }

    public void SelectDefaultRadio()
    {
        var radioButton = driver.FindElement(By.Id("my-radio-1"));

        if (!radioButton.Selected)
        {
            radioButton.Click();
        }
    }

    public void Submit()
    {
        driver.FindElement(By.CssSelector("button")).Click();
    }

    public string ReadConfirmationMessage()
    {
        return driver.FindElement(By.Id("message")).Text;
    }
}
