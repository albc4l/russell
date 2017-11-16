using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using RussellScreener.Entities;

namespace RussellScreener {
    public class Screener {

        #region Methods

        /// <summary>
        /// Pick the final portfolio stocks according to Piard's rules.
        /// </summary>
        /// <param name="stockRepository">StockRepository object with all stocks that must be analyzed</param>
        /// <returns>List of selected stock entries sorted according to the rules, starting with the best candidate.</returns>
        public List<Stock> PickStocks(StockRepository stockRepository) {
            var topDividendsCount = int.Parse(ConfigurationManager.AppSettings["ModelTopDividendsCount"]);
            var maxStockPerSector = int.Parse(ConfigurationManager.AppSettings["ModelStockPerSectorLimit"]);
            var maxPortfolioStockCount = int.Parse(ConfigurationManager.AppSettings["ModelPortfolioStockCount"]);

            // Apply Piard's rules:
            // * Take the first M stocks sorted by dividend yield (M=200)
            // * Rank them by ROA (higher = better)
            // * Pick the top N stocks (N=20), but take the next entry if you have already too many stocks in a given sector (max stock per sector = 5)

            var bestDividends = stockRepository.Stocks.OrderByDescending(s => s.Stats.DividendYield).Take(topDividendsCount);
            var bestRoa = bestDividends.OrderByDescending(s => s.Stats.ReturnOnAssets);

            List<Stock> selection = new List<Stock>();
            Dictionary<string, int> sectorCountMap = new Dictionary<string, int>();

            foreach (var s in bestRoa) {
                int sectorCount;
                var sector = s.Company.Sector;

                if (!sectorCountMap.TryGetValue(sector, out sectorCount)) {
                    sectorCountMap.Add(sector, 0);
                }
                sectorCountMap[sector]++;

                if (sectorCount > maxStockPerSector) {
                    // Console.WriteLine("Skip this sector " + s.Company.Sector);
                    continue;
                }

                selection.Add(s);
            }

            return selection.Take(maxPortfolioStockCount).ToList();
        }

        #endregion Methods

    }
}
