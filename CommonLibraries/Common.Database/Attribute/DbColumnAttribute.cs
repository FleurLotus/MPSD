namespace Common.Database
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
