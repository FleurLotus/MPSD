namespace Common.ViewModel.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Common.Collection;
    using Common.Threading;

    public class NotifyPropertyChangedWithValidationBase : NotifyPropertyChangedBase, IValidable
    {
        private readonly PropertyInfo[] _toBeValidatedProperty;
        private readonly PropertyInfo[] _toBeValidatedRecursiveProperty;
        private readonly IDictionary<PropertyInfo, Func<object, string>[]> _propertyValidatorRules;

        private readonly IDictionary<string, IList<Func<string>>> _rules;

        private readonly Lazy<ReaderWriterLockSlim> _lazyLock = new Lazy<ReaderWriterLockSlim>(() => new ReaderWriterLockSlim());

        protected NotifyPropertyChangedWithValidationBase()
        {
            _rules = new Dictionary<string, IList<Func<string>>>();

            _toBeValidatedProperty = ReflectionCacheRepository.GetToBeValidatedProperty(GetType());

            _propertyValidatorRules = _toBeValidatedProperty.ToDictionary(pi => pi, ReflectionCacheRepository.GetTypeValidatorRules);

            _toBeValidatedRecursiveProperty = ReflectionCacheRepository.GetToBeValidatedRecursiveProperty(GetType());

        }
        protected IValidator Validator { get; set; }
        public string this[string columnName]
        {
            get { return ValidateProperty(columnName); }
        }
        public string Error
        {
            get { return Validate(); }
        }

        protected void AddValidationRule(IEnumerable<string> propertyNames, Func<string> rule)
        {
            ArgumentNullException.ThrowIfNull(propertyNames);
            foreach (string propertyName in propertyNames)
            {
                AddValidationRule(propertyName, rule);
            }
        }
        protected void AddValidationRule(string propertyName, IEnumerable<Func<string>> rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (Func<string> rule in rules)
            {
                AddValidationRule(propertyName, rule);
            }
        }
        protected void AddValidationRule(string propertyName, Func<string> rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            if (_toBeValidatedProperty.All(pi => pi.Name != propertyName))
            {
                throw new ArgumentException("property is unknown");
            }

            using (new WriterLock(_lazyLock.Value))
            {
                if (!_rules.TryGetValue(propertyName, out IList<Func<string>> list))
                {
                    list = new List<Func<string>>();
                    _rules[propertyName] = list;
                }
                list.Add(rule);
            }
        }

        //Be Careful of circular reference in IValidation hierarchie
        public string Validate()
        {
            StringBuilder errorMessage = new StringBuilder();

            //Property rule checks
            foreach (PropertyInfo pi in _toBeValidatedProperty)
            {
                string propName = pi.Name;
                string res = ValidateProperty(propName);

                if (!string.IsNullOrWhiteSpace(res))
                {
                    errorMessage.AppendFormat("{0} -> {1}", propName, res);
                }
            }

            //Recursice call on IValidable
            foreach (PropertyInfo prop in _toBeValidatedRecursiveProperty)
            {
                MethodInfo getter = prop.GetGetMethod();
                if (getter != null)
                {
                    if (getter.Invoke(this, null) is IValidable o)
                    {
                        string res = o.Validate();
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            errorMessage.AppendFormat("{0} ---> {1}", prop.Name, res);
                        }
                    }
                }
            }

            //Global validation
            IValidator validator = Validator;
            if (validator != null)
            {
                string res = validator.Validate();
                if (!string.IsNullOrWhiteSpace(res))
                {
                    errorMessage.AppendFormat("Global validation ---> {0}", res);
                }
            }

            return errorMessage.ToString();
        }
        private string ValidatePropertyUsingAttributes(string propertyName)
        {
            //Rules Attribute check
            KeyValuePair<PropertyInfo, Func<object, string>[]> keyValue = _propertyValidatorRules.First(kv => kv.Key.Name == propertyName);
            if (keyValue.Value == null || keyValue.Value.Length == 0)
            {
                return null;
            }

            PropertyInfo pi = keyValue.Key;
            MethodInfo getter = pi.GetGetMethod();
            if (getter == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            object value = getter.Invoke(this, null);
            foreach (Func<object, string> rule in keyValue.Value)
            {
                string res = rule(value);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    sb.AppendLine(res);
                }
            }
            return sb.ToString();
        }
        private string ValidateProperty(string propertyName)
        {
            StringBuilder sb = new StringBuilder();

            IList<Func<string>> propertyRules;
            using (new ReaderLock(_lazyLock.Value))
            {
                IList<Func<string>> lst = _rules.GetOrDefault(propertyName);
                propertyRules = lst == null || lst.Count == 0 ? new List<Func<string>>() : new List<Func<string>>(lst);
            }

            foreach (Func<string> rule in propertyRules)
            {
                string res = rule();
                if (!string.IsNullOrWhiteSpace(res))
                {
                    sb.AppendLine(res);
                }
            }

            sb.Append(ValidatePropertyUsingAttributes(propertyName));

            return sb.ToString();
        }
    }
}