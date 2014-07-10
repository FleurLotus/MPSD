using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core
{
    internal interface IParser<out T>
    {
        IEnumerable<T> Parse(string text);
    }
}
