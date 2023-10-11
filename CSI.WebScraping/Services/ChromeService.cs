using CSI.Common.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace CSI.WebScraping.Services
{
    internal class ChromeService
    {
        private readonly BackgroundWorker _bgWorker;
        private ChromeDriverConfig _cdConfig;

        public bool SaveMilestoneScreenshots => _cdConfig.SaveMilestoneScreenshots;

        public ChromeService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;

            InitChromeDriverConfig();
        }

        private void InitChromeDriverConfig()
        {
            _cdConfig = new ChromeDriverConfig
            {
                HideCommandPromptWindow = Convert.ToBoolean(ConfigurationManager.AppSettings["ChromeDriver:HideCommandPromptWindow"]),
                SaveMilestoneScreenshots = Convert.ToBoolean(ConfigurationManager.AppSettings["ChromeDriver:SaveMilestoneScreenshots"]),
                ImplicitWaitSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["ChromeDriver:ImplicitWaitSeconds"])
            };
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
            var wesConfig = GetWescoConfig();

            _bgWorker.ReportProgress(0, $"Signing on Wesco using URL '{wesConfig.LoginUrl}' with username '{wesConfig.Username}' and password '{wesConfig.Password}'");

            driver.Navigate().GoToUrl(wesConfig.LoginUrl);

            driver.FindElement(By.Id("j_username")).SendKeys(wesConfig.Username);
            driver.FindElement(By.Id("j_password")).SendKeys(wesConfig.Password);

            if (_cdConfig.SaveMilestoneScreenshots)
            {
                var snapshot = driver.GetScreenshot();
                snapshot.SaveAsFile("Screenshot_Wesco_Login.png");
            }

            driver.FindElement(By.CssSelector("button.button")).Click();
        }

        private static WescoConfig GetWescoConfig()
        {
            return new WescoConfig
            {
                LoginUrl = ConfigurationManager.AppSettings["WescoLoginUrl"],
                Username = ConfigurationManager.AppSettings["WescoUsername"],
                Password = ConfigurationManager.AppSettings["WescoPassword"]
            };
        }
    }
}
