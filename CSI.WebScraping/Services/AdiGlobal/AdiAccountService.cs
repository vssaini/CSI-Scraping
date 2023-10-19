using CSI.Common;
using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.ComponentModel;
using OpenQA.Selenium.Interactions;

namespace CSI.WebScraping.Services.AdiGlobal;

internal class AdiAccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly AdiConfig _adiConfig;

    public AdiAccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _adiConfig = AdiConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Navigating to URL {_adiConfig.HomeUrl}");
        _driver.Navigate().GoToUrl(_adiConfig.HomeUrl);

        _bgWorker.ReportProgress(0, $"Signing on {Constants.Website.AdiGlobal} using URL '{_adiConfig.HomeUrl}' with username '{_adiConfig.Username}' and password '{_adiConfig.Password}'");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        //var signInLink = wait.Until(d => d.FindElement(By.CssSelector(".account-menu .login-menu")));

        //var action = new Actions(_driver);
        //action.MoveToElement(signInLink).Click().Build().Perform();

        //_driver.SaveBsScreenshot(_bgWorker, _adiConfig, "SignInLinkClicked");

        //// Set active class so that account-menu-dropdown-login show up
        ////_driver.Script().ExecuteScript("document.getElementById('myaccountdropdownunauthenticated').setAttribute('class', 'active')");

        //IWebElement txtUsername = wait.Until(d => d.FindElement(By.CssSelector(".account-menu-dropdown-login .sign-in-modal .sign-in-field .UINX_usernameunder")));
        //txtUsername.SendKeys(_adiConfig.Username);

        //IWebElement txtPassword = wait.Until(d => d.FindElement(By.CssSelector(".account-menu-dropdown-login .sign-in-modal .sign-in-field .UINX_passwordunder")));
        //txtPassword.SendKeys(_adiConfig.Password);

        //wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        //IWebElement btnSignIn = wait.Until(x => x.FindElement(By.CssSelector(".account-menu-dropdown-login .sign-in-modal .sign-in-btn")));
        //btnSignIn.Click();

        //_driver.SaveBsScreenshot(_bgWorker, _adiConfig, "SignInModal");

        //_bgWorker.ReportProgress(0, $"Signed on {Constants.Website.AdiGlobal} successfully!");
    }
}