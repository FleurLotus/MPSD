namespace Common.Library.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class ReflectionExtension
    {
        private static readonly IDictionary<Type, PropertyInfo[]> _propertiesCache = new Dictionary<Type, PropertyInfo[]>();

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider member, bool inherit)
            where T : Attribute
        {
            return (T[])member.GetCustomAttributes(typeof(T), inherit);
        }

        public static PropertyInfo[] GetPublicInstanceProperties(this object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            return o.GetType().GetPublicInstanceProperties();
        }
        public static PropertyInfo[] GetPublicInstanceProperties(this Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (!_propertiesCache.TryGetValue(t, out PropertyInfo[] ret))
            {
                lock (_propertiesCache)
                {
                    if (!_propertiesCache.TryGetValue(t, out ret))
                    {
                        ret = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        _propertiesCache[t] = ret;
                    }
                }
            }

            return ret;
        }
    }
}
