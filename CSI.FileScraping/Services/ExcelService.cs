using Aspose.Cells;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using LoadOptions = Aspose.Cells.LoadOptions;

namespace CSI.FileScraping.Services
{
    internal class ExcelService
    {
        private readonly BackgroundWorker _bgWorker;
        private static PasteOptions _pasteOptions;

        public ExcelService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;

            var licService = new LicenseService();
            licService.SetLicense();

            _pasteOptions = new PasteOptions
            {
                PasteType = PasteType.All,
                SkipBlanks = true
            };
        }

        internal void MergeSourceSheetsToDestinationFile(string srcExcelFilePath, string destExcelFilePath)
        {
            var srcWb = GetSourceWorkbook(srcExcelFilePath);
            var destWb = new Workbook();

            _bgWorker.ReportProgress(0, $"Merging sheets from source excel file to destination excel file '{destExcelFilePath}'.");

            MergeSheets(srcWb, destWb);

            destWb.Save(destExcelFilePath);
        }

        private Workbook GetSourceWorkbook(string srcExcelFilePath)
        {
            _bgWorker.ReportProgress(0, $"Reading source excel file '{srcExcelFilePath}'.");

            using (var fStream = new FileStream(srcExcelFilePath, FileMode.Open))
            {
                var opt = new LoadOptions
                {
                    MemorySetting = MemorySetting.MemoryPreference
                };

                return new Workbook(fStream, opt);
            }
        }

        private void MergeSheets(Workbook srcWb, Workbook destWb)
        {
            Worksheet destSheet = destWb.Worksheets[0];

            var totalRowCount = 0;
            var firstSheetProcessed = false;

            foreach (var srcSheet in srcWb.Worksheets)
            {
                try
                {
                    _bgWorker.ReportProgress(0, $"Processing worksheet - {srcSheet.Name}");

                    // Delete header and telephone rows
                    srcSheet.Cells.DeleteRows(0, firstSheetProcessed ? 4 : 3);

                    MergeSheetsByRange(srcSheet, destSheet, ref totalRowCount);
                    firstSheetProcessed = true;
                }
                catch (Exception e)
                {
                    _bgWorker.ReportProgress(0, $"Error processing worksheet - {srcSheet.Name}. ERROR - {e.Message}");
                }
            }
        }

        private static void MergeSheetsByRange(Worksheet srcSheet, Worksheet destSheet, ref int totalRowCount)
        {
            // Ref - https://docs.aspose.com/cells/net/combine-multiple-worksheets-into-a-single-worksheet/
            Range sourceRange = srcSheet.Cells.MaxDisplayRange;

            Range destRange = destSheet.Cells.CreateRange(sourceRange.FirstRow + totalRowCount, sourceRange.FirstColumn,
                sourceRange.RowCount, sourceRange.ColumnCount);

            destRange.Copy(sourceRange, _pasteOptions);

            totalRowCount = sourceRange.RowCount + totalRowCount;
        }

        public DataTable GetDataFromExcelFile(string excelFilePath, int firstRow)
        {
            _bgWorker.ReportProgress(0, $"Preparing data table from excel file '{excelFilePath}'.");

            var wb = new Workbook(excelFilePath);
            var ws = wb.Worksheets[0];

            var dataTable = ws.Cells.ExportDataTable(firstRow, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true);

            return dataTable;
        }
    }
}
