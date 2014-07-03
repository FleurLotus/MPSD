using System;
using Common.Database;

namespace MagicPictureSetDownloader.Core.Db
{
    [DbTable]
    class Picture
    {
        [DbColumn, DbKeyColumn]
        public int Id { get; set; }
        [DbColumn]
        public Byte[] Data { get; set; }
    }
}
