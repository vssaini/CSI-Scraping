﻿using System;
using System.Globalization;
using CSI.Common;
using SautinSoft;

namespace CSI.FileScraping.Services
{
    internal class SautinFileService
    {
        public static void CreateExcelFromTableInPdf(string pathToPdf, string pathToExcel)
        {
            Logger.LogInfo("Creating Excel with tables in PDF.");

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
                Logger.LogInfo("Saving table(s) to excel file");

                int result = pdfFocus.ToExcel(pathToExcel, 1, pdfFocus.PageCount);
                if (result == 0) return;

                if (result == 2)
                    throw new Exception("Can't create output file. Check output path.");

                throw new Exception("Failed to save excel file");
            }

            Logger.LogWarning("PDF is not having any pages.");
        }
    }
}