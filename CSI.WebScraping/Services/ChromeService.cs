using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CSI.WebScraping.Services
{
    internal class ChromeService
    {
        private readonly BackgroundWorker _bgWorker;
        public ChromeService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
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
            chromeDriverService.HideCommandPromptWindow = HideChromeDriverCommandPromptWindow();
            return new ChromeDriver(chromeDriverService, chromeOptions);
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

        private static bool HideChromeDriverCommandPromptWindow()
        {
            try
            {
                var hideCmdPromptVal = ConfigurationManager.AppSettings["ChromeDriver:HideCommandPromptWindow"];
                return bool.TryParse(hideCmdPromptVal, out var hideCmdPrompt) && hideCmdPrompt;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Login(WebDriver driver)
        {
            var loginUrl = ConfigurationManager.AppSettings["WescoLoginUrl"];
            var username = ConfigurationManager.AppSettings["WescoUsername"];
            var password = ConfigurationManager.AppSettings["WescoPassword"];

            _bgWorker.ReportProgress(0, $"Logging in to Wesco on URL '{loginUrl}' with username '{username}' and password '{password}'");

            driver.Navigate().GoToUrl(loginUrl);

            driver.FindElement(By.Id("j_username")).SendKeys(username);
            driver.FindElement(By.Id("j_password")).SendKeys(password);

            var saveMilestoneScreenshots = ConfigurationManager.AppSettings["ChromeDriver:SaveMilestoneScreenshots"] == "true";
            if (saveMilestoneScreenshots)
            {
                var snapshot = driver.GetScreenshot();
                snapshot.SaveAsFile("Screenshot_Wesco_Login.png");
            }

            driver.FindElement(By.CssSelector("button.button")).Click();

            WaitForMilliseconds(5000);
        }

        public void WaitForMilliseconds(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
