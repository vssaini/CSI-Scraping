﻿using System;
using System.Configuration;

namespace CSI.Common.Config
{
    public sealed class ScanSourceConfig : BaseConfig
    {
        public string HomeUrl { get; set; }

        private ScanSourceConfig() { }

        private static readonly object Lock = new();
        private static ScanSourceConfig _instance;

        public static ScanSourceConfig GetInstance()
        {
            if (_instance != null)
                return _instance;

            lock (Lock)
            {
                _instance ??= new ScanSourceConfig
                {

                    HomeUrl = ConfigurationManager.AppSettings["ScanSource:HomeUrl"],
                    Username = ConfigurationManager.AppSettings["ScanSource:Username"],
                    Password = ConfigurationManager.AppSettings["ScanSource:Password"],
                    SaveScreenshots = Convert.ToBoolean(ConfigurationManager.AppSettings["ScanSource:SaveScreenshots"]),
                    ScreenshotDirectoryName = ConfigurationManager.AppSettings["ScanSource:ScreenshotDirectoryName"],
                };
            }

            return _instance;
        }
    }
}
