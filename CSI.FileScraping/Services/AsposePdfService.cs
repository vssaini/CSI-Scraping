using System.Collections.Generic;
using System.Linq;
using Aspose.Cells;
using Aspose.Cells.Tables;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using CSI.Common;
using Cell = Aspose.Cells.Cell;
using Cells = Aspose.Cells.Cells;

namespace CSI.FileScraping.Services
{
    internal class AsposePdfService
    {
        public AsposePdfService()
        {
            var licService = new LicenseService();
            licService.SetLicense();
        }

        #region Read PDF to Excel

        public void ReadTableWithinPdfToExcel(string pdfFilePath, string excelFilePath)
        {
            var workbook = new Workbook();
            var worksheetIndex = 0;

            Logger.LogInfo("Reading PDF doc and retrieving pages");

            var pdfDocument = new Document(pdfFilePath);
            foreach (var page in pdfDocument.Pages)
            {
                string sheetName = $"Page {page.Number}";
                Logger.LogWarning($"Processing {sheetName}");

                var tables = GetTablesWithinPdfPage(page);
                if (tables.Count == 0)
                {
                    Logger.LogWarning($"No tables found on {sheetName}");
                    continue;
                }

                Worksheet sheet = GetWorksheet(worksheetIndex, workbook, sheetName);
                string endCell = UseTablesToUpdateCellValuesInWorksheet(tables, sheet);

                //AddListObjectToWorksheet(sheet, endCell);

                //sheet.Cells.DeleteBlankRows();

                worksheetIndex++;
            }

            workbook.Save(excelFilePath);
        }

        private static List<AbsorbedTable> GetTablesWithinPdfPage(Page page)
        {
            var absorber = new TableAbsorber();
            absorber.Visit(page);

            var tables = absorber.TableList
                .Where(t => t.RowList.Count > 1)
                .ToList();

            Logger.LogSuccess($"Page  {page.Number} -  Retrieved {tables.Count} tables");

            return tables;
        }

        private static Worksheet GetWorksheet(int worksheetIndex, Workbook workbook, string sheetName)
        {
            Worksheet sheet;
            if (worksheetIndex == 0)
            {
                sheet = workbook.Worksheets[0];
                sheet.Name = sheetName;
            }
            else
            {
                sheet = workbook.Worksheets.Add(sheetName);
            }

            sheet.AutoFitColumns();

            return sheet;
        }

        private static string UseTablesToUpdateCellValuesInWorksheet(List<AbsorbedTable> tables, Worksheet sheet)
        {
            string endCell = null;
            var headerRowProcessed = false;

            var index = 0;
            var letter = 'A';
            const string startCell = "A1";

            foreach (AbsorbedTable table in tables)
            {
                index++;

                //if (!TableBeginWithCertainLabel(table)) continue;

                Logger.LogInfo($"Processing table with {table.RowList.Count} rows in sheet {sheet.Name}");

                foreach (AbsorbedRow row in table.RowList)
                {
                    //if (!headerRowProcessed)
                    //{
                    //    ReadPdfCellAndCreateExcelColumnsRow(cells);
                    //    headerRowProcessed = true;
                    //    continue;
                    //}

                    endCell = UpdateExcelCellValueFromPdfRow(row, sheet.Cells, letter, index);
                    index++;
                }

                AddListObjectToWorksheet(sheet, startCell, endCell);
            }

            return endCell;
        }

        private static bool TableBeginWithCertainLabel(AbsorbedTable table)
        {
            string firstLabel = GetCellValue(table.RowList[0].CellList[0]);
            return firstLabel.StartsWith("Nominal Size") || firstLabel.StartsWith("Pad Size");
        }

        private static string UseTablesToUpdateCellValuesInWorksheet(List<AbsorbedTable> tables, Worksheet sheet, int index)
        {
            string endCell = null;

            var letter = 'A';


            foreach (AbsorbedTable table in tables)
            {
                Logger.LogInfo($"Processing table with {table.RowList.Count} rows in sheet {sheet.Name}");
                string firstLabel = GetCellValue(table.RowList[0].CellList[0]);
                if (!firstLabel.StartsWith("Nominal Size"))//todo
                {
                    continue;
                }
                bool isFirstRow = true;
                foreach (AbsorbedRow row in table.RowList)
                {
                    if (index != 0 && isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }

                    endCell = UpdateExcelCellValueFromPdfRow(row, sheet.Cells, letter, index);
                    index++;
                }
            }

            return endCell;
        }

        private static void ReadPdfCellAndCreateExcelColumnsRow(Cells cells)
        {
            Logger.LogInfo("Creating column row for Excel sheet");

            var values = new[] { "Nominal Size H x W x D", "Actual Size H x W x D", "Part Number", "Carton Quantity", "Weight (Lbs./Ctn.)", "List Price Each" };

            char letter = 'A';
            foreach (var val in values)
            {
                Cell cell = cells[$"{letter}1"];
                cell.PutValue(val);

                letter = (char)(((int)letter) + 1);
            }
        }

        private static string UpdateExcelCellValueFromPdfRow(AbsorbedRow row, Cells cells, char letter, int index)
        {
            var endCell = "A1";

            foreach (AbsorbedCell absCell in row.CellList)
            {
                endCell = $"{letter}{index}";
                Cell cell = cells[endCell];

                var cellValue = GetCellValue(absCell);
                cell.PutValue(cellValue);

                letter = (char)(letter + 1);
            }

            return endCell;
        }

        private static string GetCellValue(AbsorbedCell absCell)
        {
            TextFragmentCollection textFragmentCollection = absCell.TextFragments;

            string txt = "";
            foreach (TextFragment fragment in textFragmentCollection)
            {
                //string txt = "";
                foreach (TextSegment seg in fragment.Segments)
                {
                    txt += seg.Text;
                }
            }

            return txt;
        }

        private static void AddListObjectToWorksheet(Worksheet sheet, string startCell, string endCell)
        {
            // Adding a new List Object or table to the worksheet
            var loIndex = sheet.ListObjects.Add(startCell, endCell, true);
            ListObject listObject = sheet.ListObjects[loIndex];

            // Adding Default Style to the table
            listObject.TableStyleType = TableStyleType.TableStyleMedium17;
        }

        #endregion

        #region Direct convert PDF to Excel

        public void DirectConvertPdfToExcel(string pdfFilePath, string excelFilePath)
        {
            var workbook = new Workbook();

            var pdfDocument = new Document(pdfFilePath);
            foreach (Page page in pdfDocument.Pages)
            {
                page.Resources.Images.Clear(); // Remove images
                RemoveNonTableFragments(page);
            }

            var options = new ExcelSaveOptions
            {
                MinimizeTheNumberOfWorksheets = false
                // Set output format (XLSX by default )
                // Format = ExcelSaveOptions.ExcelFormat.CSV
            };

            pdfDocument.Save(excelFilePath, options);
        }

        private static void RemoveNonTableFragments(Page page)
        {
            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
            TableAbsorber tableAbsorber = new TableAbsorber();
            tableAbsorber.Visit(page);
            textFragmentAbsorber.Visit(page);

            foreach (TextFragment textFragment in textFragmentAbsorber.TextFragments)
            {
                if (!IsThisFragmentInsideTable(tableAbsorber, textFragment))
                    textFragment.Text = "";
            }
        }

        private static bool IsThisFragmentInsideTable(TableAbsorber tableAbsorber, TextFragment fragment)
        {
            foreach (AbsorbedTable table in tableAbsorber.TableList)
            {
                if (fragment.Rectangle.Intersect(table.Rectangle) != null)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
