﻿namespace Common.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class NotifyPropertyChangedWithLinkedPropertiesBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly HashSet<string> _innerPropertyNameSet;
        private readonly HashSet<string> _childPropertyNameSet;
        private readonly Dictionary<string, HashSet<string>> _linkedProperties;

        static NotifyPropertyChangedWithLinkedPropertiesBase()
        {
            IEnumerable<string> propertyNames = typeof(NotifyPropertyChangedBase).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(pi => pi.Name);
            _innerPropertyNameSet = new HashSet<string>(propertyNames);
        }

        protected NotifyPropertyChangedWithLinkedPropertiesBase()
        {
            _linkedProperties = new Dictionary<string, HashSet<string>>();
            IEnumerable<string> propertyNames = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(pi => pi.Name).Where(n => !_innerPropertyNameSet.Contains((n)));
            _childPropertyNameSet = new HashSet<string>(propertyNames);
        }
        public void OnNotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            OnNotifyPropertyChangedWithLinked(expression.GetMemberName(), new HashSet<string>());
        }
        public void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>> destination)
        {
            string sourceName = source.GetMemberName();
            string destinationName = destination.GetMemberName();

            if (!_childPropertyNameSet.Contains(sourceName))
                throw new ArgumentException(sourceName + " is not a valid property Name");

            if (!_childPropertyNameSet.Contains(destinationName))
                throw new ArgumentException(destinationName + " is not a valid property Name");

            if (sourceName == destinationName)
                throw new ArgumentException("source and destination could not be the same");
            
            HashSet<string> linked;
            if (!_linkedProperties.TryGetValue(sourceName, out linked))
            {
                linked = new HashSet<string>();
                _linkedProperties.Add(sourceName, linked);
            }

            linked.Add(destinationName);
        }
        public void AddLinkedProperty<T1, T2>(Expression<Func<T1>>[] sources, Expression<Func<T2>> destination)
        {
            foreach (Expression<Func<T1>> source in sources)
            {
                AddLinkedProperty(source, destination);
            }

        }
        public void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>>[] destinations)
        {
            foreach (Expression<Func<T2>> destination in destinations)
            {
                AddLinkedProperty(source, destination);
            }
        }

        private void OnNotifyPropertyChangedWithLinked(string propertyName, ISet<string> firedPropertyChanged)
        {
            if (!firedPropertyChanged.Contains(propertyName))
            {
                firedPropertyChanged.Add(propertyName);
                OnNotifyPropertyChanged(propertyName);

                HashSet<string> linked;
                if (_linkedProperties.TryGetValue(propertyName, out linked))
                {
                    foreach (string linkedPropertyName in linked)
                    {
                        OnNotifyPropertyChangedWithLinked(linkedPropertyName, firedPropertyChanged);
                    }
                }
            }
        }
        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
