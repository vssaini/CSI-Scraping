namespace CSI.Common
{
    public class Constants
    {
        public const string StatusFound = "Found";
        public const string StatusNotFound = "Not Found";

        public const string DateFormat = "yyyy-MM-dd_hhmmss";

        public class Website
        {
            public const string AdiGlobal = "ADI Global";
            public const string Wesco = "Wesco";
            public const string ScanSource = "Scan Source";
            public const string BHPhotoVideo = "B&H Photo Video";
        }

        public class System
        {
            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        }
    }
}
