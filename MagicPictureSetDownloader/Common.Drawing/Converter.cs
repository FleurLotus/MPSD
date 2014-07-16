using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Common.Drawing
{
    public static class Converter
    {
        public static byte[] ImageToBytes(Image img)
        {
            MemoryStream memoryStream = new MemoryStream();
            img.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream.GetBuffer();
        }
        public static Image BytesToImage(byte[] bytes)
        {
            return Image.FromStream(new MemoryStream(bytes), true);
        }
    }
}
