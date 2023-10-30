using CSI.Common;
using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using CSI.WebScraping.Services.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Log = Serilog.Log;

namespace CSI.WebScraping.Services.Wesco
{
    public class WescoService
    {
        private readonly ChromeService _chromeService;
        private readonly BackgroundWorker _bgWorker;
        private readonly WescoConfig _wesConfig;

        private const string WebAbbrv = "W";

        public WescoService(BackgroundWorker bgWorker)
        {
            _chromeService = new ChromeService(bgWorker);
            _bgWorker = bgWorker;
            _wesConfig = WescoConfig.GetInstance();

            if (_wesConfig.SaveScreenshots)
                CommonService.CreateDirectory(_wesConfig.ScreenshotDirectoryName);
        }

        public IEnumerable<ProductDto> GetProducts(List<string> productIds)
        {
            using var driver = _chromeService.GetChromeDriver(Constants.Website.Wesco);
            var accService = new WescoAccountService(_bgWorker, driver);
            accService.Login();

            var startTime = DateTime.Now;
            _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Searching of products started at {startTime:T}.");

            for (var i = 0; i < productIds.Count; i++)
            {
                var productId = productIds[i];
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - {i + 1}/{productIds.Count} - Searching the product '{productId}'");

                yield return SearchProduct(driver, productId, i);
            }

            var dateDiff = DateTime.Now - startTime;
            _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Searching of products completed at {DateTime.Now:T} (within {dateDiff.Minutes} minutes).");
        }

        private ProductDto SearchProduct(WebDriver driver, string productId, int counter)
        {
            try
            {
                SendSearchCommand(driver, productId);
                driver.SaveSearchScreenshot(_bgWorker, _wesConfig, productId);

                return LookForProductInSearchResult(driver, productId, counter);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Error occurred while searching the product '{productId}'. Error - {e.Message}");
            }

            return CommonService.ProductNotFound(productId, counter, WebAbbrv);
        }

        private static void SendSearchCommand(ISearchContext driver, string productId)
        {
            var searchField = driver.FindElement(By.Id("search-desktop"));
            searchField.Clear();
            searchField.SendKeys(productId);
            searchField.SendKeys(Keys.Enter);
        }

        private ProductDto LookForProductInSearchResult(ISearchContext driver, string productId, int counter)
        {
            try
            {
                var paginatedDivExist = PaginatedDivExist(driver);
                var product = paginatedDivExist
                    ? GetPaginatedProduct(driver, productId, counter)
                    : GetSingleProduct(driver, productId, counter);

                return product ?? CommonService.ProductNotFound(productId, counter, WebAbbrv);
            }
            catch (NoSuchElementException e)
            {
                Log.Logger.Error(e, "No such element was found.");
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Div with class 'product-info' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - An error occurred while searching for the product '{productId}'.");
            }

            return CommonService.ProductNotFound(productId, counter, WebAbbrv);
        }

        private static bool PaginatedDivExist(ISearchContext driver)
        {
            var searchResultDiv = driver.FindElements(By.CssSelector(".section-specific-results"));
            return searchResultDiv.Any();
        }

        #region Paginated product

        private ProductDto GetPaginatedProduct(ISearchContext driver, string productId, int counter)
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
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Div with class 'productList' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the paginated product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - An error occurred while searching for the paginated product '{productId}'. Error - {e.Message}");
            }

            return null;
        }

        private ProductDto GetProductFromPaginatedDiv(ISearchContext productListDiv, string productId, int counter)
        {
            var productName = productListDiv.FindElement(By.CssSelector(".product-tile .title-section .title-primary")).Text;

            _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Product '{productId}' found. Name - {productName}");

            var priceSpan = productListDiv.FindElement(By.CssSelector(".cart-item-price .listing-page-price .js-priceDisplay"));
            var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

            var stockDetail = productListDiv.FindElement(By.CssSelector(".cart-item-price .manufacturer-note .stock-detail"));
            var stockValue = Regex.Match(stockDetail.Text, @"\d+").Value;

            return new ProductDto
            {
                Id = $"{WebAbbrv}{counter + 1}",
                ProductId = productId,
                Status = Constants.StatusFound,
                Name = productName,
                Price = productPrice.ToDecimal(),
                Stock = stockValue.ToInt()
            };
        }

        #endregion

        #region Single product

        private ProductDto GetSingleProduct(ISearchContext driver, string productId, int counter)
        {
            try
            {
                var productInfoDiv = driver.FindElement(By.CssSelector(".product-info"));
                var productExist = ProductExist(productInfoDiv, productId, ".row .col-sm-7 .product-attributes .attribute-value");

                if (productExist)
                    return GetProductFromInfoDiv(productInfoDiv, productId, counter);

                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Product '{productId}' not found.");
            }
            catch (NoSuchElementException e)
            {
                Log.Logger.Error(e, "No such element was found.");
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Div with class 'product-info' not found for the product '{productId}'.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
                _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - An error occurred while searching for the product '{productId}'.");
            }

            return null;
        }

        private ProductDto GetProductFromInfoDiv(ISearchContext productInfoDiv, string productId, int counter)
        {
            var spans = productInfoDiv.FindElements(By.TagName("span"));
            var productName = spans[1].Text;

            _bgWorker.ReportProgress(0, $"{Constants.Website.Wesco} - Product '{productId}' found. Name - {productName}");

            var priceSpan = productInfoDiv.FindElement(By.CssSelector(".product-pricing .price .js-priceDisplay"));
            var productPrice = priceSpan.GetAttribute("data-formatted-price-value");

            var stockDetail = productInfoDiv.FindElement(By.CssSelector(".product-shipping .manufacturer-note .stock-detail"));
            var stockValue = Regex.Match(stockDetail.Text, @"\d+").Value;

            return new ProductDto
            {
                Id = $"{WebAbbrv}{counter + 1}",
                ProductId = productId,
                Status = Constants.StatusFound,
                Name = productName,
                Price = productPrice.ToDecimal(),
                Stock = stockValue.ToInt(),
                Source = Constants.Website.Wesco
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
