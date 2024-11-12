using OfficeOpenXml;
using System.Data;

namespace Cron_Job
{
    public class ExcelUtility
    {
        public static DataTable ExcelDataToDataTable(string filePath, string sheetName, bool hasHeader = true)
        {
            var dt = new DataTable();
            var file = new FileInfo(filePath);

            // Check if the file exists
            if (!file.Exists)
            {
                Console.WriteLine("Not exists");
                throw new Exception("File " + filePath + " Does Not Exists");
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var xlPackage = new ExcelPackage(file);

            // get the first worksheet in the workbook
            var worksheet = xlPackage.Workbook.Worksheets[sheetName];

            dt = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].ToDataTable(c =>
            {
                c.FirstRowIsColumnNames = true;
            });
            return dt;
        }
    }
}
