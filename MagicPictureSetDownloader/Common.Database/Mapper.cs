using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using Common.Database.Attribute;
using Common.Libray;

namespace Common.Database
{
    public static class Mapper<T> where T : class, new()
    {
        private static readonly string _tableName;
        private static readonly string[] _keys;
        private static readonly IDictionary<string, PropertyInfo> _columns = new Dictionary<string, PropertyInfo>();

        static Mapper()
        {
            Type type = typeof (T);

            DbTableAttribute[] tableAttributes = type.GetCustomAttributes<DbTableAttribute>();
            if (tableAttributes.Length != 1)
                throw new AttributedTypeException(type, "DbTableAttribute must be declared one and one time for the type");

            _tableName = tableAttributes[0].Name ?? type.Name;

            DbKeyColumnsAttribute[] tableKeysAttributes = type.GetCustomAttributes<DbKeyColumnsAttribute>();
            if (tableKeysAttributes.Length == 1)
                _keys = tableKeysAttributes[0].Names;

            foreach (PropertyInfo pi in type.GetProperties())
            {
                DbColumnAttribute[] columnAttributes = pi.GetCustomAttributes<DbColumnAttribute>();
                if (columnAttributes.Length == 1)
                    _columns.Add(columnAttributes[0].Name ?? pi.Name, pi);
            }

            if (_columns.Count == 0)
                throw new AttributedTypeException(type, "DbColumnAttribute must be declared at least one time for the type");
        }

        public static T[] Load(DbConnection cnx)
        {
            IList<T> ret = new List<T>();

            using (DbCommand cmd = cnx.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = GenerateSelectQuery();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    IDictionary<int, PropertyInfo> map = new Dictionary<int, PropertyInfo>();
                    foreach (KeyValuePair<string, PropertyInfo> kv in _columns)
                    {
                        map.Add(reader.GetOrdinal(kv.Key), kv.Value);
                    }
                    
                    while (reader.Read())
                    {
                        T t = new T();

                        foreach (KeyValuePair<int, PropertyInfo> kv in map)
                        {
                            Type wantedType = Nullable.GetUnderlyingType(kv.Value.PropertyType) ?? kv.Value.PropertyType;
                            object value = reader.GetValue(kv.Key);
                            object safeValue = value == null  || value == DBNull.Value ? null : Convert.ChangeType(value, wantedType);
                            kv.Value.SetValue(t, safeValue, null);
                        }
                        ret.Add(t);
                    }
                }
            }

            return ret.ToArray();
        }


        private static string GenerateSelectQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            string[] keys = _columns.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.Append(keys[i]);
            }
            sb.Append(" FROM ");
            sb.Append(_tableName);

            return sb.ToString();

        }
        private static string GenerateUpdateQuery(T input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE ");
            sb.Append(_tableName);
            sb.Append(" SET ");
            string[] cols = _columns.Keys.Where(s => !_keys.Contains(s)).ToArray();
            for (int i = 0; i < cols.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.AppendFormat("{0}={1}", cols[i], _columns[cols[i]].GetValue(input, null).ToSqlString());
            }
            sb.Append(" WHERE ");
            for (int i = 0; i < _keys.Length; i++)
            {
                if (i != 0)
                    sb.Append(" AND ");
                string value = _columns[_keys[i]].GetValue(input, null).ToSqlString();
                sb.AppendFormat("({0}{1}{2})", _keys[i], SQLQueryTools.EqualityOperator(value), value);
            }

            return sb.ToString();

        }
    }
}

/*
 * ALERT: Sample of save / load image in SQL CE Base
Image myImage = Image.FromFile("c:\\xyz.jpg");
System.IO.MemoryStream imgMemoryStream = new System.IO.MemoryStream();
myImage.Save(imgMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
byte[] imgByteData = imgMemoryStream.GetBuffer();
//Store image bytes data into database
SqlCeConnection scon = new SqlCeConnection("Data Source=D:\\myDB.sdf");
SqlCeCommand cmd = new SqlCeCommand("Update ImageTable Set Picture=@myPicture Where Username='Martin'", scon);
cmd.Parameters.Add("@myPicture", SqlDbType.Image).Value = imgByteData;
// Or cmd.Parameters.Add("@myPicture", SqlDbType.Binary).Value = imgByteData
scon.Open();
cmd.ExecuteNonQuery();
scon.Close();



SqlCeConnection scon = new SqlCeConnection("Data Source=D:\\myDB.sdf");
SqlCeCommand cmd = new SqlCeCommand("Select * From ImageTable Where id = 1", scon);
scon.Open();
SqlCeDataReader sdr = cmd.ExecuteReader();
byte[] imgByteData = Convert.ToByte(sdr.Item("Picture"));
Bitmap bitmap = new Bitmap(new System.IO.MemoryStream(imgByteData));
// or Dim bitmap As Bitmap = Drawing.Image.FromStream(new System.IO.MemoryStream(imgByteData)); 
PictureBox1.Image = bitmap;
*/
