using System;
using System.Configuration;

namespace CSI.Common.Config
{
    public sealed class AdiConfig : BaseConfig
    {
        public string HomeUrl { get; set; }

        private AdiConfig() { }

        private static readonly object Lock = new();
        private static AdiConfig _instance;

        public static AdiConfig GetInstance()
        {
            if (_instance != null)
                return _instance;

            lock (Lock)
            {
                _instance ??= new AdiConfig
                {

                    HomeUrl = ConfigurationManager.AppSettings["AdiGlobal:HomeUrl"],
                    Username = ConfigurationManager.AppSettings["AdiGlobal:Username"],
                    Password = ConfigurationManager.AppSettings["AdiGlobal:Password"],
                    SaveScreenshots = Convert.ToBoolean(ConfigurationManager.AppSettings["AdiGlobal:SaveScreenshots"]),
                    ScreenshotDirectoryName = ConfigurationManager.AppSettings["AdiGlobal:ScreenshotDirectoryName"],
                };
            }

            return _instance;
        }
    }
}
