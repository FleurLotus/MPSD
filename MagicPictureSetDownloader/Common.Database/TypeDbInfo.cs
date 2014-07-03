using System.Collections.Generic;
using System.Reflection;
using Common.Libray;

namespace Common.Database
{
    internal class TypeDbInfo
    {
        public TypeDbInfo(string tableName, IEnumerable<string> keys, IDictionary<string, PropertyInfo> prop)
        {
            TableName = tableName;
            Keys = (new List<string>(keys)).AsReadOnly();
            Columns = new ReadOnlyDictionary<string, PropertyInfo>(prop);
        }
        
        public IDictionary<string, PropertyInfo> Columns { get; private set; }
        public string TableName { get; private set; }
        public IList<string> Keys { get; private set; }
    }
}
