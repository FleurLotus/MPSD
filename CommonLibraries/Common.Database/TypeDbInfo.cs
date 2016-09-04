namespace Common.Database
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common.Library;

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

        public IDictionary<string, PropertyInfo> Columns { get; private set; }
        public string TableName { get; private set; }
        public IList<string> Keys { get; private set; }
        public string Identity { get; private set; }
        public Restriction Restriction { get; private set; }
    }
}
