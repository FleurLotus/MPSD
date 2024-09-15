namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System;
    using System.Text.Json.Serialization;

    public class ImageUris : JsonWithExtensionDataBase
    {
        [JsonPropertyName("art_crop")]
        public Uri ArtCrop { get; set; }

        [JsonPropertyName("border_crop")]
        public Uri BorderCrop { get; set; }

        [JsonPropertyName("large")]
        public Uri Large { get; set; }

        [JsonPropertyName("normal")]
        public Uri Normal { get; set; }

        [JsonPropertyName("png")]
        public Uri Png { get; set; }

        [JsonPropertyName("small")]
        public Uri Small { get; set; }
    }
}
