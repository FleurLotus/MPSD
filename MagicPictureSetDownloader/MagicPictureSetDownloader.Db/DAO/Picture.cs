namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{IdGatherer}")]
    [DbTable]
    internal class Picture : PictureKey, IPicture
    {
        private byte[] _image;
        [DbColumn]
        public byte[] Image
        {
            get { return _image == null ? null : (byte[]) _image.Clone(); }
            set { _image = value; }
        }
    }
}
