namespace Compression.Transformation
{

    using Compression.IO;

    internal class DecoderVisitor : IVisitor
    {
        private readonly BitReader _reader;

        public DecoderVisitor(BitReader reader)
        {
            _reader = reader;
        }

        public byte Value { get; private set; }

        public void Visit(HuffmanTreeNode node)
        {
            if (node.IsLeaf)
            {
                Value = node.Code.Value;
                return;
            }

            if (_reader.ReadNext())
            {
                node.RightChild.AcceptVisitor(this);
            }
            else
            {
                node.LeftChild.AcceptVisitor(this);
            }
        }
    }
}
