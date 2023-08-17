using CSI.Common;
using CSI.Common.Wesco;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        private readonly ChromeService _chromeService;
        private readonly BackgroundWorker _bgWorker;

        public WescoService(BackgroundWorker bgWorker)
        {
            _chromeService = new ChromeService(bgWorker);
            _bgWorker = bgWorker;
        }

        public IEnumerable<Product> GetProducts(List<string> productIds)
        {
            using var driver = _chromeService.GetChromeDriver();
            _chromeService.Login(driver);

            var startTime = DateTime.Now;
            _bgWorker.ReportProgress(0, $"Searching of products started at {startTime:T}.");

            for (var i = 0; i < productIds.Count; i++)
            {
                var productId = productIds[i];
                _bgWorker.ReportProgress(0, $"{i + 1}/{productIds.Count} - Searching the product '{productId}'");

                yield return SearchProduct(driver, productId, i);
            }

            var dateDiff = DateTime.Now - startTime;
            _bgWorker.ReportProgress(0, $"Searching of products completed at {DateTime.Now:T} (within {dateDiff.Minutes} minutes).");
        }

        private Product SearchProduct(WebDriver driver, string productId, int counter)
        {
            var searchField = driver.FindElement(By.Id("search-desktop"));
            searchField.SendKeys(productId);
            searchField.SendKeys(Keys.Enter);

            Thread.Sleep(_chromeService.SleepMilliSeconds);

            if (_chromeService.SaveMilestoneScreenshots)
            {
                var searchShot = driver.GetScreenshot();
                searchShot.SaveAsFile("Screenshot_Wesco_SearchResult.png");
            }

            // NOTE - Space after each class name is mandatory
            var productInfoAttrElements = driver.FindElements(By.CssSelector(".product-info .product-attributes .attribute-value"));
            var isProductFound = productInfoAttrElements.Any(p => p.Text.Contains(productId));

            var product = new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = isProductFound ? Constants.StatusFound : Constants.StatusNotFound
            };

            if (isProductFound)
            {
                var productInfo = driver.FindElement(By.CssSelector(".product-info"));

                var productName = productInfo.FindElement(By.CssSelector(".inner-product-heading")).Text;
                var priceSpan = productInfo.FindElement(By.CssSelector(".product-pricing .js-priceDisplay"));
                var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

                _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

                product.Name = productName;
                product.Price = productPrice;
            }
            else
            {
                _bgWorker.ReportProgress(0, $"Product '{productId}' not found.");
            }

            return product;
        }
    }
}
