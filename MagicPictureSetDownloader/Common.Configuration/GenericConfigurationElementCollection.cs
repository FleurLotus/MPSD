namespace Common.Configuration
{
    using System.Configuration;

    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, IHasKey, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            IHasKey key = element as IHasKey;
            // ReSharper restore SuspiciousTypeConversion.Global
            return null == key ? null : key.Key;
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
