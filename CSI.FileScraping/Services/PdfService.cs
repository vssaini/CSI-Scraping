using SautinSoft;
using System;
using System.ComponentModel;
using System.Globalization;

namespace CSI.FileScraping.Services
{
    internal class PdfService
    {
        private readonly BackgroundWorker _bgWorker;

        public PdfService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
        }

        public void CreateExcelUsingTablesInPdf(string pathToPdf, string pathToExcel)
        {
            _bgWorker.ReportProgress(0, "Creating Excel using tables in PDF.");

            try
            {
                CreateExcelFile(pathToPdf, pathToExcel);
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, $"ERROR - {e.Message}.");
            }
        }

        private void CreateExcelFile(string pathToPdf, string pathToExcel)
        {
            // Convert only tables from PDF to XLS spreadsheet and skip all textual data.
            var pdfFocus = new PdfFocus();

            // 'true' = Convert all data to spreadsheet (tabular and even textual).
            // 'false' = Skip textual data and convert only tabular (tables) data.
            pdfFocus.ExcelOptions.ConvertNonTabularDataToSpreadsheet = false;

            // 'true'  = Preserve original page layout.
            // 'false' = Place tables before text.
            pdfFocus.ExcelOptions.PreservePageLayout = true;

            // The information includes the names for the culture, the writing system, 
            // the calendar used, the sort order of strings, and formatting for dates and numbers.
            var ci = new CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalSeparator = ",";
            ci.NumberFormat.NumberGroupSeparator = ".";
            pdfFocus.ExcelOptions.CultureInfo = ci;

            pdfFocus.OpenPdf(pathToPdf);

            if (pdfFocus.PageCount > 0)
            {
                _bgWorker.ReportProgress(0, "Saving rows to excel file");

                int result = pdfFocus.ToExcel(pathToExcel, 1, pdfFocus.PageCount);
                if (result == 0) return;

                if (result == 2)
                    throw new Exception("Can't create excel file. Check excel file path.");

                throw new Exception("Failed to save excel file");
            }

            _bgWorker.ReportProgress(0, "WARNING - PDF is not having any pages.");
        }
    }
}
