using CSI.Common;
using CSI.Common.Config;
using OpenQA.Selenium;
using System;
using System.ComponentModel;
using System.IO;

namespace CSI.WebScraping.Services.Wesco;

internal class AccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly WescoConfig _wesConfig;

    public AccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _wesConfig = WescoConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Signing on Wesco using URL '{_wesConfig.LoginUrl}' with username '{_wesConfig.Username}' and password '{_wesConfig.Password}'");

        _driver.Navigate().GoToUrl(_wesConfig.LoginUrl);

        _driver.FindElement(By.Id("j_username")).SendKeys(_wesConfig.Username);
        _driver.FindElement(By.Id("j_password")).SendKeys(_wesConfig.Password);

        // TODO: Implement Polly retry for 3 times and then throw exception

        SaveScreenshotIfRequested();

        _driver.FindElement(By.CssSelector("button.button")).Click();
    }

    private void SaveScreenshotIfRequested()
    {
        if (!_wesConfig.SaveScreenshots) 
            return;

        _bgWorker.ReportProgress(0, "Saving the screenshot for the Wesco login page.");

        var filePath = Path.Combine(_wesConfig.ScreenshotDirectoryName, $"Wesco_Login_{DateTime.Now.ToString(Constants.DateFormat)}.png");
        var screenshot = _driver.GetScreenshot();
        screenshot.SaveAsFile(filePath);
    }
}