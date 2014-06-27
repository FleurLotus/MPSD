using System;

namespace Common.Database.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DbColumnAttribute : System.Attribute
    {
        public string Name { get; set; }
    }
}
