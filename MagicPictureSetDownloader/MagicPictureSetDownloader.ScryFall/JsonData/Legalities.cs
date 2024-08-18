namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System.Text.Json.Serialization;

    public class Legalities : JsonWithExtensionDataBase
    {
        [JsonPropertyName("alchemy")]
        public Legality Alchemy { get; set; }

        [JsonPropertyName("brawl")]
        public Legality Brawl { get; set; }

        [JsonPropertyName("commander")]
        public Legality Commander { get; set; }

        [JsonPropertyName("duel")]
        public Legality Duel { get; set; }

        [JsonPropertyName("explorer")]
        public Legality Explorer { get; set; }

        [JsonPropertyName("future")]
        public Legality Future { get; set; }

        [JsonPropertyName("gladiator")]
        public Legality Gladiator { get; set; }

        [JsonPropertyName("historic")]
        public Legality Historic { get; set; }

        [JsonPropertyName("historicbrawl")]
        public Legality HistoricBrawl { get; set; }

        [JsonPropertyName("legacy")]
        public Legality Legacy { get; set; }

        [JsonPropertyName("modern")]
        public Legality Modern { get; set; }

        [JsonPropertyName("oathbreaker")]
        public Legality OathBreaker { get; set; }

        [JsonPropertyName("oldschool")]
        public Legality OldSchool { get; set; }

        [JsonPropertyName("pauper")]
        public Legality Pauper { get; set; }

        [JsonPropertyName("paupercommander")]
        public Legality PauperCommander { get; set; }

        [JsonPropertyName("penny")]
        public Legality Penny { get; set; }

        [JsonPropertyName("pioneer")]
        public Legality Pioneer { get; set; }

        [JsonPropertyName("predh")]
        public Legality Predh { get; set; }

        [JsonPropertyName("premodern")]
        public Legality PreModern { get; set; }

        [JsonPropertyName("standard")]
        public Legality Standard { get; set; }

        [JsonPropertyName("standardbrawl")]
        public Legality StandardBrawl { get; set; }

        [JsonPropertyName("timeless")]
        public Legality Timeless { get; set; }

        [JsonPropertyName("vintage")]
        public Legality Vintage { get; set; }
    }
}