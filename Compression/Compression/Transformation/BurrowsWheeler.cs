
namespace Compression.Transformation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    
    internal class BurrowsWheeler : ITransformation
    {
        public const ushort MaxIteration = 65000;

        public Stream Transform(Stream source)
        {
            if (source == null)
                return null;

            int len = (int)source.Length;
            byte[] input = new byte[source.Length];
            source.Read(input, 0, len);

            if (len == 0)
                return new MemoryStream();

            if (len > MaxIteration)
                throw new IndexOutOfRangeException("Can't manage data longer than " + MaxIteration);

            List<CircularData> dataToSort = new List<CircularData>();
            for (int i = 0; i < len; i++)
                dataToSort.Add(new CircularData(input, i));

            dataToSort.Sort();

            List<byte> ret = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                CircularData circularData = dataToSort[i];
                ret.Add(circularData[len - 1]);
                if (circularData.StartIndex == 0)
                {
                    byte[] index =  BitConverter.GetBytes((ushort)i);

                    ret.InsertRange(0, index);
                }
            }

            return new MemoryStream(ret.ToArray());
        }
        public Stream ReverseTransform(Stream source)
        {
            if (source == null)
                return null;

            int len = (int)source.Length;
            byte[] input = new byte[source.Length];
            source.Read(input, 0, len);

            if (len == 0)
                return new MemoryStream();

            if (len - 2 > MaxIteration)
                throw new IndexOutOfRangeException("Can't manage data longer than " + MaxIteration);
            if (len <= 2)
                throw new ArgumentOutOfRangeException("source", "Input is too short to be decoded");

            ushort index = BitConverter.ToUInt16(input, 0);

            if (index >= len - 2)
                throw new WrongFormattedInputException("index is longer than source");

            byte[] data = new byte[len - 2];
            Array.Copy(input, 2, data, 0, data.Length);

            List<byte> sorted = new List<byte>(data);
            sorted.Sort();

            ushort[] next = GenerateNext(sorted, data);

            byte[] ret = new byte[len - 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = sorted[index];
                index = next[index];
            }

            return new MemoryStream(ret);
        }

        private ushort[] GenerateNext(List<byte> sorted, byte[] data)
        {
            ushort[] ret = new ushort[data.Length];

            for (int i = 0; i < sorted.Count; i++)
            {
                byte current = sorted[i];
                int previousindex = -1;

                //Same value as previous
                if (i != 0 && sorted[i - 1] == current)
                    previousindex = ret[i - 1];
                
                for (int j = previousindex + 1; j < data.Length; j++)
                {
                    if (data[j] == current)
                    {
                        ret[i] = (ushort)j;
                        break;
                    }
                }
            }
            
            return ret;
        }
    }
}
