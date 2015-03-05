namespace Compression.Transformation
{
    using System.Collections.Generic;

    internal class HuffmanTreeNodeComparer: IComparer<HuffmanTreeNode>
    {
        public int Compare(HuffmanTreeNode x, HuffmanTreeNode y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            int comp = x.Frequency.CompareTo(y.Frequency);
            if (comp != 0)
                return comp;

            return x.MinCode.CompareTo(y.MinCode);
        }
    }
}
