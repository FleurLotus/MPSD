namespace Common.SQL.UnitTests
{
    using System.Collections.Generic;

    internal class CaseSensitivityTestCaseSource
    {
        public CaseSensitivityTestCaseSource(bool? value)
        {
            Value = value;
            if (value.HasValue)
            {
                CaseSensitivity = new CaseSensitivity(value.Value);
                Label = $" - CaseSensitivity = {value.Value})";
            }
            else
            {
                CaseSensitivity = null;
                Label = " - CaseSensitivity = null)";
            }
        }
        public bool? Value { get; }
        public CaseSensitivity CaseSensitivity { get; }
        public string Label { get; }

        public static IEnumerable<CaseSensitivityTestCaseSource> GetTestCases()
        {
            yield return new CaseSensitivityTestCaseSource(true);
            yield return new CaseSensitivityTestCaseSource(false);
            yield return new CaseSensitivityTestCaseSource(null);
        }
        internal void Apply(Column column)
        {
            if (column == null || !Value.HasValue || Value.Value)
            {
                return;
            }

            column.SchemaName = column.SchemaName?.ToLower();
            column.TableName = column.TableName?.ToLower();
            column.Name = column.Name?.ToLower();
        }
        internal void Apply(ForeignKey foreignKey)
        {
            if (foreignKey == null || !Value.HasValue || Value.Value)
            {
                return;
            }

            foreignKey.ReferenceSchemaName = foreignKey.ReferenceSchemaName?.ToLower();
            foreignKey.SourceSchemaName = foreignKey.SourceSchemaName?.ToLower();
            foreignKey.ReferenceTableName = foreignKey.ReferenceTableName?.ToLower();
            foreignKey.SourceTableName = foreignKey.SourceTableName?.ToLower();
            foreignKey.UpdateRule = foreignKey.UpdateRule?.ToLower();
            foreignKey.DeleteRule = foreignKey.DeleteRule?.ToLower();
            foreignKey.Name = foreignKey.Name?.ToLower();
        }
        internal void Apply(Index index)
        {
            if (index == null || !Value.HasValue || Value.Value)
            {
                return;
            }

            index.SchemaName = index.SchemaName?.ToLower();
            index.TableName = index.TableName?.ToLower();
            index.Name = index.Name?.ToLower();
        }
        internal void Apply(PrimaryKey primaryKey)
        {
            if (primaryKey == null || !Value.HasValue || Value.Value)
            {
                return;
            }

            primaryKey.SchemaName = primaryKey.SchemaName?.ToLower();
            primaryKey.TableName = primaryKey.TableName?.ToLower();
            primaryKey.Name = primaryKey.Name?.ToLower();
        }
        internal string Expected(string str)
        {
            if (str == null || !Value.HasValue || Value.Value)
            {
                return str;
            }

            return str.ToLower();
        }
    }
}