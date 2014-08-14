namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{IdGatherer}")]
    [DbTable(Name = "Picture")]
    internal class PictureKey : IPictureKey
    {
        //Use for light load
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdGatherer { get; set; }
    }
}
