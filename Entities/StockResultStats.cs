using Newtonsoft.Json;

namespace RussellScreener.Entities {

    /// <summary>
    /// Company statistics/metrics as defined by the IEX API
    /// </summary>
    public class StockResultStats : BaseJsonObject {

        #region Properties

        [JsonProperty("beta")]
        public double? Beta { get; set; }

        [JsonProperty("cash")]
        public long? Cash { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("consensusEPS")]
        public double? ConsensusEPS { get; set; }

        [JsonProperty("day200MovingAvg")]
        public double? Day200MovingAvg { get; set; }

        [JsonProperty("day50MovingAvg")]
        public double? Day50MovingAvg { get; set; }

        [JsonProperty("day5ChangePercent")]
        public double? Day5ChangePercent { get; set; }

        [JsonProperty("debt")]
        public long? Debt { get; set; }

        [JsonProperty("dividendRate")]
        public double? DividendRate { get; set; }

        [JsonProperty("dividendYield")]
        public double? DividendYield { get; set; }

        [JsonProperty("EBITDA")]
        public long? EBITDA { get; set; }

        [JsonProperty("EPSSurpriseDollar")]
        public object EPSSurpriseDollar { get; set; }

        [JsonProperty("EPSSurprisePercent")]
        public double? EPSSurprisePercent { get; set; }

        [JsonProperty("exDividendDate")]
        public string ExDividendDate { get; set; }

        [JsonProperty("float")]
        public long? Float { get; set; }

        [JsonProperty("grossProfit")]
        public long? GrossProfit { get; set; }

        [JsonProperty("insiderPercent")]
        public double? InsiderPercent { get; set; }

        [JsonProperty("institutionPercent")]
        public double? InstitutionPercent { get; set; }

        [JsonProperty("latestEPS")]
        public double? LatestEPS { get; set; }

        [JsonProperty("latestEPSDate")]
        public string LatestEPSDate { get; set; }

        [JsonProperty("marketcap")]
        public long? Marketcap { get; set; }

        [JsonProperty("month1ChangePercent")]
        public double? Month1ChangePercent { get; set; }

        [JsonProperty("month3ChangePercent")]
        public double? Month3ChangePercent { get; set; }

        [JsonProperty("month6ChangePercent")]
        public double? Month6ChangePercent { get; set; }

        [JsonProperty("numberOfEstimates")]
        public long? NumberOfEstimates { get; set; }

        [JsonProperty("peRatioHigh")]
        public double? PeRatioHigh { get; set; }

        [JsonProperty("peRatioLow")]
        public double? PeRatioLow { get; set; }

        [JsonProperty("priceToBook")]
        public double? PriceToBook { get; set; }

        [JsonProperty("priceToSales")]
        public double? PriceToSales { get; set; }

        [JsonProperty("profitMargin")]
        public double? ProfitMargin { get; set; }

        [JsonProperty("returnOnAssets")]
        public double? ReturnOnAssets { get; set; }

        [JsonProperty("returnOnCapital")]
        public object ReturnOnCapital { get; set; }

        [JsonProperty("returnOnEquity")]
        public double? ReturnOnEquity { get; set; }

        [JsonProperty("revenue")]
        public long? Revenue { get; set; }

        [JsonProperty("revenuePerEmployee")]
        public long? RevenuePerEmployee { get; set; }

        [JsonProperty("revenuePerShare")]
        public long? RevenuePerShare { get; set; }

        [JsonProperty("sharesOutstanding")]
        public long? SharesOutstanding { get; set; }

        [JsonProperty("shortDate")]
        public string ShortDate { get; set; }

        [JsonProperty("shortInterest")]
        public long? ShortInterest { get; set; }

        [JsonProperty("shortRatio")]
        public double? ShortRatio { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ttmEPS")]
        public double? TtmEPS { get; set; }

        [JsonProperty("week52change")]
        public double? Week52change { get; set; }

        [JsonProperty("week52high")]
        public double? Week52high { get; set; }

        [JsonProperty("week52low")]
        public double? Week52low { get; set; }

        [JsonProperty("year1ChangePercent")]
        public double? Year1ChangePercent { get; set; }

        [JsonProperty("year2ChangePercent")]
        public double? Year2ChangePercent { get; set; }

        [JsonProperty("year5ChangePercent")]
        public double? Year5ChangePercent { get; set; }

        [JsonProperty("ytdChangePercent")]
        public double? YtdChangePercent { get; set; }

        #endregion Properties


    }
}
