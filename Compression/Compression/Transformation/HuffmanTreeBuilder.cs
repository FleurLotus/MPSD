namespace Compression.Transformation
{
    using System.Collections.Generic;
    using System.Linq;

    using Compression.IO;

    internal static class HuffmanTreeBuilder
    {
        public static HuffmanTreeNode Build(BitReader reader)
        {
            return BuildNode(reader);
        }
        public static HuffmanTreeNode Build(int[] freq)
        {
            SortedList<HuffmanTreeNode, object> allnodes = new SortedList<HuffmanTreeNode, object>(new HuffmanTreeNodeComparer());

            for (int i = 0; i < freq.Length; i++)
            {
                if (freq[i] != 0)
                    allnodes.Add(new HuffmanTreeNode((byte)i, (ushort)freq[i]), null);
            }

            if (allnodes.Count == 1)
            {
                HuffmanTreeNode left = allnodes.Keys.First();
                allnodes.Remove(left);
                allnodes.Add(new HuffmanTreeNode(left, new HuffmanTreeNode(left.Code.Value, 0)), null);
            }

            while (allnodes.Count >= 2)
            {
                HuffmanTreeNode left = allnodes.Keys.First();
                allnodes.Remove(left);
                HuffmanTreeNode right = allnodes.Keys.First();
                allnodes.Remove(right);

                allnodes.Add(new HuffmanTreeNode(left, right), null);
            }

            return allnodes.Keys.FirstOrDefault();
        }

        private static HuffmanTreeNode BuildNode(BitReader reader)
        {
            if (reader.ReadNext())
                return new HuffmanTreeNode(reader.ReadByte(), 1);

            HuffmanTreeNode node = new HuffmanTreeNode();
            node.AddLeftChild(BuildNode(reader));
            node.AddRightChild(BuildNode(reader));
            return node;
        }
    }
}
