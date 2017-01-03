namespace Common.Database
{
    using System;

    public enum ColumnKind
    {
        Normal,
        PrimaryKey,
        Identity
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public DbColumnAttribute()
        {
            Kind = ColumnKind.Normal;
        }

        public ColumnKind Kind { get; set; }
        public string Name { get; set; }
    }
}
