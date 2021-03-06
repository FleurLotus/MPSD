﻿namespace Common.SQL
{
    public interface IPrimaryKey
    {
        string Name { get; }
        string TableName { get; }
        string SchemaName { get; }

        IColumn[] Columns();
    }
}