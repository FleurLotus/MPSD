namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICardCount: IEnumerable<KeyValuePair<ICardCountKey, int>>
    {
        int GetTotalCount();
        int GetCount(ICardCountKey key);
        void Add(ICardCountKey key, int count);
    }
}
