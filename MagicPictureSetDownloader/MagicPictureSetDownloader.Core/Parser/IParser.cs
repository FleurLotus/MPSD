namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;

    internal interface IParser<out T>
    {
        IEnumerable<T> Parse(string text);
    }
}
