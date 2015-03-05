namespace Compression.Transformation
{
    using System.Collections.Generic;

    internal class TranscoderVisitor : IVisitor
    {
        private readonly IDictionary<byte, bool[]> _transco = new Dictionary<byte, bool[]>();
        private readonly List<bool> _currentPath = new List<bool>();

        public IDictionary<byte, bool[]> Value
        {
            get { return new Dictionary<byte, bool[]>(_transco); }
        }

        public void Visit(HuffmanTreeNode node)
        {
            if (node.IsLeaf)
            {
                if (node.Frequency != 0)
                    _transco.Add(node.Code.Value, _currentPath.ToArray());

                return;
            }

            if (node.LeftChild != null)
            {
                _currentPath.Add(false);
                node.LeftChild.AcceptVisitor(this);
                _currentPath.RemoveAt(_currentPath.Count - 1);
            }

            if (node.RightChild != null)
            {
                _currentPath.Add(true);
                node.RightChild.AcceptVisitor(this);
                _currentPath.RemoveAt(_currentPath.Count - 1);
            }
        }
    }
}
