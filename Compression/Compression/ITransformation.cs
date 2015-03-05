namespace Compression
{
    using System.IO;

    public interface ITransformation
    {
        Stream Transform(Stream source);
        Stream ReverseTransform(Stream source);
    }
}
