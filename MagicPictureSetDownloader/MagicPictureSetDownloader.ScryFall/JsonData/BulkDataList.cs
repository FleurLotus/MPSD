namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class BulkDataList : JsonWithExtensionDataBase
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("data")]
        public List<BulkData> Data { get; set; } = new List<BulkData>();
    }
}
