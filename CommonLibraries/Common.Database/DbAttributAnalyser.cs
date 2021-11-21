namespace Common.Database
{
    using System.Linq;
    using System.Reflection;
    using System;
    using System.Collections.Generic;

    using Common.Library.Extension;

    internal static class DbAttributAnalyser
    {
        private static readonly IDictionary<Type, TypeDbInfo> _analysied = new Dictionary<Type, TypeDbInfo>();

        public static TypeDbInfo Analyse(Type type)
        {
            if (_analysied.TryGetValue(type, out TypeDbInfo typeDbInfo))
            {
                return typeDbInfo;
            }

            DbTableAttribute[] tableAttributes = type.GetCustomAttributes<DbTableAttribute>().ToArray();
            if (tableAttributes.Length != 1)
            {
                throw new AttributedTypeException(type, "DbTableAttribute must be declared one and one time for the type");
            }

            string tableName = tableAttributes[0].Name ?? type.Name;

            IDictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();
            IList<string> keys = new List<string>();
            string identity = null;

            foreach (PropertyInfo pi in type.GetPublicInstanceProperties())
            {
                DbColumnAttribute[] columnAttributes = pi.GetCustomAttributes<DbColumnAttribute>().ToArray();
                if (columnAttributes.Length == 1)
                {
                    DbColumnAttribute columnAttribute = columnAttributes[0];
                    string name = columnAttribute.Name ?? pi.Name;

                    columns.Add(name, pi);

                    switch (columnAttribute.Kind)
                    {
                        case ColumnKind.Normal:
                            break;
                        case ColumnKind.PrimaryKey:
                            keys.Add(name);
                            break;
                        case ColumnKind.Identity:
                            keys.Add(name);
                            if (!string.IsNullOrEmpty(identity))
                            {
                                throw new AttributedTypeException(type, "DbColumnAttribute identity could only be declared one time for the type");
                            }
                            identity = name;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(string.Format("{0} is not managed", columnAttribute.Kind));
                    }
                }
            }

            if (columns.Count == 0)
            {
                throw new AttributedTypeException(type, "DbColumnAttribute must be declared at least one time for the type");
            }

            DbRestictedDmlAttribute[] restrictionAttributes = type.GetCustomAttributes<DbRestictedDmlAttribute>().ToArray();

            Restriction restriction = restrictionAttributes.Length == 1 ? restrictionAttributes[0].Restriction : Restriction.None;
            
            typeDbInfo = new TypeDbInfo(tableName, keys, identity, columns, restriction);
            _analysied[type] = typeDbInfo;

            return typeDbInfo;
        }
    }
}
