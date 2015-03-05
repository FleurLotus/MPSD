namespace Compression
{
    using Compression.Transformation;

    internal interface IVisitor
    {
        void Visit(HuffmanTreeNode node);
    }
}
