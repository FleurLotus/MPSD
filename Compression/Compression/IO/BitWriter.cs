
namespace Compression.IO
{
    using System;
    using System.Collections.Generic;

    public class BitWriter
    {
        private readonly List<byte> _stream;
        private byte _bitIndex;
        private byte _currentByte;

        public BitWriter()
        {
            _stream = new List<byte>();
            _bitIndex = 8;
            _currentByte = 0;
        }

        public int Length 
        {
            get { return _stream.Count * 8 + 8 - _bitIndex; }
        }
        public void Write(bool[] input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentNullException("input");

            foreach (bool b in input)
                Write(b);
        }
        public void Write(byte[] input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentNullException("input");

            foreach (byte b in input)
                Write(b);
        }
        public void Write(byte input)
        {
            if (_bitIndex == 8)
            {
                _stream.Add(input);
                return;
            }
            if (_bitIndex == 0)
            {
                _stream.Add(_currentByte);
                _currentByte = input;
                return;
            }

            _currentByte = (byte)(_currentByte | (input >> (8 - _bitIndex)));
            _stream.Add(_currentByte);
            _currentByte = (byte)(input << _bitIndex);
        }
        public void Write(bool input)
        {
            if (_bitIndex == 0)
            {
                _bitIndex = 8;
                _stream.Add(_currentByte);
                _currentByte = 0;
            }
            
            _bitIndex --;
            if (input)
                _currentByte |= (byte) (1 << _bitIndex);
        }
        public byte[] GetValues()
        {
            List<byte> ret = new List<byte>(_stream);

            if (_bitIndex != 8)
                ret.Add(_currentByte);

            return ret.ToArray();
        }
    }
}
