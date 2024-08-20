using MagicPictureSetDownloader.ScryFall.JsonData;

namespace MagicPictureSetDownloader.ScryFall.JsonLite
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
            SetType = s.SetType;
            IconSvgUri = s.IconSvgUri;

            SetBlock();
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

        [JsonPropertyName("set_type")]
        public SetType SetType { get; set; }

        [JsonPropertyName("icon_svg_uri")]
        public Uri IconSvgUri { get; set; }

        private void SetBlock()
        {
            if (!string.IsNullOrEmpty(Block))
            {
                return;
            }
            Block = SetType switch
            {
                SetType.Alchemy => "Alchemy",
                SetType.Archenemy => "Archenemy",
                SetType.Commander => "Commander",
                SetType.DuelDeck => "Duel Deck",
                SetType.FromTheVault => "From the Vault",
                SetType.Funny => "Fun",
                SetType.Masters => "Masters Edition",
                SetType.Planechase => "Planechase",
                SetType.Vanguard => "Vanguard",
                _ => null,
            };
        }
    }
}
