namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System;
    using System.Text.Json.Serialization;

    public class RelatedUris : JsonWithExtensionDataBase
    {
        [JsonPropertyName("gatherer")]
        public Uri Gatherer { get; set; }

        [JsonPropertyName("tcgplayer_infinite_articles")]
        public Uri TcgplayerInfiniteArticles { get; set; }

        [JsonPropertyName("tcgplayer_infinite_decks")]
        public Uri TcgplayerInfiniteDecks { get; set; }

        [JsonPropertyName("edhrec")]
        public Uri Edhrec { get; set; }
    }
}
