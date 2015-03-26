namespace Common.Libray.Enums
{
    using System;

    public static class Matcher<T> where T : struct, IConvertible
    {
        private readonly static bool _withFlag;

        static Matcher()
        {
            Type t = typeof(T);
            if (!t.IsEnum)
                throw new ArgumentException("T could only be a Enum and not a " + t);

            _withFlag = t.GetCustomAttributes<FlagsAttribute>().Length > 0;
        }

        public static bool HasValue(T source, T matching)
        {
            if (_withFlag)
            {
                return ((int)(object)source & ((int)(object)matching)) > 0;
            }

            return source.Equals(matching);
        }
        public static bool IncludeValue(T source, T matching)
        {
            if (_withFlag)
            {
                if ((int)(object)matching == 0)
                    return false;
                return ((int)(object)source & ((int)(object)matching)) == (int)(object)matching;
            }

            return source.Equals(matching);
        }

    }
}
