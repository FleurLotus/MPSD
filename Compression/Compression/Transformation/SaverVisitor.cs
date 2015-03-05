namespace Compression.Transformation
{
    using Compression.IO;

    internal class SaverVisitor : IVisitor
    {
        private readonly BitWriter _writer;

        public SaverVisitor(BitWriter writer)
        {
            _writer = writer;
        }
        public void Visit(HuffmanTreeNode node)
        {
            if (node.IsLeaf)
            {
                _writer.Write(true);
                _writer.Write(node.Code.Value);
                return;
            }

            _writer.Write(false);

            if (node.LeftChild != null)
            {
                node.LeftChild.AcceptVisitor(this);
            }

            if (node.RightChild != null)
            {
                node.RightChild.AcceptVisitor(this);
            }
        }
    }
}
