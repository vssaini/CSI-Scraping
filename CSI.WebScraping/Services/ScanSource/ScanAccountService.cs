using CSI.Common;
using CSI.Common.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.ComponentModel;
using System.IO;

namespace CSI.WebScraping.Services.ScanSource;

internal class ScanAccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly ScanSourceConfig _ssConfig;

    private const string WebsiteName = "ScanSource";

    public ScanAccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _ssConfig = ScanSourceConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Signing on {WebsiteName} using URL '{_ssConfig.HomeUrl}' with username '{_ssConfig.Username}' and password '{_ssConfig.Password}'");

        _bgWorker.ReportProgress(0, $"Navigating to URL {_ssConfig.HomeUrl}");
        _driver.Navigate().GoToUrl(_ssConfig.HomeUrl);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        var signInLink = wait.Until(x => x.FindElement(By.Id("accountMenu")));

        var action = new Actions(_driver);
        action.MoveToElement(signInLink).Click().Build().Perform();

        IWebElement txtEmail = wait.Until(x => x.FindElement(By.Id("email")));
        txtEmail.SendKeys(_ssConfig.Username);

        _driver.FindElement(By.Id("password")).SendKeys(_ssConfig.Password);
        _driver.FindElement(By.Id("next")).Click();

        SaveScreenshotIfRequested();
    }

    private void SaveScreenshotIfRequested()
    {
        if (!_ssConfig.SaveScreenshots)
            return;

        _bgWorker.ReportProgress(0, $"Saving the screenshot for the {WebsiteName} for Login step.");

        var filePath = Path.Combine(_ssConfig.ScreenshotDirectoryName, $"{WebsiteName}_Login_{DateTime.Now.ToString(Constants.DateFormat)}.png");
        var screenshot = _driver.GetScreenshot();
        screenshot.SaveAsFile(filePath);
    }
}