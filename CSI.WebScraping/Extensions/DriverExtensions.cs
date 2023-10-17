using CSI.Common;
using CSI.Common.Config;
using CSI.WebScraping.Services;
using OpenQA.Selenium;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;

namespace CSI.WebScraping.Extensions;

public static class DriverExtensions
{
    /// <summary>
    /// Save login screenshot if <see cref="BaseConfig.SaveScreenshots"/> is true.
    /// </summary>
    public static void SaveScreenshot(this ITakesScreenshot driver, BackgroundWorker bgWorker, BaseConfig config, string feature)
    {
        if (!config.SaveScreenshots)
            return;

        var website = GetWebsiteName(config);
        bgWorker.ReportProgress(0, $"Saving the screenshot for '{website}' {feature}.");

        var dirPath = CommonService.GetDirectoryPath(config.ScreenshotDirectoryName);

        var filePath = Path.Combine(dirPath, $"{website}-{feature}-{DateTime.Now.ToString(Constants.DateFormat)}.png");
        var screenshot = driver.GetScreenshot();
        screenshot.SaveAsFile(filePath);
    }

    private static string GetWebsiteName(BaseConfig config)
    {
        var websiteName = "Wesco";

        if (config is ScanSourceConfig)
            websiteName = "ScanSource";

        return websiteName;
    }


    /// <summary>
    /// Save search screenshot if <see cref="BaseConfig.SaveScreenshots"/> is true.
    /// </summary>
    public static void SaveSearchScreenshot(this ITakesScreenshot driver, BackgroundWorker bgWorker, BaseConfig config, string productId)
    {
        if (!config.SaveScreenshots) 
            return;

        try
        {
            bgWorker.ReportProgress(0, $"Saving the screenshot for the product '{productId}'.");

            var website = GetWebsiteName(config);
            var dirPath = CommonService.GetDirectoryPath(config.ScreenshotDirectoryName);

            var filePath = Path.Combine(dirPath, $"{website}_{productId}_search_result_{DateTime.Now.ToString(Constants.DateFormat)}.png");
            var searchShot = driver.GetScreenshot();
            searchShot.SaveAsFile(filePath);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while saving the screenshot for the product '{ProductId}'.", productId);
            bgWorker.ReportProgress(0, $"Error occurred while saving the screenshot for the product '{productId}'. Error - {e.Message}");
        }
    }
}