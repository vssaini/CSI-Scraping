using System;
using System.Configuration;

namespace CSI.Common.Config
{
    public sealed class BHConfig : BaseConfig
    {
        public string HomeUrl { get; set; }

        private BHConfig() { }

        private static readonly object Lock = new();
        private static BHConfig _instance;

        public static BHConfig GetInstance()
        {
            if (_instance != null)
                return _instance;

            lock (Lock)
            {
                _instance ??= new BHConfig
                {

                    HomeUrl = ConfigurationManager.AppSettings["BHPhotoVideo:HomeUrl"],
                    Username = ConfigurationManager.AppSettings["BHPhotoVideo:Username"],
                    Password = ConfigurationManager.AppSettings["BHPhotoVideo:Password"],
                    SaveScreenshots = Convert.ToBoolean(ConfigurationManager.AppSettings["BHPhotoVideo:SaveScreenshots"]),
                    ScreenshotDirectoryName = ConfigurationManager.AppSettings["BHPhotoVideo:ScreenshotDirectoryName"],
                };
            }

            return _instance;
        }
    }
}
