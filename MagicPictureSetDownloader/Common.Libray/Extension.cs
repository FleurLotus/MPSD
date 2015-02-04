namespace Common.Libray
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

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

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(str) || oldValue == null)
                return str;

            int index = str.IndexOf(oldValue, comparison);

            if (index == -1)
                return str;

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}
