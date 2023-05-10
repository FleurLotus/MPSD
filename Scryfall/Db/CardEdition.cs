namespace ScryfallTest.Db
{
    using Common.Database;

    [DbTable]
    internal class CardEdition 
    {
        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public int IdRarity { get; set; }
        [DbColumn]
        public string IdScryfall { get; set; }
        [DbColumn]
        public int? IdGatherer { get; set; }
        [DbColumn]
        public string Url { get; set; }
        [DbColumn]
        public int? Part { get; set; }
    }
}
