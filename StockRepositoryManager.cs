using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RussellScreener.Entities;

namespace RussellScreener {
    public class StockRepositoryManager {

        #region Methods

        /// <summary>
        /// Deserialize a stock repository from a JSON file for caching purpose
        /// </summary>
        /// <param name="cacheFilename">Cache filename</param>
        /// <returns>A deserialized StockRepository object</returns>
        public StockRepository ReadStockRepositoryFromCache(string cacheFilename) {
            var json = File.ReadAllText(cacheFilename);
            return (StockRepository)BaseJsonObject.FromJson<StockRepository>(json);
        }

        /// <summary>
        /// Serialize a stock repository as a JSON file for caching purpose
        /// </summary>
        /// <param name="stockRepository">StockRepository object that must be serialized</param>
        /// <param name="cacheFilename">Cache filename</param>
        public void WriteStockRepositoryCache(StockRepository stockRepository, string cacheFilename) {
            var json = BaseJsonObject.ToJson(stockRepository);
            File.WriteAllText(cacheFilename, json);
        }

        #endregion Methods

    }
}
