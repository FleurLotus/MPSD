﻿namespace Common.Library.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;

    public static class FlagExtension
    {
        private static readonly ConcurrentDictionary<string, FlagCount> _flags = new ConcurrentDictionary<string, FlagCount>();

        public static IDisposable SetFlag(this object source, string name)
        {
            CheckArgs(source, name);
            FlagCount flagCount = _flags.GetOrAdd(name, n => new FlagCount(n));

            return new Flag(flagCount, source);

        }
        public static bool IsFlagSet(this object source, string name)
        {
            CheckArgs(source, name);

            FlagCount flagCount;
            if (!_flags.TryGetValue(name, out flagCount))
                return false;

            return flagCount.IsFlagSet(source);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void CheckArgs(object source, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Can't be null or empty", "name");
            if (source == null)
                throw new ArgumentNullException("source");
        }
    }
}
