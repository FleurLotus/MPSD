namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Block : IBlock
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
