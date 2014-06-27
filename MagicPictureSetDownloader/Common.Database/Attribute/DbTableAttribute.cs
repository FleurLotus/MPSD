using System;

namespace Common.Database.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute: System.Attribute
    {
        public string Name { get; set; }
    }
}
