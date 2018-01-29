using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Web.Tests.Pages
{
    public class AuthPage
    {
        public AuthPage(IWebDriver driver)
        {
            PageFactory.InitElements(driver, this);
        }
    }
}
