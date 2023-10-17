using System;
using System.Configuration;

namespace CSI.Common.Config
{
    public sealed class WescoConfig : BaseConfig
    {
        public string LoginUrl { get; set; }

        private WescoConfig() { }

        private static readonly object Lock = new();
        private static WescoConfig _instance;

        public static WescoConfig GetInstance()
        {
            if (_instance != null)
                return _instance;

            lock (Lock)
            {
                _instance ??= new WescoConfig
                {
                    LoginUrl = ConfigurationManager.AppSettings["Wesco:LoginUrl"],
                    Username = ConfigurationManager.AppSettings["Wesco:Username"],
                    Password = ConfigurationManager.AppSettings["Wesco:Password"],
                    SaveScreenshots = Convert.ToBoolean(ConfigurationManager.AppSettings["Wesco:SaveScreenshots"]),
                    ScreenshotDirectoryName = ConfigurationManager.AppSettings["Wesco:ScreenshotDirectoryName"],
                };
            }

            return _instance;
        }
    }
}
