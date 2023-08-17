using CSI.Common.Wesco;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace CSI.FileScraping.Services
{
    public class FileService
    {
        private readonly BackgroundWorker _bgWorker;

        public FileService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
        }

        public List<Product> GetProducts(string pdfFilePath)
        {
            var products = new List<Product> { new Product { Id = "PS-114HA", Name = "Complete Power Supply", Price = "1,660.00" } };

            ReadPdfAndGenerateExcelFile(pdfFilePath);

            // TODO: Prepare list of products by reading excel file

            return products;
        }

        private void ReadPdfAndGenerateExcelFile(string pdfFilePath)
        {
            var dirPath = GetDocsDirectoryPath();
            var fileName = Path.GetFileNameWithoutExtension(pdfFilePath);

            var srcExcelFilePath = $"{dirPath}\\{fileName}_src.xlsx";
            var destExcelFilePath = $"{dirPath}\\{fileName}_dest.xlsx";

            var sautinFileService = new SautinFileService(_bgWorker);
            sautinFileService.CreateExcelFromTableInPdf(pdfFilePath, srcExcelFilePath);

            var asExcelService = new AsposeExcelService(_bgWorker);
            asExcelService.PrepareExcelFile(srcExcelFilePath, destExcelFilePath);

            DeleteTempExcelFile(srcExcelFilePath);
            OpenExcelFile(destExcelFilePath);
        }

        private static string GetDocsDirectoryPath()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dirPath = $"{assemblyPath}\\Docs";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        private void DeleteTempExcelFile(string tempExcelPath)
        {
            _bgWorker.ReportProgress(0, $"Deleting temporary excel file at {tempExcelPath}");
            File.Delete(tempExcelPath);
        }

        private void OpenExcelFile(string pathToExcel)
        {
            _bgWorker.ReportProgress(0, $"Opening excel file at {pathToExcel}");
            System.Diagnostics.Process.Start(pathToExcel);
        }
    }
}
