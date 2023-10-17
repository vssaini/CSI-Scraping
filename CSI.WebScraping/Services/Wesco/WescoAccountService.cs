using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using OpenQA.Selenium;
using System.ComponentModel;

namespace CSI.WebScraping.Services.Wesco;

internal class WescoAccountService
{
    private readonly BackgroundWorker _bgWorker;
    private readonly WebDriver _driver;
    private readonly WescoConfig _wesConfig;

    private const string WebsiteName = "Wesco";

    public WescoAccountService(BackgroundWorker bgWorker, WebDriver driver)
    {
        _bgWorker = bgWorker;
        _driver = driver;
        _wesConfig = WescoConfig.GetInstance();
    }

    public void Login()
    {
        _bgWorker.ReportProgress(0, $"Signing on {WebsiteName} using URL '{_wesConfig.LoginUrl}' with username '{_wesConfig.Username}' and password '{_wesConfig.Password}'");

        _driver.Navigate().GoToUrl(_wesConfig.LoginUrl);

        _driver.FindElement(By.Id("j_username")).SendKeys(_wesConfig.Username);
        _driver.FindElement(By.Id("j_password")).SendKeys(_wesConfig.Password);

        // TODO: Implement Polly retry for 3 times and then throw exception

        _driver.SaveScreenshot(_bgWorker, _wesConfig, "Login");

        _driver.FindElement(By.CssSelector("button.button")).Click();
    }
}