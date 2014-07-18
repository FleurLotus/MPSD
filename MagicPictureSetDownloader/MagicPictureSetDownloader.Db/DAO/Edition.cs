using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Edition : IEdition
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }
        [DbColumn]
        public int? IdBlock { get; set; }
        public string BlockName { get; set; }
        [DbColumn]
        public int? BlockPosition { get; set; }
        [DbColumn]
        public string GathererName { get; set; }
    }
}
