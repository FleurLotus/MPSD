namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using MagicPictureSetDownloader.ScryFall.JsonLite;
    using System;
    using System.Collections.Generic;

    using System.Text.Json.Serialization;

    internal class FullCard : FullCardFace
    {
        #region Core Field
        [JsonPropertyName("arena_id")]
        public int? ArenaId { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("lang")]
        public string Language { get; set; }

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

        [JsonPropertyName("prints_search_uri")]
        public Uri PrintsSearchUri { get; set; }

        [JsonPropertyName("rulings_uri")]
        public Uri RulingsUri { get; set; }

        [JsonPropertyName("scryfall_uri")]
        public Uri ScryfallUri { get; set; }

        [JsonPropertyName("uri")]
        public Uri Uri { get; set; }
        #endregion 

        #region Gameplay
        [JsonPropertyName("all_parts")]
        public List<RelativeCardObject> AllParts { get; set; } = new List<RelativeCardObject>();

        [JsonPropertyName("card_faces")]
        public List<FullCardFace> CardFaces { get; set; } = new List<FullCardFace>();

        [JsonPropertyName("color_identity")]
        public List<Color> ColorIdentity { get; set; } = new List<Color>();

        [JsonPropertyName("edhrec_rank")]
        public int? EdhrecRank { get; set; }

        [JsonPropertyName("hand_modifier")]
        public string HandModifier { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new List<string>();

        [JsonPropertyName("legalities")]
        public Legalities Legalities { get; set; }

        [JsonPropertyName("life_modifier")]
        public string LifeModifier { get; set; }

        [JsonPropertyName("oversized")]
        public bool Oversized { get; set; }

        [JsonPropertyName("penny_rank")]
        public int? PennyRank { get; set; }

        [JsonPropertyName("produced_mana")]
        public List<string> ProducedMana { get; set; } = new List<string>();

        [JsonPropertyName("reserved")]
        public bool Reserved { get; set; }

        #endregion

        #region Print Field
        [JsonPropertyName("artist_ids")]
        public List<Guid> ArtistIds { get; set; } = new List<Guid>();

        [JsonPropertyName("attraction_lights")]
        public List<int> AttractionLights { get; set; } = new List<int>();

        [JsonPropertyName("booster")]
        public bool Booster { get; set; }

        [JsonPropertyName("border_color")]
        public BorderColor BorderColor { get; set; }

        [JsonPropertyName("card_back_id")]
        public Guid CardBackId { get; set; }

        [JsonPropertyName("collector_number")]
        public string CollectorNumber { get; set; }

        [JsonPropertyName("content_warning")]
        public bool? ContentWarning { get; set; }

        [JsonPropertyName("digital")]
        public bool Digital { get; set; }

        [JsonPropertyName("finishes")]
        public List<Finish> Finishes { get; set; } = new List<Finish>();

        [JsonPropertyName("foil")]
        public bool Foil { get; set; }

        [JsonPropertyName("frame_effects")]
        public List<FrameEffect> FrameEffects { get; set; } = new List<FrameEffect>();

        [JsonPropertyName("frame")]
        public Frame Frame { get; set; }

        [JsonPropertyName("full_art")]
        public bool FullArt { get; set; }

        [JsonPropertyName("games")]
        public List<Game> Games { get; set; } = new List<Game>();

        [JsonPropertyName("highres_image")]
        public bool HighResImage { get; set; }

        [JsonPropertyName("image_status")]
        public ImageStatus ImageStatus { get; set; }

        [JsonPropertyName("nonfoil")]
        public bool NonFoil { get; set; }

        [JsonPropertyName("prices")]
        public Prices Prices { get; set; }

        [JsonPropertyName("promo")]
        public bool Promo { get; set; }

        [JsonPropertyName("promo_types")]
        public List<string> PromoTypes { get; set; } = new List<string>();

        [JsonPropertyName("purchase_uris")]
        public object PurchaseUris { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("related_uris")]
        public RelatedUris RelatedUris { get; set; }

        [JsonPropertyName("released_at")]
        public DateTime ReleasedAt { get; set; }

        [JsonPropertyName("reprint")]
        public bool Reprint { get; set; }

        [JsonPropertyName("scryfall_set_uri")]
        public Uri ScryfallSetUri { get; set; }

        [JsonPropertyName("set_name")]
        public string SetName { get; set; }

        [JsonPropertyName("set_search_uri")]
        public Uri SetSearchUri { get; set; }

        [JsonPropertyName("set_type")]
        public string SetType { get; set; }

        [JsonPropertyName("set_uri")]
        public Uri SetUri { get; set; }

        [JsonPropertyName("set")]
        public string Set { get; set; }

        [JsonPropertyName("set_id")]
        public string SetId { get; set; }

        [JsonPropertyName("story_spotlight")]
        public bool StorySpotlight { get; set; }

        [JsonPropertyName("textless")]
        public bool Textless { get; set; }

        [JsonPropertyName("variation")]
        public bool Variation { get; set; }

        [JsonPropertyName("variation_of")]
        public Guid? VariationOf { get; set; }

        [JsonPropertyName("security_stamp")]
        public string SecurityStamp { get; set; }

        [JsonPropertyName("preview")]
        public Preview Preview { get; set; }
        #endregion

        public Card ToCard()
        {
            return new Card(this);
        }
    }
}
