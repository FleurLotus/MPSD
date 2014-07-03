using System;

namespace Common.Database
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
