namespace Common.SQL
{
    using System;
    using System.Data;

    public static class DataRecordExt
    {
        #region GetOrDefault
        public static string GetStringOrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetString(index);
        }
        public static bool GetBoolOrDefault(this IDataRecord dr, int index)
        {
            return !dr.IsDBNull(index) && dr.GetBoolean(index);
        }
        public static int GetInt32OrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetInt32(index);
        }
        public static short GetInt16OrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetInt16(index);
        }
        public static long GetInt64OrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetInt64(index);
        }
        public static double GetDoubleOrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetDouble(index);
        }
        public static DateTime GetDateTimeOrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetDateTime(index);
        }
        public static byte GetByteOrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetByte(index);
        }
        public static char GetCharOrDefault(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? default : dr.GetChar(index);
        }
        #endregion

        #region GetOrNull
        public static string GetStringOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? null : dr.GetString(index);
        }
        public static bool? GetBoolOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (bool?)null : dr.GetBoolean(index);
        }
        public static short? GetInt16OrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (short?)null : dr.GetInt16(index);
        }
        public static int? GetInt32OrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (int?)null : dr.GetInt32(index);
        }
        public static long? GetInt64OrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (long?)null : dr.GetInt64(index);
        }
        public static double? GetDoubleOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (double?)null : dr.GetDouble(index);
        }
        public static DateTime? GetDateTimeOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (DateTime?)null : dr.GetDateTime(index);
        }
        public static byte? GetByteOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (byte?)null : dr.GetByte(index);
        }
        public static char? GetCharOrNull(this IDataRecord dr, int index)
        {
            return dr.IsDBNull(index) ? (char?)null : dr.GetChar(index);
        }
        #endregion
    }
}