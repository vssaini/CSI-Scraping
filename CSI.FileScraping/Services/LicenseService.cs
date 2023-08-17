namespace CSI.FileScraping.Services
{
    internal class LicenseService
    {
        private const string LicenseFileName = "Aspose.License.lic";

        public void SetLicense()
        {
            SetCellsLicense();
        }

        private static void SetCellsLicense()
        {
            var license = new Aspose.Cells.License();
            license.SetLicense(LicenseFileName);
        }
    }
}
