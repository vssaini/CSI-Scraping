using CSI.Common;
using CSI.Common.Config;
using CSI.Common.Wesco;
using CSI.WebScraping.Extensions;
using CSI.WebScraping.Services.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSI.WebScraping.Services.ScanSource;

public class ScanService
{
    private readonly ChromeService _chromeService;
    private readonly BackgroundWorker _bgWorker;
    private readonly ScanSourceConfig _ssConfig;

    public ScanService(BackgroundWorker bgWorker)
    {
        _chromeService = new ChromeService(bgWorker);
        _bgWorker = bgWorker;
        _ssConfig = ScanSourceConfig.GetInstance();

        if (_ssConfig.SaveScreenshots)
            CommonService.CreateDirectory(_ssConfig.ScreenshotDirectoryName);
    }

    public IEnumerable<Product> GetProducts(List<string> productIds)
    {
        using var driver = _chromeService.GetChromeDriver();
        var accService = new ScanAccountService(_bgWorker, driver);
        accService.Login();

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

            driver.SaveSearchScreenshot(_bgWorker, _ssConfig, productId);

            return LookForProductInSearchResult(driver, productId, counter);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
            _bgWorker.ReportProgress(0, $"Error occurred while searching the product '{productId}'. Error - {e.Message}");
        }

        return CommonService.ProductNotFound(productId, counter);
    }

    private void SendSearchCommand(WebDriver driver, string productId)
    {
        // Find search link and click it
        var nav = driver.FindElement(By.CssSelector(".site-header__navigation .utility-navigation .utility-navigation__wrap"));
        var liItems = nav.FindElements(By.TagName("li"));
        var listItem = liItems.FirstOrDefault(x => x.Text.Trim().Contains("Search"));
        var action = new Actions(driver);
        action.MoveToElement(listItem).Click().Build().Perform();

        // Now search for the product in search box shown
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        var searchField = wait.Until(x => x.FindElement(By.Id("search")));

        searchField.Clear();
        searchField.SendKeys(productId);
        searchField.SendKeys(Keys.Enter);

        driver.SaveScreenshot(_bgWorker, _ssConfig, $"AfterProduct-{productId}-SearchCommandSent");
    }

    private Product LookForProductInSearchResult(WebDriver driver, string productId, int counter)
    {
        try
        {
            var searchResultExist = SearchResultExist(driver);
            if (!searchResultExist)
                return CommonService.ProductNotFound(productId, counter);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            var productDtlDiv = wait.Until(x => x.FindElement(By.CssSelector(".search-result-list .product-detail-container")));

            var productExist = ProductExist(productDtlDiv, productId, ".product-content-header .field-manufactureritemnumber");

            if (productExist)
                return GetProductFromDetailDiv(productDtlDiv, productId, counter);

            _bgWorker.ReportProgress(0, $"Product '{productId}' not found.");
        }
        catch (NoSuchElementException e)
        {
            Log.Logger.Error(e, "No such element was found.");
            _bgWorker.ReportProgress(0, $"Div with class 'search-result-list' not found for the product '{productId}'.");
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
            _bgWorker.ReportProgress(0, $"An error occurred while searching for the product '{productId}'.");
        }

        return CommonService.ProductNotFound(productId, counter);
    }

    private static bool SearchResultExist(ISearchContext driver)
    {
        var searchResultDiv = driver.FindElements(By.CssSelector(".search-result-list"));
        return searchResultDiv.Any();
    }

    private static bool ProductExist(ISearchContext productDiv, string productId, string cssSelectorToFind)
    {
        // NOTE - Space after each class name is mandatory
        var productManufacturerNumbers = productDiv.FindElements(By.CssSelector(cssSelectorToFind));
        return productManufacturerNumbers.Any(p => p.Text.Trim().Contains(productId));
    }

    private Product GetProductFromDetailDiv(ISearchContext productDtlDiv, string productId, int counter)
    {
        var prodDtlDiv = productDtlDiv.FindElement(By.CssSelector(".product-content-header .product-detail-name"));
        var productName = prodDtlDiv.Text;

        _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

        var priceLbl = productDtlDiv.FindElement(By.CssSelector(".product-add-to-cart .field-msrp .prices .your-price"));
        var productPrice = priceLbl.Text;

        return new Product
        {
            Id = counter + 1,
            ProductId = productId,
            Status = Constants.StatusFound,
            Name = productName,
            Price = productPrice
        };
    }
}