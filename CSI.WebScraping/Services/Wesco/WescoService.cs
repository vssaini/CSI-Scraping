using CSI.Common;
using CSI.Common.Wesco;
using CSI.WebScraping.Services.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Log = Serilog.Log;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        private readonly ChromeService _chromeService;
        private readonly BackgroundWorker _bgWorker;

        public string ScreenshotsDirectoryPath { get; set; }

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
            try
            {
                SendSearchCommand(driver, productId);
                SaveScreenshotIfRequested(driver, productId);

                return LookForProductInSearchResult(driver, productId, counter);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"Error occurred while searching the product '{productId}'. Error - {e.Message}");
            }

            return ProductNotFound(productId, counter);
        }

        private static void SendSearchCommand(ISearchContext driver, string productId)
        {
            var searchField = driver.FindElement(By.Id("search-desktop"));
            searchField.Clear();
            searchField.SendKeys(productId);
            searchField.SendKeys(Keys.Enter);
        }

        private void SaveScreenshotIfRequested(ITakesScreenshot driver, string productId)
        {
            if (!_chromeService.SaveScreenshots) return;

            try
            {
                _bgWorker.ReportProgress(0, $"Saving the screenshot for the product '{productId}'.");

                var filePath = Path.Combine(ScreenshotsDirectoryPath, $"Wesco_{productId}_search_result_{DateTime.Now.ToString(Constants.DateFormat)}.png");
                var searchShot = driver.GetScreenshot();
                searchShot.SaveAsFile(filePath);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while saving the screenshot for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"Error occurred while saving the screenshot for the product '{productId}'. Error - {e.Message}");
            }
        }

        private Product LookForProductInSearchResult(ISearchContext driver, string productId, int counter)
        {
            try
            {
                var paginatedDivExist = PaginatedDivExist(driver);
                var product = paginatedDivExist
                    ? GetPaginatedProduct(driver, productId, counter)
                    : GetSingleProduct(driver, productId, counter);

                return product ?? ProductNotFound(productId, counter);
            }
            catch (NoSuchElementException e)
            {
                Log.Logger.Error(e, "No such element was found.");
                _bgWorker.ReportProgress(0, $"Div with class 'product-info' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"An error occurred while searching for the product '{productId}'.");
            }

            return ProductNotFound(productId, counter);
        }

        private static bool PaginatedDivExist(ISearchContext driver)
        {
            var searchResultDiv = driver.FindElements(By.CssSelector(".section-specific-results"));
            return searchResultDiv.Any();
        }

        private static Product ProductNotFound(string productId, int counter)
        {
            return new Product
            {
                Id = counter + 1,
                ProductId = productId
            };
        }

        #region Paginated product

        private Product GetPaginatedProduct(ISearchContext driver, string productId, int counter)
        {
            try
            {
                var productListDiv = driver.FindElement(By.CssSelector(".section-specific-results .tab-content .tab-container .productList"));
                var productExist = ProductExist(productListDiv, productId, ".accessible-list .product-tile-tertiary .product-tile .product-tile-item-details-info .label .value");

                if (productExist)
                    return GetProductFromPaginatedDiv(productListDiv, productId, counter);

                _bgWorker.ReportProgress(0, $"Product '{productId}' not found.");
            }
            catch (NoSuchElementException e)
            {
                Log.Logger.Error(e, "No such element was found.");
                _bgWorker.ReportProgress(0, $"Div with class 'productList' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the paginated product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"An error occurred while searching for the paginated product '{productId}'. Error - {e.Message}");
            }

            return null;
        }

        private Product GetProductFromPaginatedDiv(ISearchContext productListDiv, string productId, int counter)
        {
            var productName = productListDiv.FindElement(By.CssSelector(".product-tile .title-section .title-primary")).Text;

            _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

            var priceSpan = productListDiv.FindElement(By.CssSelector(".cart-item-price .listing-page-price .js-priceDisplay"));
            var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

            return new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = Constants.StatusFound,
                Name = productName,
                Price = productPrice
            };
        }

        #endregion

        #region Single product

        private Product GetSingleProduct(ISearchContext driver, string productId, int counter)
        {
            try
            {
                var productInfoDiv = driver.FindElement(By.CssSelector(".product-info"));
                var productExist = ProductExist(productInfoDiv, productId, ".row .col-sm-7 .product-attributes .attribute-value");

                if (productExist)
                    return GetProductFromInfoDiv(productInfoDiv, productId, counter);

                _bgWorker.ReportProgress(0, $"Product '{productId}' not found.");
            }
            catch (NoSuchElementException e)
            {
                Log.Logger.Error(e, "No such element was found.");
                _bgWorker.ReportProgress(0, $"Div with class 'product-info' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"An error occurred while searching for the product '{productId}'.");
            }

            return null;
        }

        private Product GetProductFromInfoDiv(ISearchContext productInfoDiv, string productId, int counter)
        {
            var spans = productInfoDiv.FindElements(By.TagName("span"));
            var productName = spans[1].Text;

            _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

            var priceSpan = productInfoDiv.FindElement(By.CssSelector(".product-pricing .price .js-priceDisplay"));
            var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

            return new Product
            {
                Id = counter + 1,
                ProductId = productId,
                Status = Constants.StatusFound,
                Name = productName,
                Price = productPrice
            };
        }

        #endregion

        private static bool ProductExist(ISearchContext productDiv, string productId, string cssSelectorToFind)
        {
            // NOTE - Space after each class name is mandatory
            var productInfoAttrElements = productDiv.FindElements(By.CssSelector(cssSelectorToFind));
            return productInfoAttrElements.Any(p => p.Text.Trim().Contains(productId));
        }
    }
}
