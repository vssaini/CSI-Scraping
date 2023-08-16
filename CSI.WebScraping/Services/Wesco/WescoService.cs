using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using CSI.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        public static void SearchProductPrice(bool saveMilestoneScreenshots = false)
        {
            var searchTerm = GetWescoProductSearchTerm();

            Logger.LogWithDoubleLines($"Scraping products price from Wesco started at {DateTime.Now:HH:mm:ss}.");

            using var driver = GetChromeDriver();

            Login(driver, saveMilestoneScreenshots);

            var productIds = searchTerm.Split(',').Select(x => x.Trim()).ToList();
            foreach (var productId in productIds)
            {
                SearchProduct(driver, productId, saveMilestoneScreenshots);
            }

            Logger.LogWithDoubleLines($"Scraping products price from Wesco completed at {DateTime.Now:HH:mm:ss}.");
        }

        private static ChromeDriver GetChromeDriver()
        {
            Logger.LogInfo("Initiating Chrome driver.");

            // to open Chrome in headless mode 
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("headless");
            chromeOptions.AddArgument("ignore-certificate-errors");

            // Disable writing of unnecessary logs
            // Valid levels are INFO = 0, WARNING = 1, LOG_ERROR = 2, LOG_FATAL = 3
            chromeOptions.AddArgument("log-level=3");

            const string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            chromeOptions.AddArgument($"user-agent={userAgent}");

            return new ChromeDriver(chromeOptions);
        }

        private static string GetWescoProductSearchTerm()
        {
            var searchTerm = ConfigurationManager.AppSettings["WescoProductSearchTerm"];
            return searchTerm ?? throw new Exception("WescoProductSearchTerm is not defined in App.config");
        }

        private static void Login(WebDriver driver, bool saveMilestoneScreenshots)
        {
            var loginUrl = ConfigurationManager.AppSettings["WescoLoginUrl"];
            var username = ConfigurationManager.AppSettings["WescoUsername"];
            var password = ConfigurationManager.AppSettings["WescoPassword"];

            Logger.LogInfo($"Logging in to Wesco on URL '{loginUrl}' with username '{username}'");

            driver.Navigate().GoToUrl(loginUrl);

            driver.FindElement(By.Id("j_username")).SendKeys(username);
            driver.FindElement(By.Id("j_password")).SendKeys(password);

            if (saveMilestoneScreenshots)
            {
                var snapshot = driver.GetScreenshot();
                snapshot.SaveAsFile("Screenshot_Wesco_Login.png");
            }

            driver.FindElement(By.CssSelector("button.button")).Click();

            WaitForMilliseconds(5000);
        }

        private static void SearchProduct(WebDriver driver, string searchTerm, bool saveMilestoneScreenshots)
        {
            Logger.LogInfo($"Searching the product '{searchTerm}'");

            var searchField = driver.FindElement(By.Id("search-desktop"));
            searchField.SendKeys(searchTerm);
            searchField.SendKeys(Keys.Enter);

            WaitForMilliseconds(5000);

            if (saveMilestoneScreenshots)
            {
                var searchShot = driver.GetScreenshot();
                searchShot.SaveAsFile("Screenshot_Wesco_SearchResult.png");
            }

            // NOTE - Space after each class name is mandatory
            var productInfoAttrElements = driver.FindElements(By.CssSelector(".product-info .product-attributes .attribute-value"));
            var isProductFound = productInfoAttrElements.Any(p => p.Text.Contains(searchTerm));

            if (isProductFound)
            {
                var priceSpan = driver.FindElement(By.CssSelector(".product-info .product-pricing .js-priceDisplay"));
                var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

                Logger.LogSuccess($"Product with search term '{searchTerm}' found. Price: {productPrice}");
            }
            else
            {
                Logger.LogWarning($"Product with search term '{searchTerm}' not found.");
            }
        }

        private static void WaitForMilliseconds(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
