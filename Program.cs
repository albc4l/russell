using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
            StockRepositoryManager manager = new StockRepositoryManager();

            SixSwissManager swissManager = new SixSwissManager();
            //  StockRepository stockRepository = await swissManager.DownloadSwissStockInfos(@".\countries\switzerland\swiss-listings.csv");

            //  manager.WriteStockRepositoryCache(stockRepository, swissManager.GetCacheFileName());
            var stockRepository = manager.ReadStockRepositoryFromCache(swissManager.GetCacheFileName());

            var screener = new Screener();
            Console.WriteLine("Analyzing data...");
            var finalPortfolio = screener.PickStocks(stockRepository);
            string xlxsFilename = ConfigurationManager.AppSettings["ModelPortfolioExcelFile"];

            ExcelManager excelManager = new ExcelManager();
            excelManager.WriteExcelFile(xlxsFilename, finalPortfolio, stockRepository.Stocks);

            // await ApplyRussell1000();
        }

        private static async Task ApplyRussell1000() {
            IexManager iex = new IexManager();
            var cacheName = iex.GetCacheFileName();
            FileInfo cacheFileInfo = new FileInfo(cacheName);

            // Main workflow
            // 1. Load data from iShares (the Russell 100 current composition. See DownloadISharesComposition method)
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
                refreshCache = AskQuestion(
                    $"Your cache has been saved on {cacheFileInfo.LastWriteTime}. Do you want to refresh with latest data?");
                Console.WriteLine();
            }

            StockRepositoryManager manager = new StockRepositoryManager();

            StockRepository stockRepository;

            if (refreshCache) {
                Console.WriteLine("Refreshing the cache. Download will start now, please wait...");
                iex.DownloadISharesComposition(ETF_FILENAME);
                List<string> tickers = iex.ExtractISharesTickers(ETF_FILENAME);
                stockRepository = await iex.DownloadAllStocksFromIex(tickers);
                manager.WriteStockRepositoryCache(stockRepository, cacheName);
            } else {
                Console.WriteLine("Reading cache...");
                stockRepository = manager.ReadStockRepositoryFromCache(cacheName);
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


        #endregion Fields

    }
}