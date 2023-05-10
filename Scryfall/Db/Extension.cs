namespace ScryfallTest.Db
{
    using System.Collections.Generic;

    public static class Extension
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            if (dic != null && dic.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return default;
        }
    }
}
