using CSI.WebScraping.Models.Wesco;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        public IEnumerable<Product> GetProducts(List<string> productIds, BackgroundWorker bgWorker)
        {
            using var driver = GetChromeDriver(bgWorker);

            Login(driver, false, bgWorker);

            foreach (var productId in productIds)
            {
                yield return SearchProduct(driver, productId, false, bgWorker);
            }
        }

        private static ChromeDriver GetChromeDriver(BackgroundWorker bgWorker)
        {
            bgWorker.ReportProgress(0, "Initiating Chrome driver.");

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

        private static void Login(WebDriver driver, bool saveMilestoneScreenshots, BackgroundWorker bgWorker)
        {
            var loginUrl = ConfigurationManager.AppSettings["WescoLoginUrl"];
            var username = ConfigurationManager.AppSettings["WescoUsername"];
            var password = ConfigurationManager.AppSettings["WescoPassword"];

            bgWorker.ReportProgress(0, $"Logging in to Wesco on URL '{loginUrl}' with username '{username}'");

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

        private static Product SearchProduct(WebDriver driver, string productId, bool saveMilestoneScreenshots, BackgroundWorker bgWorker)
        {
            bgWorker.ReportProgress(0, $"Searching the product '{productId}'");

            var searchField = driver.FindElement(By.Id("search-desktop"));
            searchField.SendKeys(productId);
            searchField.SendKeys(Keys.Enter);

            WaitForMilliseconds(5000);

            if (saveMilestoneScreenshots)
            {
                var searchShot = driver.GetScreenshot();
                searchShot.SaveAsFile("Screenshot_Wesco_SearchResult.png");
            }

            // NOTE - Space after each class name is mandatory
            var productInfoAttrElements = driver.FindElements(By.CssSelector(".product-info .product-attributes .attribute-value"));
            var isProductFound = productInfoAttrElements.Any(p => p.Text.Contains(productId));

            var product = new Product { Id = productId, Found = isProductFound };

            if (isProductFound)
            {
                // TODO: Get product name too

                var priceSpan = driver.FindElement(By.CssSelector(".product-info .product-pricing .js-priceDisplay"));
                var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

                bgWorker.ReportProgress(0, $"Product with Id '{productId}' found. Price: {productPrice}");

                product.Price = productPrice;
            }

            return product;
        }

        private static void WaitForMilliseconds(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
