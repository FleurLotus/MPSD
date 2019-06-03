namespace Common.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Common.Library.Extension;

    internal class LinkedProperties
    {
        private readonly HashSet<string> _propertyNameSet;
        private readonly Dictionary<string, HashSet<string>> _linkedProperties = new Dictionary<string, HashSet<string>>();
        private readonly object _sync = new object();
        
        internal LinkedProperties(INotifyPropertyChanged parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

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
                HashSet<string> linked;
                if (!_linkedProperties.TryGetValue(sourceName, out linked))
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
            if (!notifylist.Contains(propertyName))
            {
                notifylist.Add(propertyName);

                HashSet<string> linked;
                if (_linkedProperties.TryGetValue(propertyName, out linked))
                {
                    foreach (string linkedPropertyName in linked)
                    {
                        GetNotifyList(linkedPropertyName, notifylist);
                    }
                }
            }
        }
    }
}
