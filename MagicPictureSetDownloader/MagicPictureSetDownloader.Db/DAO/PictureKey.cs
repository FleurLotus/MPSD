namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{IdScryFall}")]
    [DbTable(Name = "Picture")]
    [DbRestictedDml(Restriction.Insert | Restriction.Delete | Restriction.Update)]
    internal class PictureKey : IPictureKey
    {
        //Use for light load
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public string IdScryFall { get; set; }
    }
}
