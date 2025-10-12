using ClosedXML.Excel;

namespace Bokify.Web.Extensions
{
    public static class ExcelSheetExtension
    {
        public static void AddHeader(this IXLWorksheet sheet, string[] headerCells)
        {
            for (int i = 0; i < headerCells.Length; i++)
            {
                sheet.Cell(1, i + 1).SetValue(headerCells[i]);
            }
        }

        public static void Format(this IXLWorksheet sheet)
        {
            sheet.ColumnsUsed().AdjustToContents();
            sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            sheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.CellsUsed().Style.Border.OutsideBorderColor = XLColor.Black;
        }

        public static void AddTable(this IXLWorksheet sheet, int numberOfRows, int numberOfColumns)
        {
            var range = sheet.Range(1, 1, numberOfRows, numberOfColumns);
            var table = range.CreateTable();

            table.Theme = XLTableTheme.TableStyleMedium16;
            table.ShowAutoFilter = false;
        }
    }
}
