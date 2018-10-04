namespace Common.Database
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    internal class TypeDbInfo
    {
        public TypeDbInfo(string tableName, IEnumerable<string> keys, string identity, IDictionary<string, PropertyInfo> prop, Restriction restriction)
        {
            TableName = tableName;
            Keys = (new List<string>(keys)).AsReadOnly();
            Identity = identity;
            Columns = new ReadOnlyDictionary<string, PropertyInfo>(prop);
            Restriction = restriction;
        }

        public IDictionary<string, PropertyInfo> Columns { get; }
        public string TableName { get; }
        public IList<string> Keys { get; }
        public string Identity { get; }
        public Restriction Restriction { get; }
    }
}
