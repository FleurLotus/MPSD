using System;
using System.Collections.Generic;

namespace Common.Database.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbKeyColumnsAttribute: System.Attribute
    {
        private readonly List<string> _names = new List<string>();
        public string[] Names
        {
            get { return _names.ToArray(); }
        }

        public DbKeyColumnsAttribute(string columnName, params string[] columnNames)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException("columnName");

            _names.Add(columnName);

            if (columnNames != null)
            {
                foreach (string s in columnNames)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        throw new ArgumentNullException("columnNames");

                    if (_names.Contains(s))
                        throw new ArgumentException("Dupilcate keys", "columnNames");

                    _names.Add(s);
                }
            }
        }
    }
}
