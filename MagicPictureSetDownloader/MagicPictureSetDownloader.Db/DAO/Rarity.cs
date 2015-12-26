namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Rarity : IRarity
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            Rarity r = obj as Rarity;

            if (r == null)
                return -1;

            return Id.CompareTo(r.Id);
        }
    }
}
