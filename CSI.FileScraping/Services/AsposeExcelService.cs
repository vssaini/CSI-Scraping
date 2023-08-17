using Aspose.Cells;
using System;
using System.ComponentModel;
using System.IO;
using Cell = Aspose.Cells.Cell;
using Cells = Aspose.Cells.Cells;
using LoadOptions = Aspose.Cells.LoadOptions;

namespace CSI.FileScraping.Services
{
    internal class AsposeExcelService
    {
        private readonly BackgroundWorker _bgWorker;
        private static PasteOptions _pasteOptions;

        public AsposeExcelService(BackgroundWorker bgWorker)
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

        internal void PrepareExcelFile(string srcExcelFilePath, string destExcelFilePath)
        {
            var srcWb = GetSourceWorkbook(srcExcelFilePath);
            var destWb = new Workbook();

            //CopyCellsFromSourceToDestinationExcelFile(srcWb, destWb);
            MergeSheetsFromSourceToDestination(srcWb, destWb);

            destWb.Save(destExcelFilePath);
        }

        private static Workbook GetSourceWorkbook(string srcExcelFilePath)
        {
            using (var fStream = new FileStream(srcExcelFilePath, FileMode.Open))
            {
                var opt = new LoadOptions
                {
                    MemorySetting = MemorySetting.MemoryPreference
                };

                return new Workbook(fStream, opt);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void CopyCellsFromSourceToDestinationExcelFile(Workbook srcWb, Workbook destWb)
        {
            _bgWorker.ReportProgress(0, "Copying cells from source to destination Excel file");

            //Worksheet srcSheet = srcWb.Worksheets[0];
            Worksheet destSheet = destWb.Worksheets[0];

            var lastRowIndex = 0;
            var firstSheet = true;

            foreach (var ws in srcWb.Worksheets)
            {
                _bgWorker.ReportProgress(0, $"Processing worksheet - {ws.Name}");

                var cells = ws.Cells;

                // Create source first cell and end cell name
                var srcStartRowIndex = cells.MinDataRow + 1;
                var srcEndRowIndex = cells.MaxDataRow + 1;

                var srcFirstCellName = $"A{srcStartRowIndex}";

                var srcFirstCellText = Convert.ToString(cells[srcFirstCellName].Value);
                if (!srcFirstCellText.StartsWith("Nominal") && !srcFirstCellText.StartsWith("Pad")) continue;

                if (!firstSheet)
                {
                    srcStartRowIndex += 1;
                    srcEndRowIndex += 1;
                }

                srcFirstCellName = $"A{srcStartRowIndex}";
                var srcEndCellName = $"F{srcEndRowIndex}";

                //var endCellText = Convert.ToString(cells[endCellName].Value);

                // Source range to be copied
                var srcAddress = $"{srcFirstCellName}:{srcEndCellName}";
                _bgWorker.ReportProgress(0, $"Creating source range {srcAddress}");
                Range sourceRange = ws.Cells.CreateRange(srcAddress);

                int destStartRowIndex;
                var destEndRowIndex = srcEndRowIndex;

                if (firstSheet)
                {
                    firstSheet = false;
                    destStartRowIndex = 0;
                    lastRowIndex = destEndRowIndex;
                }
                else
                {
                    destStartRowIndex = lastRowIndex + 1;
                    destEndRowIndex = lastRowIndex + srcEndRowIndex;
                    lastRowIndex = destEndRowIndex;
                }

                var destFirstCellName = $"A{destStartRowIndex + 1}";
                var destEndCellName = $"F{destEndRowIndex}";

                var destAddress = $"{destFirstCellName}:{destEndCellName}";
                _bgWorker.ReportProgress(0, $"Creating destination range {destAddress}");
                Range destRange = destSheet.Cells.CreateRange(destAddress);

                var options = new PasteOptions
                {
                    PasteType = PasteType.All,
                    SkipBlanks = true
                };

                destRange.Copy(sourceRange, options);
            }
        }

        private void MergeSheetsFromSourceToDestination(Workbook srcWb, Workbook destWb)
        {
            _bgWorker.ReportProgress(0, "Merging cells from source to destination Excel file");

            Worksheet destSheet = destWb.Worksheets[0];

            var totalRowCount = 0;

            foreach (var srcSheet in srcWb.Worksheets)
            {
                // if (srcSheet.Name != "Page 1" && srcSheet.Name != "Page 2" && srcSheet.Name != "Page 5") continue;

                try
                {
                    _bgWorker.ReportProgress(0, $"Processing worksheet - {srcSheet.Name}");

                    //var srcCells = srcSheet.Cells;

                    //var srcStartRowIndex = srcCells.MinDataRow + 1;
                    //var srcFirstCellName = $"A{srcStartRowIndex}";
                    //var srcFirstCellText = Convert.ToString(srcCells[srcFirstCellName].Value);
                    //if (!srcFirstCellText.StartsWith("Nominal") && !srcFirstCellText.StartsWith("Pad")) continue;

                    MergeSheetsByRange(srcSheet, destSheet, ref totalRowCount);

                    //if (srcFirstCellText.StartsWith("Nominal"))
                    //{
                    //    MergeSheetsByRange(srcSheet, destSheet, ref totalRowCount);
                    //}
                    //else
                    //{
                    //    MergeSheetsByRangeForPadAndRoll(srcSheet, destSheet, ref totalRowCount);
                    //}
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

        private static void MergeSheetsByRangeForPadAndRoll(Worksheet srcSheet, Worksheet destSheet, ref int totalRowCount)
        {
            var srcCells = srcSheet.Cells;
            var destCells = destSheet.Cells;

            totalRowCount += 1; // For adding one row top space

            ProcessPadCells(srcCells, destCells, ref totalRowCount);

            totalRowCount += 1; // For adding one row top space

            ProcessRollCells(srcCells, destCells, ref totalRowCount);
        }

        private static void ProcessPadCells(Cells srcCells, Cells destCells, ref int totalRowCount)
        {
            const string searchTerm = "Pad";

            var rollPrevColIndex = GetRollColPrevIndex(srcCells);
            if (rollPrevColIndex == -1)
            {
                rollPrevColIndex = srcCells.MaxColumn;
            }

            var padCell = FindCell(srcCells, null, searchTerm);
            var upperLeftCell = GetUpperLeftCell(padCell.Row, 0);

            var nextRowIndex = GetNextRowIndex(srcCells, padCell.Name, searchTerm);
            var lrRowIndex = nextRowIndex > 0 ? nextRowIndex - 1 : 0;
            var lowerRightCell = GetLowerRightCell(lrRowIndex, rollPrevColIndex);

            var dUpperLeftCell = GetUpperLeftCell(1 + totalRowCount, 0);
            var dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);

            while (padCell != null)
            {
                CopySourceCellsToDestination(srcCells, destCells, upperLeftCell, lowerRightCell, dUpperLeftCell, dLowerRightCell, ref totalRowCount);

                padCell = FindCell(srcCells, padCell.Name, searchTerm);
                if (padCell == null) break;

                upperLeftCell = GetUpperLeftCell(padCell.Row, 0);

                dUpperLeftCell = GetUpperLeftCell(1 + totalRowCount, 0);

                nextRowIndex = GetNextRowIndex(srcCells, padCell.Name, searchTerm);
                if (nextRowIndex == 0) // Last range
                {
                    padCell = null;
                    lowerRightCell = GetLowerRightCell(srcCells.MaxDataRow, rollPrevColIndex);
                    dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);

                    CopySourceCellsToDestination(srcCells, destCells, upperLeftCell, lowerRightCell, dUpperLeftCell, dLowerRightCell, ref totalRowCount);
                }
                else
                {
                    lrRowIndex = nextRowIndex > 0 ? nextRowIndex - 1 : 0;
                    lowerRightCell = GetLowerRightCell(lrRowIndex, rollPrevColIndex);
                    dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);
                }
            }
        }

        private static void ProcessRollCells(Cells srcCells, Cells destCells, ref int totalRowCount)
        {
            const string searchTerm = "Roll";

            var rollCell = FindCell(srcCells, null, searchTerm);
            if (rollCell == null) return;

            var rollColIndex = GetRollColIndex(srcCells);
            var upperLeftCell = GetUpperLeftCell(rollCell.Row, rollColIndex);

            var nextRowIndex = GetNextRowIndex(srcCells, rollCell.Name, searchTerm);
            var lrRowIndex = nextRowIndex > 0 ? nextRowIndex - 1 : 0;
            var lowerRightCell = GetLowerRightCell(lrRowIndex, srcCells.MaxColumn);

            var dUpperLeftCell = GetUpperLeftCell(1 + totalRowCount, 0);
            var dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);

            while (rollCell != null)
            {
                CopySourceCellsToDestination(srcCells, destCells, upperLeftCell, lowerRightCell, dUpperLeftCell, dLowerRightCell, ref totalRowCount);

                rollCell = FindCell(srcCells, rollCell.Name, searchTerm);
                if (rollCell == null) break;

                upperLeftCell = GetUpperLeftCell(rollCell.Row, rollColIndex);

                dUpperLeftCell = GetUpperLeftCell(1 + totalRowCount, 0);

                nextRowIndex = GetNextRowIndex(srcCells, rollCell.Name, searchTerm);
                if (nextRowIndex == 0) // Last range
                {
                    rollCell = null;
                    lowerRightCell = GetLowerRightCell(srcCells.MaxDataRow, srcCells.MaxColumn);
                    dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);

                    CopySourceCellsToDestination(srcCells, destCells, upperLeftCell, lowerRightCell, dUpperLeftCell, dLowerRightCell, ref totalRowCount);
                }
                else
                {
                    lrRowIndex = nextRowIndex > 0 ? nextRowIndex - 1 : 0;
                    lowerRightCell = GetLowerRightCell(lrRowIndex, srcCells.MaxColumn);
                    dLowerRightCell = GetDestLowerRightCell(srcCells, upperLeftCell, lowerRightCell, totalRowCount);
                }
            }
        }

