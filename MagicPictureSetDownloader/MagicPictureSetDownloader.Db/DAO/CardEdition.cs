using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DbTable]
    public class CardEdition
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public int IdRarity { get; set; }
        [DbColumn]
        public string IdGatherer { get; set; }
    }
}
