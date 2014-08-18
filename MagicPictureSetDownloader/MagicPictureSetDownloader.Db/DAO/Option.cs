namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("Type: {Type} - Key: {Key} - Value: {Value}")]
    [DbTable]
    internal class Option : IOption
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public TypeOfOption Type { get; set; }
        [DbColumn]
        public string Key { get; set; }
        [DbColumn]
        public string Value { get; set; }
    }
}
