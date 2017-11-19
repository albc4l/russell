using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RussellScreener.Entities;

namespace RussellScreener.DataAccess {
    public class SixSwissManager : BaseDataAccessManager {

        #region Methods

        /// <summary>
        /// Return the name of the cache to be used for this manager.
        /// </summary>
        /// <returns>Name of the cache to use for this manager</returns>
        public override string GetCacheFileName() {
            return "swiss-stocks.json";
        }

        public override async Task<StockRepository> Process() {
            return await DownloadSwissStockInfos(@".\countries\switzerland\swiss-listings.csv");
        }

        /// <summary>
        /// Asynchronously download Swiss stock information by extracting necessary values from HTML pages
        /// retrieved from SIX.
        /// </summary>
        /// <param name="swissListingsCsv">CSV with Swiss tickers. Prepared in advance.</param>
        protected async Task<StockRepository> DownloadSwissStockInfos(string swissListingsCsv) {
            var rawCsvLines = File.ReadAllLines(swissListingsCsv).ToList();

            // Forced to use Regex to extract the values from the raw HTML page returned by SIX. 
            // I did not find a more convenient API, XML, etc. 
            Regex divYieldSwissStockRegex = new Regex("Dividend yield - indicated annual dividend.*?valign=\"top\">(.*?)</td>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Regex roaSwissStockRegex = new Regex("Return on assets.*?valign=\"top\">(.*?)</td>", RegexOptions.CultureInvariant | RegexOptions.Compiled);

            var swissStatsUrl = ConfigurationManager.AppSettings["SixSwissStockStatsUrl"];

            StockRepository repository = new StockRepository();

            // Prepare Swiss stocks. Note that we only extract a very small subset of what is potentially available. The goal
            // is to have sufficient information for the screening. More advanced strategies would need more extraction to have
            // other metrics.
            foreach (string t in rawCsvLines) {
                var values = t.Split(';');

                var name = values[0];
                var ticker = values[1];
                var isin = values[2];
                var sector = values[3];

                var stock = new Stock(ticker) {
                    Company = {
                        CompanyName = name,
                        Industry = sector,
                        Sector = sector
                    },
                    Isin = isin
                };

                using (var wc = new WebClient()) {
                    try {
                        Console.WriteLine(name, ticker);
                        // we are forced to do HTML scrapping, no known CSV or free API for Swiss stocks
                        string url = string.Format(swissStatsUrl, isin);

                        var stockStatsRawHtml = await wc.DownloadStringTaskAsync(new Uri(url));
                        if (stockStatsRawHtml.Contains("No data is available at present")) {
                            stockStatsRawHtml = await wc.DownloadStringTaskAsync(new Uri(string.Format(swissStatsUrl.Replace("{0}CHF4", "{0}CHF1"), isin)));
                            if (stockStatsRawHtml.Contains("No data is available at present")) {
                                Console.WriteLine("This ticker has some issues with data. Manual checking? " + ticker);
                            }
                        } 

                        // extract dividend yield from the page
                        stock.Stats.DividendYield = ParseSwissRegexResult(divYieldSwissStockRegex, stockStatsRawHtml);
                        stock.Stats.ReturnOnAssets = ParseSwissRegexResult(roaSwissStockRegex, stockStatsRawHtml);

                        repository.Stocks.Add(stock);
                    } catch (Exception) {
                        Console.WriteLine($"{ticker} not found/error (invalid data, unknown ISIN, etc.)");
                    }
                }
            }

            return repository;
        }

        /// <summary>
        /// Parse the result of the matching regex. 
        /// </summary>
        /// <param name="regex">Regex to use for matching</param>
        /// <param name="rawHtml">HTML to parse</param>
        /// <returns>A double value. 0.0 if the parsing failed or no match was found</returns>
        private double ParseSwissRegexResult(Regex regex, string rawHtml) {
            double value = 0.0;
            var result = regex.Match(rawHtml);
            if (result.Success) {
                var v = result.Groups[1].Value.Replace("%", "");
                double.TryParse(v, out value);
            }
            return value;
        }

        #endregion Methods
    }
}
