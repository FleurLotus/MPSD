namespace Common.Database.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using NUnit.Framework;

    [TestFixture]
    public class TestSQLQueryTools
    {
        [TestCase(typeof(byte), ExpectedResult = DbType.Byte)]
        [TestCase(typeof(sbyte), ExpectedResult = DbType.SByte)]
        [TestCase(typeof(short), ExpectedResult = DbType.Int16)]
        [TestCase(typeof(ushort), ExpectedResult = DbType.UInt16)]
        [TestCase(typeof(int), ExpectedResult = DbType.Int32)]
        [TestCase(typeof(uint), ExpectedResult = DbType.UInt32)]
        [TestCase(typeof(long), ExpectedResult = DbType.Int64)]
        [TestCase(typeof(ulong), ExpectedResult = DbType.UInt64)]
        [TestCase(typeof(float), ExpectedResult = DbType.Single)]
        [TestCase(typeof(double), ExpectedResult = DbType.Double)]
        [TestCase(typeof(decimal), ExpectedResult = DbType.Decimal)]
        [TestCase(typeof(bool), ExpectedResult = DbType.Boolean)]
        [TestCase(typeof(string), ExpectedResult = DbType.String)]
        [TestCase(typeof(char), ExpectedResult = DbType.StringFixedLength)]
        [TestCase(typeof(Guid), ExpectedResult = DbType.Guid)]
        [TestCase(typeof(DateTime), ExpectedResult = DbType.DateTime)]
        [TestCase(typeof(DateTimeOffset), ExpectedResult = DbType.DateTimeOffset)]
        [TestCase(typeof(byte?), ExpectedResult = DbType.Byte)]
        [TestCase(typeof(sbyte?), ExpectedResult = DbType.SByte)]
        [TestCase(typeof(short?), ExpectedResult = DbType.Int16)]
        [TestCase(typeof(ushort?), ExpectedResult = DbType.UInt16)]
        [TestCase(typeof(int?), ExpectedResult = DbType.Int32)]
        [TestCase(typeof(uint?), ExpectedResult = DbType.UInt32)]
        [TestCase(typeof(long?), ExpectedResult = DbType.Int64)]
        [TestCase(typeof(ulong?), ExpectedResult = DbType.UInt64)]
        [TestCase(typeof(float?), ExpectedResult = DbType.Single)]
        [TestCase(typeof(double?), ExpectedResult = DbType.Double)]
        [TestCase(typeof(decimal?), ExpectedResult = DbType.Decimal)]
        [TestCase(typeof(bool?), ExpectedResult = DbType.Boolean)]
        [TestCase(typeof(char?), ExpectedResult = DbType.StringFixedLength)]
        [TestCase(typeof(Guid?), ExpectedResult = DbType.Guid)]
        [TestCase(typeof(DateTime?), ExpectedResult = DbType.DateTime)]
        [TestCase(typeof(DateTimeOffset?), ExpectedResult = DbType.DateTimeOffset)]
        [TestCase(null, ExpectedResult = null)]
        [TestCase(typeof(object), ExpectedResult = null)]
        public DbType? TestToDbType(Type type)
        {
            return type.ToDbType();
        }
        [TestCase("a", ExpectedResult = " = ")]
        [TestCase("NULL", ExpectedResult = " IS ")]
        public string TestEqualityOperator(string value)
        {
            return SQLQueryTools.EqualityOperator(value);
        }
        [TestCaseSource(nameof(TestToSqlStringCases), new object[] { nameof(TestToSqlStringCase) })]
        public string TestToSqlStringCase(object o)
        {
            return o.ToSqlString();
        }
        public static IEnumerable<TestCaseData> TestToSqlStringCases(string methodCaller)
        {
            yield return new TestCaseData(null) { ExpectedResult = "NULL" }.SetName($"{methodCaller} (null)");
            yield return new TestCaseData(5) { ExpectedResult = "5" }.SetName($"{methodCaller} (5)");
            yield return new TestCaseData(5.25) { ExpectedResult = "5.25" }.SetName($"{methodCaller} (5.25)");
            yield return new TestCaseData(new DateTime(2024,10,5,13,53,17)) { ExpectedResult = "20241005 13:53:17" }.SetName($"{methodCaller} (05/10/2024 13:53:17)");
            yield return new TestCaseData("") { ExpectedResult = "''" }.SetName($"{methodCaller} (\"\")");
            yield return new TestCaseData("azerty") { ExpectedResult = "'azerty'" }.SetName($"{methodCaller} (\"azerty\")");
            yield return new TestCaseData("abc'cde''aa") { ExpectedResult = "'abc''cde''''aa'" }.SetName($"{methodCaller} (\"abc'cde''aa\")");
            Guid guid = Guid.NewGuid();
            yield return new TestCaseData(guid) { ExpectedResult = $"'{guid}'" }.SetName($"{methodCaller} (guid)");
        }
    }
}