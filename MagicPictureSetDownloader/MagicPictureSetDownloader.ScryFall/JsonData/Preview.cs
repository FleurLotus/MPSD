namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System;
    using System.Text.Json.Serialization;

    internal class Preview : JsonWithExtensionDataBase
    {
        [JsonPropertyName("previewed_at")]
        public DateTime PreviewedAt { get; set; }

        [JsonPropertyName("source_uri")]
        public Uri SourceUri { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}