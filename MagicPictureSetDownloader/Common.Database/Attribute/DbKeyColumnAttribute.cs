using System;

namespace Common.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbKeyColumnAttribute : Attribute
    {
        public DbKeyColumnAttribute(bool isIdentity)
        {
            IsIdentity = isIdentity;
        }
        
        public bool IsIdentity { get; set; }
    }
}
