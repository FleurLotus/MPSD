
namespace Compression.IO
{
    using System;
    using System.IO;

    public class BitReader
    {
        private readonly Stream _stream;
        private readonly long _length;
        private short _bitIndex;
        private bool[] _temp;

        public BitReader(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                throw new ArgumentNullException("stream");
            _stream = stream;
            _length = _stream.Length;
        }

        public bool EoF 
        {
            get { return _stream.Position == _length && _bitIndex == 0; }
        }

        public byte ReadByte()
        {
            int ret = 0;
            for (int i = 7; i >= 0; i--)
            {
                if (ReadNext())
                    ret |= 1 << i;
            }
            return (byte)ret;
        }
        public bool ReadNext()
        {
            if (EoF)
                throw new IndexOutOfRangeException("No more data to read");

            if (_temp == null)
                _temp = ReadNextByteInternal();

            bool ret = _temp[_bitIndex];

            _bitIndex++;
            if (_bitIndex == 8)
            {
                _bitIndex = 0;
                _temp = null;
            }
            return ret;
        }

        private bool[] ReadNextByteInternal()
        {
            bool[] ret = new bool[8];

            int data = _stream.ReadByte();
            if (data == -1)
                throw new IndexOutOfRangeException("No more data to read");

            for (int i = 0; i < 8; i++)
            {
                ret[7 - i] = (data % 2) == 1;
                data = data >> 1;
            }
            return ret;
        }
    }
}
