using System.Linq;
using System.Reflection;
using Common.Libray;
using System;
using System.Collections.Generic;

namespace Common.Database
{
    internal static class DbAttributAnalyser
    {
        private static readonly IDictionary<Type, TypeDbInfo> _analysied = new Dictionary<Type, TypeDbInfo>();

        public static TypeDbInfo Analyse(Type type)
        {
            TypeDbInfo typeDbInfo;

            if (_analysied.TryGetValue(type, out typeDbInfo))
                return typeDbInfo;

            DbTableAttribute[] tableAttributes = type.GetCustomAttributes<DbTableAttribute>();
            if (tableAttributes.Length != 1)
                throw new AttributedTypeException(type, "DbTableAttribute must be declared one and one time for the type");

            string tableName = tableAttributes[0].Name ?? type.Name;

            IDictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                DbColumnAttribute[] columnAttributes = pi.GetCustomAttributes<DbColumnAttribute>();
                if (columnAttributes.Length == 1)
                    columns.Add(columnAttributes[0].Name ?? pi.Name, pi);
            }

            if (columns.Count == 0)
                throw new AttributedTypeException(type, "DbColumnAttribute must be declared at least one time for the type");

            IEnumerable<string> keys = type.GetProperties().Where(pi => pi.GetCustomAttributes<DbKeyColumnAttribute>().Length == 1)
                                                           .Select(pi => pi.Name);

            string identities = type.GetProperties().Where(pi => pi.GetCustomAttributes<DbKeyColumnAttribute>().Length == 1 && pi.GetCustomAttributes<DbKeyColumnAttribute>()[0].IsIdentity)
                                                    .Select(pi => pi.Name)
                                                    .FirstOrDefault();

            typeDbInfo = new TypeDbInfo(tableName, keys, identities, columns);
            _analysied[type] = typeDbInfo;

            return typeDbInfo;
        }
    }
}
