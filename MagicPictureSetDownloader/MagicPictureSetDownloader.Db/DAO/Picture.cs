using System;
using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DebuggerDisplay("{IdGatherer}")]
    [DbTable]
    public class Picture
    {
        [DbColumn, DbKeyColumn(false)]
        public int IdGatherer { get; set; }
        [DbColumn]
        public Byte[] Image { get; set; }
        [DbColumn]
        public Byte[] FoilImage { get; set; }
    }
}
