using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            FileInfo cacheFileInfo = new FileInfo(STOCK_CACHE_FILENAME);

            // Main workflow
            // 1. Load data from iShares (the Russell 100 current composition. See DownloadRussellComposition method)
            // 2. Extract tickers
            // 3. Load data from IEX for each ticker (metrics + company info)
            // 4. Store these info into a "StockRepository" which is then saved on disk and used as a cache
            // nb. Steps (1-4) are done when "refreshing" the cache or if no cache is available because it's time-consuming
            // 5. Read the cache, extract the stocks
            // 6. Apply the strategy rules, get a list of stocks for the portfolio
            // 7. Generate an Excel file with results

            Screener screener = new Screener();

            bool refreshCache;
            if (!cacheFileInfo.Exists) {
                Console.WriteLine("No cache with stocks information exists.");
                refreshCache = true;
            } else {
                refreshCache = AskQuestion($"Your cache has been saved on {cacheFileInfo.LastWriteTime}. Do you want to refresh with latest data?");
                Console.WriteLine();
            }

            StockRepositoryManager manager = new StockRepositoryManager();
            StockRepository stockRepository;

            if (refreshCache) {
                Console.WriteLine("Refreshing the cache. Download will start now, please wait...");
                manager.DownloadRussellComposition(ETF_FILENAME);
                List<string> tickers = manager.ExtractRussellTickers(ETF_FILENAME);
                stockRepository = await manager.DownloadAllStocksFromIex(tickers);
                manager.WriteStockRepositoryCache(stockRepository, STOCK_CACHE_FILENAME);
            } else {
                Console.WriteLine("Reading cache...");
                stockRepository = manager.ReadStockRepositoryFromCache(STOCK_CACHE_FILENAME);
            }

            Console.WriteLine("Analyzing data...");
            var finalPortfolio = screener.PickStocks(stockRepository);
            string xlxsFilename = ConfigurationManager.AppSettings["ModelPortfolioExcelFile"];

            ExcelManager excelManager = new ExcelManager();
            excelManager.WriteExcelFile(xlxsFilename, finalPortfolio, stockRepository.Stocks);

            Console.WriteLine($"An excel file has been generated with your portfolio selection. Name: {xlxsFilename}");
            bool showExcel = AskQuestion("Do you want to open this Excel file now?");
            if (showExcel) {
                Process.Start(xlxsFilename);
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to close this application.");
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

        #region Fields

        const string ETF_FILENAME = "russell.csv";
        const string STOCK_CACHE_FILENAME = "stocks.json";

        #endregion Fields

    }
}