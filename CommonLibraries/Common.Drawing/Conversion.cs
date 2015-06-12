namespace Common.Drawing
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    public static class Conversion
    {
        public static Image ResizeImage(this Image source, float percent)
        {
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            int newWidth = (int)(sourceWidth * percent);
            int newHeight = (int)(sourceHeight * percent);

            return CreateResizedImage(source, newWidth, newHeight, new Rectangle(0, 0, newWidth, newHeight));
        }
        public static Image ResizeImage(this Image source, int newWidth, int newHeight)
        {

            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int destX = 0, destY = 0;
            float nPercent;

            float nPercentW = ((float)newWidth / sourceWidth);
            float nPercentH = ((float)newHeight / sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            return CreateResizedImage(source, newWidth, newHeight, new Rectangle(destX, destY, destWidth, destHeight));
        }
        private static Image CreateResizedImage(Image source, int newWidth, int newHeight, Rectangle dest)
        {
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            Bitmap outbm = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            outbm.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (Graphics g = Graphics.FromImage(outbm))
            {
                g.Clear(Color.Black);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(source, dest, new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            }
            return outbm;
        }
    }
}
