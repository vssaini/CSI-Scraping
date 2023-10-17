using CSI.Common.Config;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Diagnostics;
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

            var options = new ChromeOptions();

            // Open Chrome without displaying 
            //options.AddArgument("--headless=new");
            //options.AddArguments("--kiosk"); // Keep Chrome in full screen. But only works without headless.
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--window-size=1920,1080");

            // Disable writing of unnecessary logs
            // Valid levels are INFO = 0, WARNING = 1, LOG_ERROR = 2, LOG_FATAL = 3
            options.AddArgument("log-level=3");

            const string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            options.AddArgument($"user-agent={userAgent}");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = _cdConfig.HideCommandPromptWindow;

            var driver = new ChromeDriver(chromeDriverService, options);

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
    }
}
