﻿using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Core.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    class Block
    {
        [DbColumn, DbKeyColumn]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
