namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System;
    using System.Text.Json.Serialization;

    public class RelativeCardObject : JsonWithExtensionDataBase
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("component")]
        public Component Component { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type_line")]
        public string TypeLine { get; set; }

        [JsonPropertyName("uri")]
        public Uri Uri { get; set; }
    }
}