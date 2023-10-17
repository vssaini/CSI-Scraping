namespace CSI.Common.Config;

public abstract class BaseConfig
{
    public string Username { get; set; }
    public string Password { get; set; }

    public bool SaveScreenshots { get; set; }
    public string ScreenshotDirectoryName { get; set; }
}