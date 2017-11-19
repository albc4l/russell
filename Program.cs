using System;
using System.IO;
using System.Threading.Tasks;
using RussellScreener.DataAccess;
using RussellScreener.Entities;

namespace RussellScreener {
    public class Program {

        #region Methods

        /// <summary>
        /// Main console (async) program.
        /// </summary>
        /// <param name="args">Command-line arguments. Not used</param>
        /// <returns>Task for async running</returns>
        public static async Task Main(string[] args) {
            Console.WriteLine("1. Russell 1000");
            Console.WriteLine("2. Swiss Stocks");
            Console.WriteLine("-- Other keys to Exit --");
            var choice = Console.ReadKey().Key;

            BaseDataAccessManager manager = null;
            string xlsxFilename = null;
            if (choice == ConsoleKey.D1) {
                manager = new IexManager();
                xlsxFilename = "russell_portfolio.xlsx";
            } else if (choice == ConsoleKey.D2) {
                manager = new SixSwissManager();
                xlsxFilename = "six_swiss_portfolio.xlsx";
            } else {
                return;
            }

            await Process(manager, xlsxFilename);
        }

        private static async Task Process(BaseDataAccessManager dataAccessManager, string outputXlsFilename) {
            var cacheName = dataAccessManager.GetCacheFileName();
            FileInfo cacheFileInfo = new FileInfo(cacheName);

            Screener screener = new Screener();

            // Main workflow for Russell 1000. Workflows with other data access managers are similar.
            // 1. Load data from iShares (the Russell 1000 current composition. See DownloadISharesComposition method)
            // 2. Extract tickers
            // 3. Load data from IEX for each ticker (metrics + company info)
            // 4. Store these info into a "StockRepository" which is then saved on disk and used as a cache
            // nb. Steps (1-4) are done when "refreshing" the cache or if no cache is available because it's time-consuming
            // 5. Read the cache, extract the stocks
            // 6. Apply the strategy rules, get a list of stocks for the portfolio
            // 7. Generate an Excel file with results

            bool refreshCache;
            if (!cacheFileInfo.Exists) {
                Console.WriteLine("No cache with stocks information exists.");
                refreshCache = true;
            } else {
                refreshCache = AskQuestion(
                    $"Your cache has been saved on {cacheFileInfo.LastWriteTime}. Do you want to refresh with latest data?");
                Console.WriteLine();
            }

            StockRepositoryManager manager = new StockRepositoryManager();
            StockRepository stockRepository;

            if (refreshCache) {
                Console.WriteLine("Refreshing the cache. Download will start now, please wait...");

                stockRepository = await dataAccessManager.Process();
                manager.WriteStockRepositoryCache(stockRepository, cacheName);
            } else {
                Console.WriteLine("Reading cache...");
                stockRepository = manager.ReadStockRepositoryFromCache(cacheName);
            }

            Console.WriteLine("Analyzing data...");
            var finalPortfolio = screener.PickStocks(stockRepository);

            ExcelManager excelManager = new ExcelManager();
            excelManager.WriteExcelFile(outputXlsFilename, finalPortfolio, stockRepository.Stocks);

            Console.WriteLine($"An excel file has been generated with your portfolio selection. Name: {outputXlsFilename}");
            bool showExcel = AskQuestion("Do you want to open this Excel file now?");
            if (showExcel) {
                System.Diagnostics.Process.Start(outputXlsFilename);
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to close this application.");
            Console.ReadLine();
        }

        /// <summary>
        /// Show a question in console and intercept the result (y)es or (n)o
        /// </summary>
        /// <param name="question">String with the question</param>
        /// <returns>True if user pressed 'y'. False otherwise</returns>
        public static bool AskQuestion(string question) {
            Console.WriteLine(question + " " + "(y)es or (n)o");
            return Console.ReadKey().Key == ConsoleKey.Y;
        }

        #endregion Methods


    }
}