
namespace Compression
{
    using System;
    using System.IO;
    using System.Linq;

    using Compression.Transformation;

    public class Compressor
    {
        private static readonly ITransformation[] _algo = { new BurrowsWheeler(), new MoveToFront(), new Huffman() };

        public static Stream Deflate(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            int count;
            byte[] buffer = new byte[BurrowsWheeler.MaxIteration];
            MemoryStream ret = new MemoryStream();
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                byte[] working = new byte[count];
                Array.Copy(buffer, working, count);
                Stream ms = new MemoryStream(working);
                ms = _algo.Aggregate(ms, (current, transformation) => transformation.Transform(current));
                ms.CopyTo(ret);
            }
            ret.Seek(0, SeekOrigin.Begin);
            return ret;
        }
        public static Stream Inflate(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            MemoryStream ret = new MemoryStream();
            while (stream.Length>stream.Position)
            {
                Stream ms = _algo.Reverse().Aggregate(stream, (current, transformation) => transformation.ReverseTransform(current));
                ms.CopyTo(ret);
            }
            ret.Seek(0, SeekOrigin.Begin);
            return ret;
        }
    }
}
