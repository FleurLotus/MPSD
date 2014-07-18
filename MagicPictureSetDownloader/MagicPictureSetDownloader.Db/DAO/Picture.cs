using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DebuggerDisplay("{IdGatherer}")]
    [DbTable]
    internal class Picture : IPicture
    {
        private byte[] _image;
        private byte[] _foilImage;
        [DbColumn, DbKeyColumn(false)]
        public int IdGatherer { get; set; }
        [DbColumn]
        public byte[] Image
        {
            get { return _image == null ? null : (byte[]) _image.Clone(); }
            set { _image = value; }
        }
        [DbColumn]
        public byte[] FoilImage
        {
            get { return _foilImage == null ? null : (byte[]) _foilImage.Clone(); }
            set { _foilImage = value; }
        }
    }
}
