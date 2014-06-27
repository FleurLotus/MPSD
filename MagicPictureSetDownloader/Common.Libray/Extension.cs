using System;
using System.Reflection;

namespace Common.Libray
{
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
    }
}
