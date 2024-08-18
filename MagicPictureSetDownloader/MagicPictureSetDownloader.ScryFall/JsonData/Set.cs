namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System;
    using System.Text.Json.Serialization;

    public class Set 
    {
        public Set() 
        { }

        internal Set(FullSet s)
        {
            Id = s.Id;
            Code = s.Code;
            Name = s.Name;
            ReleasedAt = s.ReleasedAt;
            Block = s.Block;
            CardCount = s.CardCount;
            NonFoilOnly = s.NonFoilOnly;
        }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("released_at")]
        public DateTime? ReleasedAt { get; set; }

        [JsonPropertyName("block")]
        public string Block { get; set; }

        [JsonPropertyName("card_count")]
        public int CardCount { get; set; }

        [JsonPropertyName("nonfoil_only")]
        public bool NonFoilOnly { get; set; }
/*
        [JsonPropertyName("icon_svg_uri")]
        public Uri IconSvgUri { get; set; }
*/
    }
}
