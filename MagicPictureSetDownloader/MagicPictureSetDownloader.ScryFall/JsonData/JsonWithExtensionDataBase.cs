namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System.Collections.Generic;

    using System.Text.Json.Serialization;

    public class JsonWithExtensionDataBase
    {
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}