namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{IdGatherer}")]
    [DbTable(Name = "Picture")]
    [DbRestictedDml(Restriction.Insert | Restriction.Delete | Restriction.Update)]
    internal class PictureKey : IPictureKey
    {
        //Use for light load
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdGatherer { get; set; }
    }
}
