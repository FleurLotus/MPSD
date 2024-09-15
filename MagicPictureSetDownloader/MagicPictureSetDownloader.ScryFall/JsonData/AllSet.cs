namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System.Text.Json.Serialization;

    internal class AllSet : JsonWithExtensionDataBase
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("data")]
        public FullSet[] Data { get; set; }
    }
}
