using Newtonsoft.Json;

namespace RussellScreener.Entities {

    /// <summary>
    /// Stock object holding the company + statistics information for a given ticker.
    /// </summary>
    public class Stock : BaseJsonObject {

        #region Properties

        public Stock(string ticker) {
            Ticker = ticker;
            Company = new StockResultCompany();
            Stats = new StockResultStats();
        }

        [JsonProperty("ticker")]
        public string Ticker {
            get;
            private set;
        }

        [JsonProperty("company")]
        public StockResultCompany Company {
            get;
            set;
        }

        [JsonProperty("stats")]
        public StockResultStats Stats {
            get;
            set;
        }

        public string Isin {
            get;
            set;
        }

        #endregion Properties

    }
}
