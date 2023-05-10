namespace ScryfallTest.Db
{
    using Common.Database;

    [DbTable]
    internal class CardEditionVariation
    {
        [DbColumn]
        public string IdScryfall { get; set; }
        [DbColumn]
        public string OtherIdScryfall { get; set; }
        [DbColumn]
        public int? Part { get; set; }
        [DbColumn]
        public string Url { get; set; }
        [DbColumn]
        public int? IdGatherer { get; set; }
    }
}
