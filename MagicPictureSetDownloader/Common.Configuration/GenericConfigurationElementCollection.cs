using System.Configuration;

namespace Common.Configuration
{
    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection
        where T: ConfigurationElement, IHasKey, new()
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as IHasKey).Key;
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
