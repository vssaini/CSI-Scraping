using CSI.Common;
using CSI.Common.Wesco;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        private readonly ChromeService _chromeService;
        private readonly BackgroundWorker _bgWorker;

        public string ScreenShotsDirectoryPath { get; set; }

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
            var product = new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = Constants.StatusNotFound
            };

            try
            {
                var searchField = driver.FindElement(By.Id("search-desktop"));
                searchField.Clear();
                searchField.SendKeys(productId);
                searchField.SendKeys(Keys.Enter);

                SaveScreenShotIfRequested(driver, productId);

                return GetProduct(driver, productId, counter);
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, $"Error occurred while searching the product '{productId}'. Error - {e.Message}");
            }

            return product;
        }

        private void SaveScreenShotIfRequested(WebDriver driver, string productId)
        {
            if (!_chromeService.SaveMilestoneScreenshots) return;

            try
            {
                _bgWorker.ReportProgress(0, $"Saving the screenshot for the product '{productId}'.");

                var filePath = Path.Combine(ScreenShotsDirectoryPath, $"Wesco_{productId}_search_result_{DateTime.Now:yyyy-MM-dd_hhmmss}.png");
                var searchShot = driver.GetScreenshot();
                searchShot.SaveAsFile(filePath);
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, $"Error occurred while saving the screenshot for the product '{productId}'. Error - {e.Message}");
            }
        }

        private Product GetProduct(WebDriver driver, string productId, int counter)
        {
            try
            {
                return GetSingleProduct(driver, productId, counter);
            }
            catch (NoSuchElementException)
            {
                _bgWorker.ReportProgress(0, $"Div with class 'product-info' not found for the product '{productId}'.");
                return new Product
                {
                    Id = counter + 1,
                    ProductId = productId,
                    Status = Constants.StatusNotFound
                };

                //return GetPaginatedProduct(driver, productId, counter);
            }
        }

        private Product GetSingleProduct(WebDriver driver, string productId, int counter)
        {
            var product = new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = Constants.StatusNotFound
            };

            var productInfoDiv = driver.FindElement(By.CssSelector(".product-info"));
            var productExist = IsProductExist(productInfoDiv, productId, ".product-attributes .attribute-value");

            if (productExist)
            {
                product.Status = Constants.StatusFound;

                var spans = productInfoDiv.FindElements(By.TagName("span"));
                var productName = spans[1].Text;

                var priceSpan = productInfoDiv.FindElement(By.CssSelector(".product-pricing .price .js-priceDisplay"));
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

        private static bool IsProductExist(IWebElement productDiv, string productId, string cssSelectorToFind)
        {
            // NOTE - Space after each class name is mandatory
            var productInfoAttrElements = productDiv.FindElements(By.CssSelector(cssSelectorToFind));
            return productInfoAttrElements.Any(p => p.Text.Contains(productId));
        }

        private Product GetPaginatedProduct(WebDriver driver, string productId, int counter)
        {
            _bgWorker.ReportProgress(0, $"Now attempting to search product '{productId}' in paged element.");

            var product = new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = Constants.StatusNotFound
            };

            try
            {
                var productListDiv = driver.FindElement(By.CssSelector(".productList"));
                var productExist = IsProductExist(productListDiv, productId, ".product-tile .product-tile-item-details-info .label .value");

                if (productExist)
                {
                    product.Status = Constants.StatusFound;

                    var productName = productListDiv.FindElement(By.CssSelector(".product-tile .title-section .title-primary")).Text;

                    var priceSpan = productListDiv.FindElement(By.CssSelector(".cart-item-price .data-price .js-priceDisplay"));
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
            catch (NoSuchElementException)
            {
                _bgWorker.ReportProgress(0, $"Div with class 'productList' not found for the product '{productId}'.");
                return product;
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, $"An error occurred while searching for the paginated product '{productId}'. Error - {e.Message}");
                return product;
            }
        }
    }
}
