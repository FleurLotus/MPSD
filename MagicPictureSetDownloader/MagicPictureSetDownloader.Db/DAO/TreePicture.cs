namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class TreePicture: ITreePicture
    {
        private byte[] _image;
        [DbColumn, DbKeyColumn(false)]
        public string Name { get; set; }
        [DbColumn]
        public byte[] Image
        {
            get { return _image == null ? null : (byte[]) _image.Clone(); }
            set { _image = value; }
        }
    }
}
