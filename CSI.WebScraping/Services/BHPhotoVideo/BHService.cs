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

namespace CSI.WebScraping.Services.BHPhotoVideo;

public class BHService
{
    private readonly ChromeService _chromeService;
    private readonly BackgroundWorker _bgWorker;
    private readonly BHConfig _bhConfig;

    public BHService(BackgroundWorker bgWorker)
    {
        _chromeService = new ChromeService(bgWorker);
        _bgWorker = bgWorker;
        _bhConfig = BHConfig.GetInstance();

        if (_bhConfig.SaveScreenshots)
            CommonService.CreateDirectory(_bhConfig.ScreenshotDirectoryName);
    }

    public IEnumerable<Product> GetProducts(List<string> productIds)
    {
        using var driver = _chromeService.GetChromeDriver();
        OpenWebsite(driver);

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

    private void OpenWebsite(WebDriver driver)
    {
        _bgWorker.ReportProgress(0, $"Navigating to URL {_bhConfig.HomeUrl}");
        driver.Navigate().GoToUrl(_bhConfig.HomeUrl);

        //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        //var signInLink = wait.Until(d => d.FindElement(By.CssSelector(".searchAndAccount_DEpV0ZOp7G .searchContainer_dDF1464xPi .searchInput_dDF1464xPi")));

        //var action = new Actions(driver);
        //action.MoveToElement(signInLink).Click().Build().Perform();
    }

    private Product SearchProduct(WebDriver driver, string productId, int counter)
    {
        try
        {
            SendSearchCommand(driver, productId);

            driver.SaveSearchScreenshot(_bgWorker, _bhConfig, productId);

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
        var searchField = driver.FindElement(By.CssSelector(".searchAndAccount_DEpV0ZOp7G .searchContainer_dDF1464xPi .searchInput_dDF1464xPi"));
        searchField.Clear();
        searchField.SendKeys(productId);
        searchField.SendKeys(Keys.Enter);

        driver.SaveScreenshot(_bgWorker, _bhConfig, $"AfterProduct-{productId}-SearchCommandSent");
    }

    private Product LookForProductInSearchResult(WebDriver driver, string productId, int counter)
    {
        try
        {
            var productNotFound = IsProductNotFound(driver);
            if (!productNotFound)
                return CommonService.ProductNotFound(productId, counter);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            var searchResultsDiv = wait.Until(x => x.FindElement(By.XPath("//*[@data-selenium='listingProductDetailSection']")));

            var productDiv = searchResultsDiv.FindElement(By.XPath("//*[@data-selenium='miniProductPage']"));

            var productExist = ProductExist(productDiv, productId, "//*[@data-selenium='miniProductPageProductSkuInfo']");

            if (productExist)
                return GetProductFromMiniPageDiv(productDiv, productId, counter);

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

    private static bool IsProductNotFound(ISearchContext driver)
    {
        //var infoMessages = driver.FindElements(By.XPath(".topSection_OOvCaqUpUN message_jvmnw0TV7p"));
        var infoMessages = driver.FindElements(By.XPath("//*[@data-selenium='correctedSearchTermMsg']"));
        return infoMessages.Any(i => i.Text.Contains("We did not find any matches for"));
    }

    private static bool ProductExist(ISearchContext productDiv, string productId, string xPathToFind)
    {
        var prodSkuNumbers = productDiv.FindElements(By.XPath(xPathToFind));
        return prodSkuNumbers.Any(p => p.Text.Trim().Contains(productId));
    }

    private Product GetProductFromMiniPageDiv(ISearchContext productDiv, string productId, int counter)
    {
        var prodDtlDiv = productDiv.FindElement(By.XPath("//*[@data-selenium='miniProductPageProductName']"));
        var productName = prodDtlDiv.Text;

        _bgWorker.ReportProgress(0, $"Product '{productId}' found. Name - {productName}");

        var priceLbl = productDiv.FindElement(By.XPath("//*[@data-selenium='uppedDecimalPriceFirst']"));
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