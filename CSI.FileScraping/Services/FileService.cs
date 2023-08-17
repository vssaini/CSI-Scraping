using CSI.Common;
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

        public void ReadPdfAndGenerateExcelFile(string pdfFilePath)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dirPath = $"{assemblyPath}\\Docs";
            const string fileName = "Securitech";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            var srcExcelFilePath = $"{dirPath}\\{fileName}_src.xlsx";
            var destExcelFilePath = $"{dirPath}\\{fileName}_dest.xlsx";

            var sautinFileService = new SautinFileService(_bgWorker);
            sautinFileService.CreateExcelFromTableInPdf(pdfFilePath, srcExcelFilePath);

            //var asExcelService = new AsposeExcelService();
            //asExcelService.PrepareExcelFile(srcExcelFilePath, destExcelFilePath);

            //DeleteTempExcelFile(srcExcelFilePath);
            OpenExcelFile(srcExcelFilePath);
        }

        private static void DeleteTempExcelFile(string tempExcelPath)
        {
            Logger.LogInfo($"Deleting temporary excel file at {tempExcelPath}");
            File.Delete(tempExcelPath);
        }

        private static void OpenExcelFile(string pathToExcel)
        {
            Logger.LogSuccess($"Opening excel file at {pathToExcel}");
            System.Diagnostics.Process.Start(pathToExcel);
        }
    }
}
