namespace Common.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Common.Library;

    internal class LinkedProperties
    {
        private readonly HashSet<string> _propertyNameSet;
        private readonly Dictionary<string, HashSet<string>> _linkedProperties = new Dictionary<string, HashSet<string>>();
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _sync = new System.Threading.Lock();
#else
        private readonly object _sync = new object();
#endif

        internal LinkedProperties(INotifyPropertyChanged parent)
        {
            ArgumentNullException.ThrowIfNull(parent);

            _propertyNameSet = new HashSet<string>(parent.GetPublicInstanceProperties().Select(pi => pi.Name));
        }

        internal void Add(string sourceName, string destinationName)
        {
            if (!_propertyNameSet.Contains(sourceName))
            {
                throw new ArgumentException(sourceName + " is not a valid property Name");
            }

            if (!_propertyNameSet.Contains(destinationName))
            {
                throw new ArgumentException(destinationName + " is not a valid property Name");
            }

            if (sourceName == destinationName)
            {
                throw new ArgumentException("source and destination could not be the same");
            }

            lock (_sync)
            {
                if (!_linkedProperties.TryGetValue(sourceName, out HashSet<string> linked))
                {
                    linked = new HashSet<string>();
                    _linkedProperties.Add(sourceName, linked);
                }
                linked.Add(destinationName);
            }
        }

        internal IEnumerable<string> GetNotifyList(string propertyName)
        {
            HashSet<string> ret = new HashSet<string>();
            lock (_sync)
            {
                GetNotifyList(propertyName, ret);
            }
            return ret;
        }
        private void GetNotifyList(string propertyName, ISet<string> notifylist)
        {
            if (notifylist.Contains(propertyName))
            {
                return;
            }

            notifylist.Add(propertyName);

            if (_linkedProperties.TryGetValue(propertyName, out HashSet<string> linked))
            {
                foreach (string linkedPropertyName in linked)
                {
                    GetNotifyList(linkedPropertyName, notifylist);
                }
            }
        }
    }
}