namespace Common.Database
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute: Attribute
    {
        public string Name { get; set; }
    }
}
