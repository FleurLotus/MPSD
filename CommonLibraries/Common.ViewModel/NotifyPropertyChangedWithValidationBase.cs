namespace Common.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Common.Libray.Extension;
    using Common.Libray.Threading;

    public class NotifyPropertyChangedWithValidationBase : NotifyPropertyChangedBase, IValidable
    {
        private static readonly string[] _innerProperty;
        private readonly string[] _toBeValidatedProperty;
        private readonly PropertyInfo[] _toBeValidatedRecursiveProperty;

        private readonly IDictionary<string, IList<Func<string>>> _rules;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        static NotifyPropertyChangedWithValidationBase()
        {
            _innerProperty = typeof(NotifyPropertyChangedWithValidationBase).GetPublicInstanceProperties()
                                                                            .Select(pi=> pi.Name)
                                                                            .ToArray();
        }
        
        protected NotifyPropertyChangedWithValidationBase()
        {
            _rules = new Dictionary<string, IList<Func<string>>>();

            _toBeValidatedProperty = GetType().GetPublicInstanceProperties()
                                              .Select(pi => pi.Name)
                                              .Where(n => !_innerProperty.Contains(n))
                                              .ToArray();

            _toBeValidatedRecursiveProperty = GetType().GetPublicInstanceProperties()
                                                       .Where(pi => !_innerProperty.Contains(pi.Name) &&  typeof(IValidable).IsAssignableFrom(pi.PropertyType))
                                                       .ToArray();
            
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

        protected void AddValidationRule<T>(IEnumerable<Expression<Func<T>>> expressions, Func<string> rule)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            foreach (var expression in expressions)
            {
                AddValidationRule(expression, rule);
            }
        }
        protected void AddValidationRule<T>(Expression<Func<T>> expression, IEnumerable<Func<string>> rules)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");

            foreach (var rule in rules)
            {
                AddValidationRule(expression, rule);
            }
        }
        protected void AddValidationRule<T>(Expression<Func<T>> expression, Func<string> rule)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");

            string propertyName = expression.GetMemberName();

            if (_toBeValidatedProperty.All(name => name != propertyName))
                throw new ArgumentException("property is unknown");

            using (new WriterLock(_lock))
            {
                IList<Func<string>> list;
                if (!_rules.TryGetValue(propertyName, out list))
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
            foreach (string propName in _toBeValidatedProperty)
            {
                string res = ValidateProperty(propName);
                if (!string.IsNullOrWhiteSpace(res))
                    errorMessage.AppendFormat("{0} -> {1}", propName, res);
            }

            //Recursice call on IValidable
            foreach (PropertyInfo prop in _toBeValidatedRecursiveProperty)
            {
                MethodInfo getter = prop.GetGetMethod();
                if (getter != null)
                {
                    IValidable o = getter.Invoke(this, null) as IValidable;
                    if (o != null)
                    {
                        string res = o.Validate();
                        if (!string.IsNullOrWhiteSpace(res))
                            errorMessage.AppendFormat("{0} ---> {1}", prop.Name, res);
                    }
                }
            }

            //Global validation
            IValidator validator = Validator;
            if (validator != null)
            {
                string res = validator.Validate();
                if (!string.IsNullOrWhiteSpace(res))
                    errorMessage.AppendFormat("Global validation ---> {0}", res);
            }

            if (errorMessage.Length == 0)
                return null;

            return errorMessage.ToString();
        }
        private string ValidateProperty(string propertyName)
        {
            IList<Func<string>> propertyRules;
            using (new ReaderLock(_lock))
            {
                IList<Func<string>> lst = _rules.GetOrDefault(propertyName);
                if (lst == null || lst.Count == 0)
                    return null;

                propertyRules = new List<Func<string>>(lst);
            }
            
            StringBuilder sb = new StringBuilder();

            foreach (var rule in propertyRules)
            {
                string res = rule();
                if (!string.IsNullOrWhiteSpace(res))
                    sb.AppendLine(res);
            }

            if (sb.Length == 0)
                return null;

            return sb.ToString();
        }
    }
}
