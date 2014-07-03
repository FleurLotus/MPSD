using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Common.Database
{
    public static class Mapper<T> where T : class, new()
    {
        private static readonly CommandBuilder _commandBuilder;
        
        static Mapper()
        {
            _commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(T)));
        }

        public static T Load(DbConnection cnx, T input)
        {
            using (DbCommand cmd = cnx.CreateCommand())
            {
                _commandBuilder.BuildSelectOneCommand(cmd, input);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    IDictionary<int, PropertyInfo> map = _commandBuilder.GenerateReaderMap(reader);

                    if (reader.Read())
                    {
                        foreach (KeyValuePair<int, PropertyInfo> kv in map)
                        {
                            object value = reader.GetValue(kv.Key);
                            SetValue(input, kv.Value, value);
                        }
                    }

                    return input;
                }
            }
        }
        public static IEnumerable<T> LoadAll(DbConnection cnx)
        {
            IList<T> ret = new List<T>();

            using (DbCommand cmd = cnx.CreateCommand())
            {
                _commandBuilder.BuildSelectAllCommand(cmd);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    IDictionary<int, PropertyInfo> map = _commandBuilder.GenerateReaderMap(reader);
                    
                    while (reader.Read())
                    {
                        T t = new T();

                        foreach (KeyValuePair<int, PropertyInfo> kv in map)
                        {
                            object value = reader.GetValue(kv.Key);
                            SetValue(t, kv.Value, value);
                        }
                        ret.Add(t);
                    }
                }
            }
            return ret;
        }
        public static void InsertOne(DbConnection cnx, T value)
        {
            ExecuteWithTransaction(cnx, cmd => _commandBuilder.BuildInsertOneCommand(cmd, value));
        }
        public static void UpdateOne(DbConnection cnx, T value)
        {
            ExecuteWithTransaction(cnx, cmd => _commandBuilder.BuildUpdateOneCommand(cmd, value));
        }

        private static void ExecuteWithTransaction(DbConnection cnx, Action<DbCommand> prepareCommand)
        {
            using (DbTransaction transaction = cnx.BeginTransaction())
            {
                try
                {
                    using (DbCommand cmd = cnx.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        prepareCommand(cmd);
                        if (cmd.ExecuteNonQuery() != 1)
                            throw new ApplicationDbException("Wrong number of row affected. Rollback");
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        private static void SetValue(T t, PropertyInfo pi, object value)
        {
            Type wantedType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

            object safeValue = value == null || value == DBNull.Value ? null : Convert.ChangeType(value, wantedType);
            pi.SetValue(t, safeValue, null);
        }
    }
}