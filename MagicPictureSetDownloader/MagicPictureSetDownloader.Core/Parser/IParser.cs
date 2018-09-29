namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;

    public interface IParser<out T>
    {
        IEnumerable<T> Parse(string text);
    }
}
