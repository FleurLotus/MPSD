namespace Common.Library.Extension
{
    using System.Collections.Generic;

    public static class DictionaryExtension
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> added)
        {
            foreach (KeyValuePair<TKey, TValue> kv in added)
            {
                source.Add(kv);
            }
        }
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (!source.TryGetValue(key, out TValue ret))
            {
                ret = default;
            }

            return ret;
        }
    }
}
