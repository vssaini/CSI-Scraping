using System;
using System.ComponentModel;
using CSI.Common;
using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CSI.WebScraping.Services.BHPhotoVideo;

internal class BHAccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly BHConfig _bhConfig;

    public BHAccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _bhConfig = BHConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Signing on {Constants.Website.BHPhotoVideo} using URL '{_bhConfig.HomeUrl}' with username '{_bhConfig.Username}' and password '{_bhConfig.Password}'");

        _bgWorker.ReportProgress(0, $"Navigating to URL {_bhConfig.HomeUrl}");
        _driver.Navigate().GoToUrl(_bhConfig.HomeUrl);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        var signInLink = wait.Until(d => d.FindElement(By.Id("accountMenu")));

        var action = new Actions(_driver);
        action.MoveToElement(signInLink).Click().Build().Perform();

        _driver.SaveScreenshot(_bgWorker, _bhConfig, "HomePage");

        IWebElement txtEmail = wait.Until(d => d.FindElement(By.Id("email")));
        txtEmail.SendKeys(_bhConfig.Username);

        IWebElement txtPassword = wait.Until(d => d.FindElement(By.Id("password")));
        txtPassword.SendKeys(_bhConfig.Password);

        // Search by enter
        //txtPassword.SendKeys(Keys.Enter);

        wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        IWebElement btnSignIn = wait.Until(x => x.FindElement(By.Id("next")));
        btnSignIn.Click();

        _driver.SaveScreenshot(_bgWorker, _bhConfig, "AzureLogin");
    }
}