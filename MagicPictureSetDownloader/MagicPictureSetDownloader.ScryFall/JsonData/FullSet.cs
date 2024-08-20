namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using MagicPictureSetDownloader.ScryFall.JsonLite;
    using System;
    using System.Text.Json.Serialization;

    internal class FullSet : JsonWithExtensionDataBase
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("arena_code")]
        public string ArenaCode { get; set; }

        [JsonPropertyName("mtgo_code")]
        public string MtgoCode { get; set; }

        [JsonPropertyName("tcgplayer_id")]
        public int? TcgplayerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("set_type")]
        public string SetType { get; set; }

        [JsonPropertyName("released_at")]
        public DateTime? ReleasedAt { get; set; }

        [JsonPropertyName("block_code")]
        public string BlockCode { get; set; }

        [JsonPropertyName("block")]
        public string Block { get; set; }

        [JsonPropertyName("parent_set_code")]
        public string ParentSetCode { get; set; }

        [JsonPropertyName("card_count")]
        public int CardCount { get; set; }

        [JsonPropertyName("printed_size")]
        public int? PrintedSize { get; set; }

        [JsonPropertyName("digital")]
        public bool Digital { get; set; }

        [JsonPropertyName("foil_only")]
        public bool FoilOnly { get; set; }

        [JsonPropertyName("nonfoil_only")]
        public bool NonFoilOnly { get; set; }

        [JsonPropertyName("scryfall_uri")]
        public Uri ScryfallUri { get; set; }

        [JsonPropertyName("uri")]
        public Uri Uri { get; set; }

        [JsonPropertyName("icon_svg_uri")]
        public Uri IconSvgUri { get; set; }

        [JsonPropertyName("search_uri")]
        public Uri SearchUri{ get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        public Set ToSet()
        {
            return new Set(this);
        }
    }
}
