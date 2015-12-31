namespace Common.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    public static class Mapper<T>
        where T : class, new()
    {
        private static readonly CommandBuilder _commandBuilder;

        static Mapper()
        {
            _commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(T)));
        }

        public static T Load(IDbConnection cnx, T input)
        {
            using (IDbCommand cmd = cnx.CreateCommand())
            {
                _commandBuilder.BuildSelectOneCommand(cmd, input);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    IDictionary<int, PropertyInfo> map = _commandBuilder.GenerateReaderMap(reader);

                    if (reader.Read())
                    {
                        foreach (KeyValuePair<int, PropertyInfo> kv in map)
                        {
                            object value = reader.GetValue(kv.Key);
                            SetValue(input, kv.Value, value);
                        }
                        return input;
                    }

                    return null;
                }
            }
        }
        public static IEnumerable<T> LoadAll(IDbConnection cnx)
        {
            IList<T> ret = new List<T>();

            using (IDbCommand cmd = cnx.CreateCommand())
            {
                _commandBuilder.BuildSelectAllCommand(cmd);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    IDictionary<int, PropertyInfo> map = _commandBuilder.GenerateReaderMap(reader);

                    while (reader.Read())
                    {
                        //Don't care about internal or private because it is a call to Activator.CreateInstance<T>()
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

        public static void DeleteOne(IDbConnection cnx, T value)
        {
            DeleteMulti(cnx, new[] { value });
        }
        public static void DeleteMulti(IDbConnection cnx, IEnumerable<T> values)
        {
            ExecuteWithTransaction(cnx, values, _commandBuilder.BuildDeleteOneCommand);
        }
        public static void InsertOne(IDbConnection cnx, T value)
        {
            InsertMulti(cnx, new[] { value });
        }
        public static void InsertMulti(IDbConnection cnx, IEnumerable<T> values)
        {
            ExecuteWithTransaction(cnx, values, _commandBuilder.BuildInsertOneCommand, GetIdentity);
        }

        private static void GetIdentity(IDbCommand cmd, T value)
        {
            PropertyInfo idKeyPropertyInfo = _commandBuilder.GetIdKeyPropertyInfo();
            if (idKeyPropertyInfo != null)
            {
                int id = IdentityRetriever.GetId(cmd);
                SetValue(value, idKeyPropertyInfo, id);
            }
        }
        public static void UpdateOne(IDbConnection cnx, T value)
        {
            UpdateMulti(cnx, new[] { value });
        }
        public static void UpdateMulti(IDbConnection cnx, IEnumerable<T> values)
        {
            ExecuteWithTransaction(cnx, values, _commandBuilder.BuildUpdateOneCommand);
        }

        private static void ExecuteWithTransaction(IDbConnection cnx, IEnumerable<T> values,
                                                   Action<IDbCommand,T> prepareCommand, Action<IDbCommand,T> doPostExecuteAction = null)
        {
            using (IDbTransaction transaction = cnx.BeginTransaction())
            {
                try
                {
                    foreach (T value in values)
                    {
                        using (IDbCommand cmd = cnx.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            prepareCommand(cmd, value);
                            if (cmd.ExecuteNonQuery() != 1)
                                throw new ApplicationDbException("Wrong number of row affected. Rollback");

                            if (doPostExecuteAction != null)
                            {
                                doPostExecuteAction(cmd, value);
                            }
                        }

                    }
                    transaction.Commit();
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

            object safeValue;
            if (value == null || value == DBNull.Value)
            {
                safeValue = null;
            }
            else if (wantedType.IsEnum)
            {
                safeValue = Enum.Parse(wantedType, value.ToString());
            }
            else 
            {
                safeValue = Convert.ChangeType(value, wantedType);
            }
            pi.SetValue(t, safeValue, null);
        }
    }
}