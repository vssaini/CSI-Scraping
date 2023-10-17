﻿using System;
using System.ComponentModel;
using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CSI.WebScraping.Services.AdiGlobal;

internal class AdiAccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly AdiConfig _ssConfig;

    private const string WebsiteName = "ADI Global";

    public AdiAccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _ssConfig = AdiConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Signing on {WebsiteName} using URL '{_ssConfig.HomeUrl}' with username '{_ssConfig.Username}' and password '{_ssConfig.Password}'");

        _bgWorker.ReportProgress(0, $"Navigating to URL {_ssConfig.HomeUrl}");
        _driver.Navigate().GoToUrl(_ssConfig.HomeUrl);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        var signInLink = wait.Until(d => d.FindElement(By.Id("accountMenu")));

        var action = new Actions(_driver);
        action.MoveToElement(signInLink).Click().Build().Perform();

        _driver.SaveScreenshot(_bgWorker, _ssConfig, "HomePage");

        IWebElement txtEmail = wait.Until(d => d.FindElement(By.Id("email")));
        txtEmail.SendKeys(_ssConfig.Username);

        IWebElement txtPassword = wait.Until(d => d.FindElement(By.Id("password")));
        txtPassword.SendKeys(_ssConfig.Password);

        // Search by enter
        //txtPassword.SendKeys(Keys.Enter);

        wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        IWebElement btnSignIn = wait.Until(x => x.FindElement(By.Id("next")));
        btnSignIn.Click();

        _driver.SaveScreenshot(_bgWorker, _ssConfig, "AzureLogin");
    }
}