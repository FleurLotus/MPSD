namespace Compression.Transformation
{
    using System;

    internal class HuffmanTreeNode: IVisitable
    {
        public HuffmanTreeNode()
        {
        }

        public HuffmanTreeNode(HuffmanTreeNode leftChild, HuffmanTreeNode rightChild)
        {
            if (leftChild == null)
                throw new ArgumentNullException("leftChild");
            if (rightChild == null)
                throw new ArgumentNullException("rightChild");

            LeftChild = leftChild;
            RightChild = rightChild;

            MinCode = Math.Min(leftChild.MinCode, rightChild.MinCode);
            Frequency = (ushort)(leftChild.Frequency + rightChild.Frequency);
        }
        public HuffmanTreeNode(byte code, ushort frequency)
        {
            MinCode = code;
            Code = code;
            Frequency = frequency;
        }
        public byte? Code { get; private set; }
        public byte MinCode { get; private set; }
        public ushort Frequency { get; private set; }
        public HuffmanTreeNode LeftChild { get; private set; }
        public HuffmanTreeNode RightChild { get; private set; }
        public bool IsLeaf
        {
            get { return Code.HasValue; }
        }

        public void AddLeftChild(HuffmanTreeNode left)
        {
            if (left == null)
                throw new ArgumentNullException("left");

            if (LeftChild != null)
                throw new ArgumentException("LeftChild is already set");

            if (IsLeaf)
                throw new ArgumentException("Leaf can't have child");

            LeftChild = left;
        }
        public void AddRightChild(HuffmanTreeNode right)
        {
            if (right == null)
                throw new ArgumentNullException("right");

            if (RightChild != null)
                throw new ArgumentException("RightChild is already set");

            if (IsLeaf)
                throw new ArgumentException("Leaf can't have child");

            RightChild = right;
        }

        public void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this);
        }
        public override string ToString()
        {
            if (IsLeaf)
                return string.Format("Leaf Code={0} Freq={1}", Code, Frequency);
            
            return string.Format("Intermediary Freq={0} MinCode={1}", Frequency, MinCode);
        }
    }
}
