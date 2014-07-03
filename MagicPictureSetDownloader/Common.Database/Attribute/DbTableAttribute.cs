using System;

namespace Common.Database
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute: Attribute
    {
        public string Name { get; set; }
    }
}
