namespace Common.Database
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
