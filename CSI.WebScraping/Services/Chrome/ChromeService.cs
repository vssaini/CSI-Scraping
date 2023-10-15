using CSI.Common;
using CSI.Common.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CSI.WebScraping.Services.Chrome
{
    internal class ChromeService
    {
        private readonly BackgroundWorker _bgWorker;
        private readonly ChromeDriverConfig _cdConfig;

        public ChromeService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
            _cdConfig = ChromeDriverConfig.GetInstance();
        }

        public ChromeDriver GetChromeDriver()
        {
            _bgWorker.ReportProgress(0, "Initiating Chrome driver.");

            CloseGhostsChromeDriver();

            var chromeOptions = new ChromeOptions();

            // Open Chrome without displaying 
            chromeOptions.AddArgument("headless");
            chromeOptions.AddArgument("ignore-certificate-errors");

            // Disable writing of unnecessary logs
            // Valid levels are INFO = 0, WARNING = 1, LOG_ERROR = 2, LOG_FATAL = 3
            chromeOptions.AddArgument("log-level=3");

            const string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            chromeOptions.AddArgument($"user-agent={userAgent}");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = _cdConfig.HideCommandPromptWindow;

            var driver = new ChromeDriver(chromeDriverService, chromeOptions);

            // Implicitly wait for action or search
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_cdConfig.ImplicitWaitSeconds);

            return driver;
        }

        private static void CloseGhostsChromeDriver()
        {
            var cmd = Process.GetProcessesByName("cmd");
            var chromeDriver = Process.GetProcessesByName("chromedriver");

            var workers = chromeDriver.Concat(cmd).ToArray();

            foreach (var worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
        }

        //public void Login(WebDriver driver)
        //{
        //    var wesConfig = WescoConfig.GetInstance();

        //    _bgWorker.ReportProgress(0, $"Signing on Wesco using URL '{wesConfig.LoginUrl}' with username '{wesConfig.Username}' and password '{wesConfig.Password}'");

        //    driver.Navigate().GoToUrl(wesConfig.LoginUrl);

        //    driver.FindElement(By.Id("j_username")).SendKeys(wesConfig.Username);
        //    driver.FindElement(By.Id("j_password")).SendKeys(wesConfig.Password);

        //    SaveScreenshotIfRequested(driver);

        //    driver.FindElement(By.CssSelector("button.button")).Click();
        //}

        //private void SaveScreenshotIfRequested(WebDriver driver)
        //{
        //    if (!_cdConfig.SaveScreenshots) return;

        //    _bgWorker.ReportProgress(0, "Saving the screenshot for the Wesco login page.");

        //    var filePath = Path.Combine(_cdConfig.ScreenshotDirectoryName, $"Wesco_Login_{DateTime.Now.ToString(Constants.DateFormat)}.png");
        //    var screenshot = driver.GetScreenshot();
        //    screenshot.SaveAsFile(filePath);
        //}
    }
}
