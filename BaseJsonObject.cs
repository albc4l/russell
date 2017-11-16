using Newtonsoft.Json;

namespace RussellScreener {
    public class BaseJsonObject {

        #region Fields

        /// <summary>
        /// Settings for Json serialization
        /// </summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Deserialize a json to an object
        /// </summary>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>The deserialized object</returns>
        public static BaseJsonObject FromJson<T>(string json) where T : BaseJsonObject => JsonConvert.DeserializeObject<T>(json, Settings);

        /// <summary>
        /// Serialize an object to Json
        /// </summary>
        /// <typeparam name="T">Type of the input object</typeparam>
        /// <param name="o">Object that must be serialized to Json</param>
        /// <returns>A Json string</returns>
        public static string ToJson<T>(T o) where T : BaseJsonObject => JsonConvert.SerializeObject(o, Settings);

        #endregion Methods

    }
}