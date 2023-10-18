using CSI.Common.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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

            // Set launch args similar to puppeteer's for best performance
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-component-extensions-with-background-pages");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-features=TranslateUI,BlinkGenPropertyTrees");
            options.AddArgument("--disable-ipc-flooding-protection");
            options.AddArgument("--disable-renderer-backgrounding");
            options.AddArgument("--enable-features=NetworkService,NetworkServiceInProcess");
            options.AddArgument("--force-color-profile=srgb");
            options.AddArgument("--hide-scrollbars");
            options.AddArgument("--metrics-recording-only");
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            
            //options.AddArgument("--headless=new"); // Open Chrome without displaying 
            //options.AddArguments("--kiosk"); // Keep Chrome in full screen. But only works without headless.
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--window-size=1920,1080");

            // Disable writing of unnecessary logs
            // Valid levels are INFO = 0, WARNING = 1, LOG_ERROR = 2, LOG_FATAL = 3
            options.AddArgument("log-level=3");

            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.5993.89 Safari/537.36");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = _cdConfig.HideCommandPromptWindow;

            var driver = new ChromeDriver(chromeDriverService, options);

            // Implicitly wait for action or search
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_cdConfig.ImplicitWaitSeconds);

            return driver;
        }

        public IWebDriver GetBrowserlessChromeDriver()      
        {
            IWebDriver driver;
            var options = new ChromeOptions();

            // Set launch args similar to puppeteer's for best performance
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-component-extensions-with-background-pages");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-features=TranslateUI,BlinkGenPropertyTrees");
            options.AddArgument("--disable-ipc-flooding-protection");
            options.AddArgument("--disable-renderer-backgrounding");
            options.AddArgument("--enable-features=NetworkService,NetworkServiceInProcess");
            options.AddArgument("--force-color-profile=srgb");
            options.AddArgument("--hide-scrollbars");
            options.AddArgument("--metrics-recording-only");
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");

            //options.AddArgument("--window-size=1920,1080");

            options.AddAdditionalOption("browserless:token", _cdConfig.BrowserlessApiKey);
            options.AddAdditionalOption("browserless:timeout", 60000);//optional flag to set a different timeout value than the default
            options.AddAdditionalOption("browserless:stealth", true);//optional flag to enable Stealth
            options.AddAdditionalOption("browserless:blockAds", true);//optional flag to enable blockAds
            //options.AddAdditionalOption("browserless:trackingId", "AwENCwMHDQoHBgwKBAUODQ");//optional flag to enable trackingId
            options.AddAdditionalOption("browserless:pauseOnConnect", true);//optional flag to enable pauseOnConnect

            driver = new RemoteWebDriver(
              new Uri("https://chrome.browserless.io/webdriver"), options.ToCapabilities()
            );

            return driver;

            // Always call `quit` to ensure your session cleans up properly and you're not charged for unused time
            //driver.Quit();
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
