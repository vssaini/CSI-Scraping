using CSI.Common;
using CSI.Common.Wesco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public IEnumerable<Product> GetProducts(string pdfFilePath)
        {
            var startTime = DateTime.Now;
            _bgWorker.ReportProgress(0, $"Retrieving of products from PDF started at {startTime:T}.");

            var dataTable = GetDataFromPdfFile(pdfFilePath);

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];

                var productId = row[1].ToString();
                if (string.IsNullOrWhiteSpace(productId))
                {
                    _bgWorker.ReportProgress(0, $"{i + 1}/{dataTable.Rows.Count} - Skipping as no product Id exist in current row.");
                    continue;
                }

                _bgWorker.ReportProgress(0, $"{i + 1}/{dataTable.Rows.Count} - Processing the product '{productId}'");

                yield return new Product
                {
                    Id = i + 1,
                    ProductId = productId,
                    Name = row[2].ToString(),
                    Price = row[4].ToString(),
                    Status = Constants.StatusFound
                };
            }

            var dateDiff = DateTime.Now - startTime;
            _bgWorker.ReportProgress(0, $"Retrieval of products from PDF completed at {DateTime.Now:T} (within {dateDiff.Seconds} seconds).");
        }

        private DataTable GetDataFromPdfFile(string pdfFilePath)
        {
            var dirPath = GetDirectoryPath();
            var fileName = Path.GetFileNameWithoutExtension(pdfFilePath);

            var srcExcelFilePath = $"{dirPath}\\{fileName}_src.xlsx";
            var destExcelFilePath = $"{dirPath}\\{fileName}_dest.xlsx";

            var pdfService = new PdfService(_bgWorker);
            pdfService.CreateExcelUsingTablesInPdf(pdfFilePath, srcExcelFilePath);

            var excelService = new ExcelService(_bgWorker);
            excelService.MergeSourceSheetsToDestinationFile(srcExcelFilePath, destExcelFilePath);

            var dataTable = excelService.GetDataFromExcelFile(destExcelFilePath);

            DeleteExcelFile(srcExcelFilePath);
            DeleteExcelFile(destExcelFilePath);

            return dataTable;
        }

        private static string GetDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private void DeleteExcelFile(string excelFilePath)
        {
            _bgWorker.ReportProgress(0, $"Deleting excel file at {excelFilePath}");
            File.Delete(excelFilePath);
        }
    }
}
