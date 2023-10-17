using CSI.Common.Extensions;
using CSI.Common.Wesco;
using Serilog;
using System.IO;
using System.Reflection;

namespace CSI.WebScraping.Services;

public static class CommonService
{
    public static string CreateDirectory(string directoryName)
    {
        Log.Logger.Information("Creating directory.");

        var assemblyDirectory = Assembly.GetExecutingAssembly().DirectoryPath();
        var directoryPath = Path.Combine(assemblyDirectory, directoryName);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }

    public static Product ProductNotFound(string productId, int counter)
    {
        return new Product
        {
            Id = counter + 1,
            ProductId = productId
        };
    }
}