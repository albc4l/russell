using Newtonsoft.Json;

namespace RussellScreener.Entities {

    /// <summary>
    /// Company information as defined by the IEX API
    /// </summary>
    public class StockResultCompany : BaseJsonObject {

        #region Properties

        [JsonProperty("CEO")]
        public string CEO { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; }

        [JsonProperty("issueType")]
        public string IssueType { get; set; }

        [JsonProperty("sector")]
        public string Sector { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        #endregion Properties

    }
}