        private static void CopySourceCellsToDestination(Cells srcCells, Cells destCells, string upperLeftCell, string lowerRightCell, string dUpperLeftCell, string dLowerRightCell, ref int totalRowCount)
        {
            var srcRange = srcCells.CreateRange(upperLeftCell, lowerRightCell);

            Range destRange = destCells.CreateRange(dUpperLeftCell, dLowerRightCell);
            destRange.Copy(srcRange, _pasteOptions);

            totalRowCount = srcRange.RowCount + totalRowCount;
        }

        private static int GetRollColIndex(Cells cells)
        {
            var rollCell = FindCell(cells, null, "Roll");
            if (rollCell == null) return -1;

            return rollCell.Column;
        }

        private static int GetRollColPrevIndex(Cells cells)
        {
            var rollCell = FindCell(cells, null, "Roll");
            if (rollCell == null) return -1;

            return rollCell.Column - 1;
        }

        private static int GetNextRowIndex(Cells cells, string prevCellAddress, string searchTerm)
        {
            var nextPadCell = FindCell(cells, prevCellAddress, searchTerm);
            return nextPadCell?.Row ?? 0;
        }

        private static string GetUpperLeftCell(int row, int col)
        {
            return CellsHelper.CellIndexToName(row, col);
        }

        private static string GetLowerRightCell(int row, int col)
        {
            return CellsHelper.CellIndexToName(row, col);
        }

