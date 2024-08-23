using MagicPictureSetDownloader.ScryFall.JsonData;

namespace MagicPictureSetDownloader.ScryFall.JsonLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    public class Card : CardFace
    {
        public Card()
        { }

        internal Card(FullCard c) : base(c)
        {
            Id = c.Id;
            MtgoId= c.MtgoId;
            MtgoFoilId= c.MtgoFoilId;
            MultiverseIds = c.MultiverseIds.ToList();
            TcgplayerId= c.TcgplayerId;
            TcgplayerEtchedId = c.TcgplayerEtchedId;
            CardmarketId = c.CardmarketId;
            CardFaces = c.CardFaces.Select(cf => new CardFace(cf)).ToList();
            FrameEffects = c.FrameEffects.ToList();
            NonFoil = c.NonFoil;
            Rarity = c.Rarity;
            SetId = c.SetId;
            Set = c.Set;
        }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("mtgo_id")]
        public int? MtgoId { get; set; }

        [JsonPropertyName("mtgo_foil_id")]
        public int? MtgoFoilId { get; set; }

        [JsonPropertyName("multiverse_ids")]
        public List<int> MultiverseIds { get; set; } = new List<int>();

        [JsonPropertyName("tcgplayer_id")]
        public int? TcgplayerId { get; set; }

        [JsonPropertyName("tcgplayer_etched_id")]
        public int? TcgplayerEtchedId { get; set; }

        [JsonPropertyName("cardmarket_id")]
        public int? CardmarketId { get; set; }

        [JsonPropertyName("card_faces")]
        public List<CardFace> CardFaces { get; set; } = new List<CardFace>();

        [JsonPropertyName("frame_effects")]
        public List<FrameEffect> FrameEffects { get; set; } = new List<FrameEffect>();

        [JsonPropertyName("nonfoil")]
        public bool NonFoil { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("set_id")]
        public string SetId { get; set; }

        [JsonPropertyName("set")]
        public string Set { get; set; }


    }
}
