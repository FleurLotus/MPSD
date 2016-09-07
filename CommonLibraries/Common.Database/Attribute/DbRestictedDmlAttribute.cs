namespace Common.Database
{
    using System;

    [Flags]
    public enum Restriction
    {
        None = 0,
        Insert = 1,
        Update = 1<<1,
        Delete = 1<<2
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DbRestictedDmlAttribute : Attribute
    {
        public DbRestictedDmlAttribute(Restriction restriction)
        {
            Restriction = restriction;
        }

        public Restriction Restriction { get; private set; }
    }
}
