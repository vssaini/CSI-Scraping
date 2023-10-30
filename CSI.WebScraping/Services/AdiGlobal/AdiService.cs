using CSI.Common;
using CSI.Common.Config;
using CSI.WebScraping.Extensions;
using CSI.WebScraping.Services.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSI.WebScraping.Services.AdiGlobal;

public class AdiService
{
    private readonly ChromeService _chromeService;
    private readonly BackgroundWorker _bgWorker;
    private readonly AdiConfig _adiConfig;

    private const string WebAbbrv = "ADI";

    public AdiService(BackgroundWorker bgWorker)
    {
        _chromeService = new ChromeService(bgWorker);
        _bgWorker = bgWorker;
        _adiConfig = AdiConfig.GetInstance();

        if (_adiConfig.SaveScreenshots)
            CommonService.CreateDirectory(_adiConfig.ScreenshotDirectoryName);
    }

    public IEnumerable<ProductDto> GetProducts(List<string> productIds)
    {
        using var driver = _chromeService.GetChromeDriver(Constants.Website.AdiGlobal);

        var accService = new AdiAccountService(_bgWorker, driver);
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

    private ProductDto SearchProduct(WebDriver driver, string productId, int counter)
    {
        try
        {
            SendSearchCommand(driver, productId);

            driver.SaveSearchScreenshot(_bgWorker, _adiConfig, productId);

            return LookForProductInSearchResult(driver, productId, counter);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
            _bgWorker.ReportProgress(0, $"Error occurred while searching the product '{productId}'. Error - {e.Message}");
        }

        return CommonService.ProductNotFound(productId, counter, WebAbbrv, Constants.Website.AdiGlobal);
    }

    private void SendSearchCommand(WebDriver driver, string productId)
    {
        // Find search link and click it
        //var nav = driver.FindElement(By.CssSelector(".bottom-nav .rd-nav-header .bottom-nav-productMenu .search-section .search-wrapper .search-input"));
        //var liItems = nav.FindElements(By.TagName("li"));
        //var listItem = liItems.FirstOrDefault(x => x.Text.Trim().Contains("Search"));
        //var action = new Actions(driver);
        //action.MoveToElement(listItem).Click().Build().Perform();

        // Now search for the product in search box shown
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        var searchFieldSpan = wait.Until(x => x.FindElement(By.CssSelector(".search-section .search-wrapper .search-input")));
        var searchField = searchFieldSpan.FindElement(By.Id("widesearchbox"));

        driver.SaveScreenshot(_bgWorker, _adiConfig, $"BeforeProduct-{productId}-SearchCommandSent");

        searchField.Clear();
        searchField.SendKeys(productId);
        searchField.SendKeys(Keys.Enter);

        driver.SaveScreenshot(_bgWorker, _adiConfig, $"AfterProduct-{productId}-SearchCommandSent");
    }

    private ProductDto LookForProductInSearchResult(WebDriver driver, string productId, int counter)
    {
        //return CommonService.ProductNotFound(productId, counter);

        try
        {
            var searchResultExist = SearchResultExist(driver);
            if (!searchResultExist)
                return CommonService.ProductNotFound(productId, counter, WebAbbrv, Constants.Website.AdiGlobal);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            var productContainer = wait.Until(x => x.FindElement(By.CssSelector(".rd-productlistgrid .rd-item-list .isc-productContainer")));

            var productExist = ProductExist(productContainer, productId, ".rd-itemcode-sku .item-num-sku");

            if (productExist)
                return GetProductFromContainer(productContainer, productId, counter);

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

        return CommonService.ProductNotFound(productId, counter, WebAbbrv, Constants.Website.AdiGlobal);
    }

    private static bool SearchResultExist(ISearchContext driver)
    {
        var searchResultDiv = driver.FindElements(By.CssSelector(".rd-productlistgrid"));
        return searchResultDiv.Any();
    }

    private static bool ProductExist(ISearchContext productDiv, string productId, string cssSelectorToFind)
    {
        // NOTE - Space after each class name is mandatory
        var productManufacturerNumbers = productDiv.FindElements(By.CssSelector(cssSelectorToFind));
        return productManufacturerNumbers.Any(p => p.Text.Trim().Contains(productId));
    }

    private ProductDto GetProductFromContainer(ISearchContext productContainer, string productId, int counter)
    {
        var prodSpan = productContainer.FindElement(By.CssSelector(".rd-item-name .rd-item-name-desc"));
        var productName = prodSpan.Text;

        _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

        //var priceLbl = productContainer.FindElement(By.CssSelector(".product-add-to-cart .field-msrp .prices .your-price"));
        var productPrice = 0; //priceLbl.Text;

        return new ProductDto
        {
            Id = $"{WebAbbrv}{counter + 1}",
            ProductId = productId,
            Status = Constants.StatusFound,
            Name = productName,
            Price = productPrice
        };
    }
}