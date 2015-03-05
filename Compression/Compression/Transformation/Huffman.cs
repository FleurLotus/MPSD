
namespace Compression.Transformation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Compression.IO;

    internal class Huffman : ITransformation
    {
        public Stream Transform(Stream source)
        {
            if (source == null)
                return null;

            int len = (int)(source.Length - source.Position);

            if (len == 0)
                return new MemoryStream();

            byte[] input = new byte[len];
            source.Read(input, (int)source.Position, len);

            int[] freq = GetFrequency(input);
            HuffmanTreeNode root = HuffmanTreeBuilder.Build(freq);

            BitWriter bitWriter = new BitWriter();
            //Compressed
            bitWriter.Write(1);
            //Write total
            bitWriter.Write(BitConverter.GetBytes((ushort)freq.Sum()));
            //Write tree
            SaverVisitor saverVisitor = new SaverVisitor(bitWriter);
            root.AcceptVisitor(saverVisitor);

            if (bitWriter.GetValues().Length < len)
            {
                //Create transcoding table
                TranscoderVisitor transcoderVisitor = new TranscoderVisitor();
                root.AcceptVisitor(transcoderVisitor);
                IDictionary<byte, bool[]> transco = transcoderVisitor.Value;
                //Write result
                foreach (byte b in input)
                    bitWriter.Write(transco[b]);

                byte[] ret = bitWriter.GetValues();
                if (ret.Length < len)
                    return new MemoryStream(ret);
            }

            bitWriter = new BitWriter();
            //not compressed
            bitWriter.Write(0);
            //Write total
            bitWriter.Write(BitConverter.GetBytes((ushort)len));
            //not modify
            bitWriter.Write(input);

            return new MemoryStream(bitWriter.GetValues());
        }
        public Stream ReverseTransform(Stream source)
        {
            if (source == null)
                return null;

            int len = (int)(source.Length - source.Position);;

            if (len == 0)
                return new MemoryStream();

            if (len <= 3)
                throw new ArgumentOutOfRangeException("source", "Input is too short to be decoded");

            BitReader bitReader = new BitReader(source);
            byte compressed = bitReader.ReadByte();
            byte[] b = { bitReader.ReadByte(), bitReader.ReadByte() };

            int count = BitConverter.ToUInt16(b, 0);
            List<byte> ret = new List<byte>(); 
            if (compressed == 0)
            {
                for (int i = 0; i < count; i++)
                    ret.Add(bitReader.ReadByte());
                return new MemoryStream(ret.ToArray());
            }

            HuffmanTreeNode root = HuffmanTreeBuilder.Build(bitReader);
            DecoderVisitor decoderVisitor = new DecoderVisitor(bitReader);

            for (int i = 0; i < count; i++)
            {
                root.AcceptVisitor(decoderVisitor);
                ret.Add(decoderVisitor.Value);
            }
            return new MemoryStream(ret.ToArray());
        }

        private int[] GetFrequency(byte[] input)
        {
            int[] freq = new int[256];

            foreach (byte b in input)
                freq[b]++;

            return freq;
        }
    }
}
