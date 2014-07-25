namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{IdGatherer}")]
    [DbTable]
    internal class Picture : IPicture
    {
        private byte[] _image;
        [DbColumn, DbKeyColumn(false)]
        public int IdGatherer { get; set; }
        [DbColumn]
        public byte[] Image
        {
            get { return _image == null ? null : (byte[]) _image.Clone(); }
            set { _image = value; }
        }
    }
}
