namespace MagicPictureSetDownloader.Db.DAO
{
    using MagicPictureSetDownloader.Interface;

    internal class TreePicture: ITreePicture
    {
        private byte[] _image;
        public string Name { get; set; }
        public string FilePath { get; set; }
        public byte[] Image
        {
            get { return _image == null ? null : (byte[]) _image.Clone(); }
            set { _image = value; }
        }
    }
}
