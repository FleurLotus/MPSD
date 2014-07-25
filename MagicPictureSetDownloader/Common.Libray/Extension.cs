namespace Common.Libray
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class Extension
    {
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider member)
          where T : Attribute
        {
            return GetCustomAttributes<T>(member, true);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider member, bool inherit)
            where T : Attribute
        {
            return (T[])member.GetCustomAttributes(typeof(T), inherit);
        }
        
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> added)
        {
            foreach (KeyValuePair<TKey, TValue> kv in added)
                source.Add(kv);
        }
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            TValue ret;
            if (!source.TryGetValue(key, out ret))
                ret = default(TValue);

            return ret;
        }

    }
}
