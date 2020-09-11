namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using MagicPictureSetDownloader.Interface;

    public class CardCount: ICardCount
    {
        private readonly IDictionary<ICardCountKey, int> _counts = new Dictionary<ICardCountKey, int>();

        public CardCount()
        {

        }
        public CardCount(ICardCount toCopy)
        {
            if (toCopy == null)
            {
                throw new ArgumentNullException(nameof(toCopy));
            }

            foreach (KeyValuePair<ICardCountKey, int> kv in toCopy)
            {
                Add(kv.Key, kv.Value);
            }
        }
        public void Add(ICardCountKey key, int count)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (count == 0)
            {
                return;
            }

            if (!_counts.TryGetValue(key, out int value))
            {
                value = 0;
            }

            value += count;
            _counts[key] = value;
        }
        public int GetCount(ICardCountKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!_counts.TryGetValue(key, out int value))
            {
                return 0;
            }

            return value;
        }
        public int GetTotalCount()
        {
            return _counts.Values.Sum();
        }
        public IEnumerator<KeyValuePair<ICardCountKey, int>> GetEnumerator()
        {
            return _counts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
