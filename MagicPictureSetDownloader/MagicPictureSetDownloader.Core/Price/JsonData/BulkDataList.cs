namespace MagicPictureSetDownloader.Core.JsonData
{
    using Newtonsoft.Json;

    using System;
    using System.Collections.Generic;
    public class BulkData
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("compressed_size")]
        public int CompressedSize { get; set; }

        [JsonProperty("download_uri")]
        public string DownloadUri { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("content_encoding")]
        public string ContentEncoding { get; set; }
    }

    public class BulkDataList
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public List<BulkData> Data { get; } = new List<BulkData>();
    }
}
