using CSI.Common.Config;
using CSI.Common.Wesco;
using CSI.WebScraping.Services.Chrome;
using CSI.WebScraping.Services.Wesco;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using OpenQA.Selenium;

namespace CSI.WebScraping.Services.ScanSource;

public class ScanService
{
    private readonly ChromeService _chromeService;
    private readonly BackgroundWorker _bgWorker;
    private readonly ScanSourceConfig _ssConfig;

    private readonly string _screenshotDirectoryPath;

    public ScanService(BackgroundWorker bgWorker)
    {
        _chromeService = new ChromeService(bgWorker);
        _bgWorker = bgWorker;
        _ssConfig = ScanSourceConfig.GetInstance();

        if (_ssConfig.SaveScreenshots)
            _screenshotDirectoryPath = CommonService.CreateDirectory(_ssConfig.ScreenshotDirectoryName);
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
        //try
        //{
        //    SendSearchCommand(driver, productId);
        //    SaveScreenshotIfRequested(driver, productId);

        //    return LookForProductInSearchResult(driver, productId, counter);
        //}
        //catch (Exception e)
        //{
        //    Log.Logger.Error(e, "An error occurred while searching for the product '{ProductId}'.", productId);
        //    _bgWorker.ReportProgress(0, $"Error occurred while searching the product '{productId}'. Error - {e.Message}");
        //}

        return CommonService.ProductNotFound(productId, counter);
    }
}