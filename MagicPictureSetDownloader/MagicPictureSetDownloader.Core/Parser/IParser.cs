using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core
{
    interface IParser<out T>
    {
        IEnumerable<T> Parse(string text);
    }
}
