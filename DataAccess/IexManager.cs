using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RussellScreener.Entities;

namespace RussellScreener.DataAccess {
    public class IexManager : BaseDataAccessManager {

        /// <summary>
        /// Extract all tickers of the holdings within the ETF
        /// It simply goes through the CSV, starting at a given position where data is located (EtfCsvDataLineNumber).
        /// For each line, the first cell (the ticker) is extract and quotes chars are discarded.
        /// </summary>
        /// <param name="csvFilename">Name of the iShares CSV containing the holdings info</param>
        /// <returns>A list of string of tickers</returns>
        public List<string> ExtractISharesTickers(string csvFilename) {
            List<string> tickers = new List<string>();

            var rawCsvLines = File.ReadLines(csvFilename).ToList();
            var startIndex = int.Parse(ConfigurationManager.AppSettings["EtfCsvDataLineNumber"]);

            for (var n = startIndex; n < rawCsvLines.Count; n++) {
                var ticker = rawCsvLines[n].Split(',')[0].Replace("\"", "");
                tickers.Add(ticker);
            }

            return tickers;
        }

        /// <summary>
        /// Download the holdings as a CSV from iShares page (as of Nov. 2017)
        /// </summary>
        /// <param name="csvFilename">URL of the CSV - Stored in the app.config</param>
        public void DownloadISharesComposition(string csvFilename) {
            var etfCompositionUrl = ConfigurationManager.AppSettings["EtfCompositionUrl"];
            using (WebClient wc = new WebClient()) {
                wc.DownloadFile(etfCompositionUrl, csvFilename);
            }
        }

        /// <summary>
        /// Asynchronously download using IEX API all stocks related to the tickers given in input 
        /// </summary>
        /// <param name="tickers">List of tickers to download</param>
        /// <returns>A stock repository with all stocks corresponding to the input tickers (except those that failed during fetching)</returns>
        public async Task<StockRepository> DownloadAllStocksFromIex(List<string> tickers) {
            var stockRepository = new StockRepository();
            var iexStockStatsUrl = ConfigurationManager.AppSettings["IexStockStatsUrl"];
            var iexStockCompanyUrl = ConfigurationManager.AppSettings["IexStockCompanyUrl"];

            var tasks = new List<Task>();

            // prepare each task that takes care of a single ticker download
            foreach (var ticker in tickers) {
                tasks.Add(Task.Run(async () => {
                    await DownloadStockAsync(ticker, iexStockStatsUrl, iexStockCompanyUrl, stockRepository);
                }));
            }

            // launch them in parallel and wait for them to complete
            await Task.WhenAll(tasks.ToArray());
            return stockRepository;
        }

        /// <summary>
        /// Asynchronously download stock information (company + stats) using IEX API and add it to the stock repository
        /// </summary>
        /// <param name="ticker">Ticker name (ex. AAPL)</param>
        /// <param name="iexStockStatsUrl">IEX API URL for the stock stats</param>
        /// <param name="iexStockCompanyUrl">IEX API URL for the stock company info</param>
        /// <param name="stockRepository">Repository object where the stock will be added</param>
        /// <returns>A task performing the download of the stock info</returns>
        public async Task DownloadStockAsync(string ticker, string iexStockStatsUrl, string iexStockCompanyUrl, StockRepository stockRepository) {
            using (var wc = new WebClient()) {
                try {
                    Stock s = new Stock(ticker);

                    var stockStatsJson = await wc.DownloadStringTaskAsync(new Uri(string.Format(iexStockStatsUrl, ticker)));
                    s.Stats = (StockResultStats)BaseJsonObject.FromJson<StockResultStats>(stockStatsJson);

                    var stockCompanyJson = await wc.DownloadStringTaskAsync(new Uri(string.Format(iexStockCompanyUrl, ticker)));
                    s.Company = (StockResultCompany)BaseJsonObject.FromJson<StockResultCompany>(stockCompanyJson);

                    Console.Write(".");
                    //($"Processed {ticker}");
                    stockRepository.Stocks.Add(s);
                } catch (Exception) {
                    Console.WriteLine($"{ticker} not found/error (invalid data, non-US equity or different from official IEX ticker?)");
                }
            }
        }

        public override string GetCacheFileName() {
            return "russel-iex-stocks.json";
        }

    }
}
