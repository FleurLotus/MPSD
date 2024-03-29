﻿namespace Common.Configuration
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, IHasKey, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            IHasKey key = element as IHasKey;
            return key?.Key;
        }
        public void Add(T element)
        {
            BaseAdd(element);
        }
        public void Remove(T element)
        {
            BaseRemove(GetElementKey(element));
        }
        public void Clear()
        {
            BaseClear();
        }

    }
}
