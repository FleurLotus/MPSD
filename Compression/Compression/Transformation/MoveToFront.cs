
namespace Compression.Transformation
{
    using System.Collections.Generic;
    using System.IO;

    internal class MoveToFront : ITransformation
    {
        public Stream Transform(Stream source)
        {
            if (source == null)
                return null;

            List<byte> reference = GenerateReference();
            MemoryStream ret = new MemoryStream();
            
            int r;
            while ((r = source.ReadByte())!=-1)
            {
                byte b = (byte)r;
                for (int i = 0; i < reference.Count; i++)
                {
                    if (reference[i] == b)
                    {
                        ret.WriteByte((byte)i);
                        if (i != 0)
                        {
                            reference.RemoveAt(i);
                            reference.Insert(0, b);
                        }

                        break;
                    }
                }
            }
            ret.Seek(0, SeekOrigin.Begin);
            return ret;
        }
        public Stream ReverseTransform(Stream source)
        {
            if (source == null)
                return null;

            List<byte> reference = GenerateReference();
            MemoryStream ret = new MemoryStream();

            int r;
            while ((r = source.ReadByte())!=-1)
            {
                byte b = (byte)r;
                byte value = reference[b];
                reference.RemoveAt(b);
                reference.Insert(0, value);
                ret.WriteByte(value);
            }
            ret.Seek(0, SeekOrigin.Begin);
            return ret;
        }

        private List<byte> GenerateReference()
        {
            List<byte> ret = new List<byte>(256);
            for (int i = 0; i < 256; i++)
                ret.Add((byte)i);

            return ret;
        }

    }
}
