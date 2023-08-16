using System.IO;
using System.Reflection;
using CSI.Common;

namespace CSI.FileScraping.Services
{
    internal class FileService
    {
        public static void ReadPdfAndGenerateExcelFile()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dirPath = $"{assemblyPath}\\Docs";
            const string fileName = "Securitech";

            var pdfFilePath = $"{dirPath}\\{fileName}.pdf";
            var srcExcelFilePath = $"{dirPath}\\{fileName}_src.xlsx";
            var destExcelFilePath = $"{dirPath}\\{fileName}_dest.xlsx";

            SautinFileService.CreateExcelFromTableInPdf(pdfFilePath, srcExcelFilePath);

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
