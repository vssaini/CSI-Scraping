using CSI.Common;
using CSI.Common.Extensions;
using Serilog;
using System.IO;
using System.Reflection;

namespace CSI.WebScraping.Services;

public static class CommonService
{
    public static string CreateDirectory(string directoryName)
    {
        Log.Logger.Information("Creating directory.");

        var directoryPath = GetDirectoryPath(directoryName);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }

    public static string GetDirectoryPath(string directoryName)
    {
        var assemblyDirectory = Assembly.GetExecutingAssembly().DirectoryPath();
        return Path.Combine(assemblyDirectory, directoryName);
    }

    public static ProductDto ProductNotFound(string productId, int counter, string webAbbrv)
    {
        return new ProductDto
        {
            Id = $"{webAbbrv}{counter + 1}",
            ProductId = productId
        };
    }
}