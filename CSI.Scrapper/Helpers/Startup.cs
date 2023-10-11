using CSI.Common.Extensions;
using Serilog;
using System.IO;
using System.Reflection;

namespace CSI.Scrapper.Helpers;

public static class Startup
{
    public static void ConfigureLogger()
    {
        var logFilePath = GetLogFilePath();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    private static string GetLogFilePath()
    {
        var logDirPath = CreateLogDirectory();

        if (!Directory.Exists(logDirPath))
            Directory.CreateDirectory(logDirPath);

        return Path.Combine(logDirPath, "log-.txt");
    }

    private static string CreateLogDirectory()
    {
        var dirPath = Assembly.GetExecutingAssembly().DirectoryPath();
        var logDirPath = Path.Combine(dirPath, "Logs");

        if (!Directory.Exists(logDirPath))
            Directory.CreateDirectory(logDirPath);

        return logDirPath;
    }
}