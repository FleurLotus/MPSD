using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    public class Edition
    {
        [DbColumn, DbKeyColumn]
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
