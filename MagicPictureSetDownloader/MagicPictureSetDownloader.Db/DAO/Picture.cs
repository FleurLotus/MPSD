using System;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DbTable]
    public class Picture
    {
        [DbColumn, DbKeyColumn]
        public int Id { get; set; }
        [DbColumn]
        public Byte[] Data { get; set; }
    }
}
