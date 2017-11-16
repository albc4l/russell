using System.Collections.Generic;
using Newtonsoft.Json;

namespace RussellScreener.Entities {

    /// <summary>
    /// Simple container for a list of stocks
    /// </summary>
    public class StockRepository : BaseJsonObject {

        #region Constructors

        public StockRepository() {
            Stocks = new List<Stock>();
        }

        #endregion Constructors

        #region Properties

        [JsonProperty("Stocks")]
        public List<Stock> Stocks {
            get;
            set;
        }

        #endregion Properties

    }
}