        private static string GetDestLowerRightCell(Cells srcCells, string upperLeftCell, string lowerRightCell, int totalRowCount)
        {
            var range = srcCells.CreateRange(upperLeftCell, lowerRightCell);
            var row = range.RowCount + totalRowCount;
            var dLowerRightCell = GetLowerRightCell(row, 4);
            return dLowerRightCell;
        }

        private static Cell FindCell(Cells cells, string prevCellAddress, string searchTerm)
        {
            var opts = new FindOptions
            {
                CaseSensitive = false,
                SearchBackward = false,
                SeachOrderByRows = true,
                LookAtType = LookAtType.StartWith
            };

            var prevCell = string.IsNullOrWhiteSpace(prevCellAddress) ? null : cells[prevCellAddress];
            var cell = cells.Find(searchTerm, prevCell, opts);
            return cell;
        }

        public void DeleteColumnsInExcel(string tempExcelPath, string pathToExcel)
        {
            _bgWorker.ReportProgress(0, "Deleting extra columns on right side starting from 6th index");

            // Creating a file stream containing the Excel file to be opened
            var fStream = new FileStream(tempExcelPath, FileMode.Open);

            // Specify the LoadOptions
            var opt = new LoadOptions
            {
                MemorySetting = MemorySetting.MemoryPreference
            };

            // Instantiating a Workbook object
            // Opening the Excel file through the file stream
            var workbook = new Workbook(fStream, opt);

            // Accessing the first worksheet in the Excel file
            Worksheet worksheet = workbook.Worksheets[0];

            //DeleteTableInWorksheet(worksheet);

            // Deleting a column from the worksheet at 5th position
            var numOfColumnsToDelete = worksheet.Cells.Columns.Count - 5;

            worksheet.Cells.DeleteColumns(6, numOfColumnsToDelete, false);

            // Saving the modified Excel file
            _bgWorker.ReportProgress(0, $"Saving modified excel file at {pathToExcel}");
            workbook.Save(pathToExcel);

            // Closing the file stream to free all resources
            fStream.Close();
        }
    }
}
