namespace CSI.FileScraping.Services
{
    internal class LicenseService
    {
        private readonly string _licenseFileName = "Aspose.License.lic";

        public void SetLicense()
        {
            SetPdfLicense();
            SetCellsLicense();
        }

        private void SetPdfLicense()
        {
            var license = new Aspose.Pdf.License();
            license.SetLicense(_licenseFileName);
        }
        private void SetCellsLicense()
        {
            var license = new Aspose.Cells.License();
            license.SetLicense(_licenseFileName);
        }
    }
}
