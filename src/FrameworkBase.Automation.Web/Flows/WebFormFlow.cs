using FrameworkBase.Automation.Web.Pages;

namespace FrameworkBase.Automation.Web.Flows;

public sealed class WebFormFlow
{
    private readonly SeleniumWebFormPage webFormPage;

    public WebFormFlow(SeleniumWebFormPage webFormPage)
    {
        this.webFormPage = webFormPage;
    }

    public string SubmitCandidateForm(string name, string password, string comments)
    {
        webFormPage.Open();
        webFormPage.EnterText(name);
        webFormPage.EnterPassword(password);
        webFormPage.EnterComments(comments);
        webFormPage.SelectOption("Two");
        webFormPage.AcceptDefaultCheckBox();
        webFormPage.SelectDefaultRadio();
        webFormPage.Submit();

        return webFormPage.ReadConfirmationMessage();
    }
}
