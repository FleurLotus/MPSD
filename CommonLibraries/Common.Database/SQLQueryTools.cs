namespace Common.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    internal static class SQLQueryTools
    {
        private const string NullString = "NULL";
        private static readonly IDictionary<Type, DbType> _typeToDbTypes = new Dictionary<Type, DbType>
        {
            {typeof (byte), DbType.Byte},
            {typeof (sbyte), DbType.SByte},
            {typeof (short), DbType.Int16},
            {typeof (ushort), DbType.UInt16},
            {typeof (int), DbType.Int32},
            {typeof (uint), DbType.UInt32},
            {typeof (long), DbType.Int64},
            {typeof (ulong), DbType.UInt64},
            {typeof (float), DbType.Single},
            {typeof (double), DbType.Double},
            {typeof (decimal), DbType.Decimal},
            {typeof (bool), DbType.Boolean},
            {typeof (string), DbType.String},
            {typeof (char), DbType.StringFixedLength},
            {typeof (Guid), DbType.Guid},
            {typeof (DateTime), DbType.DateTime},
            {typeof (DateTimeOffset), DbType.DateTimeOffset},
            //{typeof (byte[]), DbType.Binary}, //Remove because Binary is limited to 8000 when SqlDbType.Image is not
            {typeof (byte?), DbType.Byte},
            {typeof (sbyte?), DbType.SByte},
            {typeof (short?), DbType.Int16},
            {typeof (ushort?), DbType.UInt16},
            {typeof (int?), DbType.Int32},
            {typeof (uint?), DbType.UInt32},
            {typeof (long?), DbType.Int64},
            {typeof (ulong?), DbType.UInt64},
            {typeof (float?), DbType.Single},
            {typeof (double?), DbType.Double},
            {typeof (decimal?), DbType.Decimal},
            {typeof (bool?), DbType.Boolean},
            {typeof (char?), DbType.StringFixedLength},
            {typeof (Guid?), DbType.Guid},
            {typeof (DateTime?), DbType.DateTime},
            {typeof (DateTimeOffset?), DbType.DateTimeOffset}
        };

        public static string ToSqlString(this object o)
        {
            if (o == null)
            {
                return NullString;
            }

            if (o is int i)
            {
                return i.ToString(CultureInfo.InvariantCulture);
            }

            if (o is double d)
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }

            if (o is DateTime dt)
            {
                return dt.ToString("yyyyMMdd HH:mm:ss");
            }

            return $"'{o.ToString().ToSqlStringEscaped()}'";
        }
        public static string EqualityOperator(string value)
        {
            return value == NullString ? " IS " : " = ";
        }

        public static DbType? ToDbType(this Type type)
        {
            if (!_typeToDbTypes.TryGetValue(type, out DbType dbType))
            {
                return null;
            }
            return dbType;
        }
        public static string ToSqlStringEscaped(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : s.Replace("'", "''");
        }
    }
}