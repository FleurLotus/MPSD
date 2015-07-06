namespace Common.ViewModel.Validation
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    using Common.Library.Extension;
    using Common.ViewModel.Validation.Attributes;

    internal static class ReflectionCacheRepository
    {
        private static readonly string[] _innerProperty;
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _toBeValidatedPropertyCache;
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _toBeValidatedRecursivePropertyCache;
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, string>[]> _propertyValidatorRulesCache;
        
        static ReflectionCacheRepository()
        {
            _toBeValidatedPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
            _toBeValidatedRecursivePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
            _propertyValidatorRulesCache = new ConcurrentDictionary<PropertyInfo, Func<object, string>[]>();

            _innerProperty = typeof(NotifyPropertyChangedWithValidationBase).GetPublicInstanceProperties()
                                                                            .Select(pi => pi.Name)
                                                                            .ToArray();
        }

        public static PropertyInfo[] GetToBeValidatedProperty(Type type)
        {
            return _toBeValidatedPropertyCache.GetOrAdd(type, t => t.GetPublicInstanceProperties()
                                                                    .Where(pi => !_innerProperty.Contains(pi.Name))
                                                                    .ToArray());
        }
        public static PropertyInfo[] GetToBeValidatedRecursiveProperty(Type type)
        {
            return _toBeValidatedRecursivePropertyCache.GetOrAdd(type, t => t.GetPublicInstanceProperties()
                                                                             .Where(pi => !_innerProperty.Contains(pi.Name) && typeof(IValidable).IsAssignableFrom(pi.PropertyType))
                                                                             .ToArray());
        }
        public static Func<object, string>[] GetTypeValidatorRules(PropertyInfo propetyInfo)
        {
            return _propertyValidatorRulesCache.GetOrAdd(propetyInfo, pi => pi.GetCustomAttributes<ValidationAttribute>(true)
                                                                              .Select(att => (Func<object, string>)att.Validate)
                                                                              .ToArray());
        }
    }
}
