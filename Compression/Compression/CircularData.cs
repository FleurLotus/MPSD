namespace Compression
{
    using System;

    internal class CircularData : IComparable<CircularData>
    {
        private readonly byte[] _data;

        public CircularData(byte[] data, int startIndex)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");

            if (startIndex < 0 || startIndex >= data.Length)
                throw new IndexOutOfRangeException("startIndex is out of range");

            _data = data;
            StartIndex = startIndex;
            Length = _data.Length;
        }

        public int StartIndex { get; private set; }
        public int Length { get; private set; }

        public byte this[int index]
        {
            get {

                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("index is out of range");
                
                int realindex = index + StartIndex;
                if (realindex >= Length)
                    realindex -= Length;

                return _data[realindex];
            }
        }

        public int CompareTo(CircularData other)
        {
            if (this == other)
                return 0;

            if (other == null)
                return -1;

            int len = Math.Min(Length, other.Length);

            for (int i = 0; i < len; i++)
            {
                int comp = this[i].CompareTo(other[i]);
                if (comp != 0)
                    return comp;
            }

            return Length.CompareTo(other.Length);
        }
    }
}