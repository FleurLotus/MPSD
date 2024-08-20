using MagicPictureSetDownloader.ScryFall.JsonData;

namespace MagicPictureSetDownloader.ScryFall.JsonLite
{
    using System;
    using System.Text.Json.Serialization;

    public class CardFace
    {
        public CardFace()
        { }

        internal CardFace(FullCardFace cf)
        {
            Defense = cf.Defense;
            ImageUris = cf.ImageUris == null ? null : new ImageUris { ArtCrop = cf.ImageUris.ArtCrop, BorderCrop = cf.ImageUris.BorderCrop, Large = cf.ImageUris.Large, Normal = cf.ImageUris.Normal, Png = cf.ImageUris.Png, Small = cf.ImageUris.Small };
            Layout = cf.Layout;
            Loyalty = cf.Loyalty;
            ManaCost = cf.ManaCost;
            Name = cf.Name;
            OracleId = cf.OracleId;
            OracleText = cf.OracleText;
            Power = cf.Power;
            Toughness = cf.Toughness;
            TypeLine = cf.TypeLine;
        }

        [JsonPropertyName("defense")]
        public string Defense { get; set; }

        [JsonPropertyName("image_uris")]
        public ImageUris ImageUris { get; set; }

        [JsonPropertyName("layout")]
        public Layout Layout { get; set; }

        [JsonPropertyName("loyalty")]
        public string Loyalty { get; set; }

        [JsonPropertyName("mana_cost")]
        public string ManaCost { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("oracle_id")]
        public Guid? OracleId { get; set; }

        [JsonPropertyName("oracle_text")]
        public string OracleText { get; set; }

        [JsonPropertyName("power")]
        public string Power { get; set; }

        [JsonPropertyName("toughness")]
        public string Toughness { get; set; }

        [JsonPropertyName("type_line")]
        public string TypeLine { get; set; }
    }
}