using System.Configuration;

namespace CSI.Common.Config
{
    public sealed class WescoConfig
    {
        public string LoginUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

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
                    LoginUrl = ConfigurationManager.AppSettings["WescoLoginUrl"],
                    Username = ConfigurationManager.AppSettings["WescoUsername"],
                    Password = ConfigurationManager.AppSettings["WescoPassword"]
                };
            }

            return _instance;
        }
    }
}
