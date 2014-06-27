using System;
using System.Globalization;

namespace Common.Database
{
    internal static class SQLQueryTools
    {
        private const string NullString = "NULL";

        public static string ToSqlString(this object o)
        {
            if (o == null)
                return NullString;

            if (o is int)
                return ((int) o).ToString(CultureInfo.InvariantCulture);

            if (o is double)
                return ((double)o).ToString(CultureInfo.InvariantCulture);

            if (o is DateTime)
                return ((DateTime)o).ToString("yyyyMMdd HH:mm:ss");

            return string.Format("'{0}'", o.ToString().Replace("'", "''"));
        }
        public static string EqualityOperator(string value)
        {
            return value == NullString ? " IS " : " = ";
        }
       /* public static object ChangeType(object value, Type desireType)
        {




            return Convert.ChangeType(reader.GetValue(kv.Key), kv.Value.PropertyType)
        }*/
    }
}
