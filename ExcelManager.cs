using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RussellScreener.Entities;

namespace RussellScreener {
    public class ExcelManager {

        #region Methods

        /// <summary>
        /// Write an Excel file with two worksheets: the selection of stocks for the final portfolio and a worksheet with all stocks from the ETF
        /// with all fields given by the IEX API.
        /// </summary>
        /// <param name="excelFileName">Name of the target excel file</param>
        /// <param name="portfolioStocks">List of stocks for the final portfolio selected according to the strategy rules</param>
        /// <param name="allStocks">List of all stocks</param>
        public void WriteExcelFile(string excelFileName, List<Stock> portfolioStocks, List<Stock> allStocks) {
            var newFile = new FileInfo(excelFileName);

            // discard any existing file
            if (newFile.Exists) {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(excelFileName);
            }

            var allStatsProperties = typeof(StockResultStats).GetProperties();
            var allCompanyProperties = typeof(StockResultCompany).GetProperties();

            using (var package = new ExcelPackage(newFile)) {
                FillWorksheet(portfolioStocks, package.Workbook.Worksheets.Add("Selection"), allCompanyProperties, allStatsProperties);
                FillWorksheet(allStocks, package.Workbook.Worksheets.Add("All Stocks"), allCompanyProperties, allStatsProperties);
                package.Save();
            }
        }

        /// <summary>
        /// Create a worksheet containing a list of stocks. 
        /// The first header cells are a summary with the important metrics for Piard's rules.
        /// The other properties coming from IEX API are then appended as additional header cells.
        /// </summary>
        /// <param name="stocks">List of stocks to write</param>
        /// <param name="ws">Target Excel worksheet</param>
        /// <param name="allCompanyProperties">All properties within the StockResultCompany object</param>
        /// <param name="allStatsProperties">All properties within the StockResultStats object</param>
        private void FillWorksheet(List<Stock> stocks, ExcelWorksheet ws, PropertyInfo[] allCompanyProperties, PropertyInfo[] allStatsProperties) {
            ws.Cells[1, 1].Value = "Ticker";
            ws.Cells[1, 2].Value = "Company";
            ws.Cells[1, 3].Value = "Sector";
            ws.Cells[1, 4].Value = "Dividend Yield";
            ws.Cells[1, 5].Value = "ROA";

            int currentHeaderCol = 6;

            foreach (var prop in allCompanyProperties) {
                ws.Cells[1, currentHeaderCol++].Value = prop.Name;
            }

            foreach (var prop in allStatsProperties) {
                ws.Cells[1, currentHeaderCol++].Value = prop.Name;
            }

            int lastHeaderCol = currentHeaderCol;

            int currentRow = 2;

            foreach (var s in stocks) {
                ws.Cells[currentRow, 1].Value = s.Ticker;
                ws.Cells[currentRow, 2].Value = s.Company.CompanyName;
                ws.Cells[currentRow, 3].Value = s.Company.Sector;
                ws.Cells[currentRow, 4].Value = s.Stats.DividendYield;
                ws.Cells[currentRow, 5].Value = s.Stats.ReturnOnAssets;

                int currentCol = 6;

                foreach (var prop in allCompanyProperties) {
                    ws.Cells[currentRow, currentCol].Value = prop.GetValue(s.Company);
                    currentCol++;
                }

                foreach (var prop in allStatsProperties) {
                    ws.Cells[currentRow, currentCol].Value = prop.GetValue(s.Stats);
                    currentCol++;
                }

                currentRow++;
            }

            // set header formatting for the first important cells
            using (var range = ws.Cells[1, 1, 1, 5]) {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
            }

            // different formatting for other header cells
            using (var range = ws.Cells[1, 6, 1, lastHeaderCol]) {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                range.Style.Font.Color.SetColor(Color.White);
            }

            // apply auto-filtering + autofit on all columns
            ws.Cells[1, 1, 1, lastHeaderCol].AutoFilter = true;
            ws.Cells.AutoFitColumns(0);
        }

        #endregion Methods
    }
}