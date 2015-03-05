
namespace Compression
{
    internal interface IVisitable
    {
        void AcceptVisitor(IVisitor visitor);
    }
}
