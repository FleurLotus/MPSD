
namespace MockDbData.UnitTests
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Linq;
    using NUnit.Framework;
    [TestFixture]
    public class TestMockDbReader
    {
        [Test]
        public void TestConstuctor()
        {
            Assert.DoesNotThrow(() => new MockDbDataReader(), "No arg should not throw");
            Assert.Throws<ArgumentNullException>(() => new MockDbDataReader(null), "null arg should throw ArgumentNullException");
            Assert.DoesNotThrow(() => new MockDbDataReader(new MockDbResult(new DataTable())), "None null MockDbResult should not throw");
        }
        [Test]
        public void TestCheckClose()
        {
            MockDbDataReader dataReader = new MockDbDataReader();
            Assert.That(dataReader.IsClosed, Is.False, "IsClosed should be false");
            dataReader.Close();
            Assert.That(dataReader.IsClosed, Is.True, "IsClosed should be true");
            CheckThrowCloseException(() => { var o = dataReader[0]; }, "this[int]");
            CheckThrowCloseException(() => { var o = dataReader["aaa"]; }, "this[name]");
            CheckThrowCloseException(() => { var o = dataReader.Depth; }, nameof(MockDbDataReader.Depth));
            CheckThrowCloseException(() => { var o = dataReader.FieldCount; }, nameof(MockDbDataReader.FieldCount));
            CheckThrowCloseException(() => { var o = dataReader.HasRows; }, nameof(MockDbDataReader.HasRows));
            CheckThrowCloseException(() => dataReader.GetBoolean(0), nameof(MockDbDataReader.GetBoolean));
            CheckThrowCloseException(() => dataReader.GetByte(0), nameof(MockDbDataReader.GetByte));
            CheckThrowCloseException(() => dataReader.GetBytes(0, 0, new byte[10], 0, 10), nameof(MockDbDataReader.GetBytes));
            CheckThrowCloseException(() => dataReader.GetChar(0), nameof(MockDbDataReader.GetChar));
            CheckThrowCloseException(() => dataReader.GetChars(0, 0, new char[10], 0, 10), nameof(MockDbDataReader.GetChars));

            CheckThrowCloseException(() => dataReader.GetDateTime(0), nameof(MockDbDataReader.GetDateTime));
            CheckThrowCloseException(() => dataReader.GetDecimal(0), nameof(MockDbDataReader.GetDecimal));
            CheckThrowCloseException(() => dataReader.GetDouble(0), nameof(MockDbDataReader.GetDouble));
            CheckThrowCloseException(() => dataReader.GetFloat(0), nameof(MockDbDataReader.GetFloat));
            CheckThrowCloseException(() => dataReader.GetGuid(0), nameof(MockDbDataReader.GetGuid));
            CheckThrowCloseException(() => dataReader.GetInt16(0), nameof(MockDbDataReader.GetInt16));
            CheckThrowCloseException(() => dataReader.GetInt32(0), nameof(MockDbDataReader.GetInt32));
            CheckThrowCloseException(() => dataReader.GetInt64(0), nameof(MockDbDataReader.GetInt64));
            CheckThrowCloseException(() => dataReader.GetName(0), nameof(MockDbDataReader.GetName));
            CheckThrowCloseException(() => dataReader.GetString(0), nameof(MockDbDataReader.GetString));
            CheckThrowCloseException(() => dataReader.GetValue(0), nameof(MockDbDataReader.GetValue));
            CheckThrowCloseException(() => dataReader.GetValues(new object[10]), nameof(MockDbDataReader.GetValues));
            CheckThrowCloseException(() => dataReader.IsDBNull(0), nameof(MockDbDataReader.IsDBNull));
            CheckThrowCloseException(() => dataReader.NextResult(), nameof(MockDbDataReader.NextResult));
            CheckThrowCloseException(() => dataReader.Read(), nameof(MockDbDataReader.Read));
            CheckThrowCloseException(() => dataReader.GetFieldValue<object>(0), nameof(MockDbDataReader.GetFieldValue));
        }

        [Test]
        public void TestHasData()
        {
            MockDbDataReader dataReader = new MockDbDataReader();
            CheckThrowNoDateException(() => { var o = dataReader[0]; }, "this[int]");
            CheckThrowNoDateException(() => { var o = dataReader["aaa"]; }, "this[name]");
            Assert.DoesNotThrow(() => { var o = dataReader.Depth; }, nameof(MockDbDataReader.Depth));
            Assert.DoesNotThrow(() => { var o = dataReader.FieldCount; }, nameof(MockDbDataReader.FieldCount));
            Assert.DoesNotThrow(() => { var o = dataReader.HasRows; }, nameof(MockDbDataReader.HasRows));
            CheckThrowNoDateException(() => dataReader.GetBoolean(0), nameof(MockDbDataReader.GetBoolean));
            CheckThrowNoDateException(() => dataReader.GetByte(0), nameof(MockDbDataReader.GetByte));
            CheckThrowNoDateException(() => dataReader.GetBytes(0, 0, new byte[10], 0, 10), nameof(MockDbDataReader.GetBytes));
            CheckThrowNoDateException(() => dataReader.GetChar(0), nameof(MockDbDataReader.GetChar));
            CheckThrowNoDateException(() => dataReader.GetChars(0, 0, new char[10], 0, 10), nameof(MockDbDataReader.GetChars));
            CheckThrowNoDateException(() => dataReader.GetDateTime(0), nameof(MockDbDataReader.GetDateTime));
            CheckThrowNoDateException(() => dataReader.GetDecimal(0), nameof(MockDbDataReader.GetDecimal));
            CheckThrowNoDateException(() => dataReader.GetDouble(0), nameof(MockDbDataReader.GetDouble));
            CheckThrowNoDateException(() => dataReader.GetFloat(0), nameof(MockDbDataReader.GetFloat));
            CheckThrowNoDateException(() => dataReader.GetGuid(0), nameof(MockDbDataReader.GetGuid));
            CheckThrowNoDateException(() => dataReader.GetInt16(0), nameof(MockDbDataReader.GetInt16));
            CheckThrowNoDateException(() => dataReader.GetInt32(0), nameof(MockDbDataReader.GetInt32));
            CheckThrowNoDateException(() => dataReader.GetInt64(0), nameof(MockDbDataReader.GetInt64));
            Assert.DoesNotThrow(() => dataReader.GetName(0), nameof(MockDbDataReader.GetName));
            CheckThrowNoDateException(() => dataReader.GetString(0), nameof(MockDbDataReader.GetString));
            CheckThrowNoDateException(() => dataReader.GetValue(0), nameof(MockDbDataReader.GetValue));
            CheckThrowNoDateException(() => dataReader.GetValues(new object[10]), nameof(MockDbDataReader.GetValues));
            CheckThrowNoDateException(() => dataReader.IsDBNull(0), nameof(MockDbDataReader.IsDBNull));
            Assert.DoesNotThrow(() => dataReader.NextResult(), nameof(MockDbDataReader.NextResult));
            Assert.DoesNotThrow(() => dataReader.Read(), nameof(MockDbDataReader.Read));
            CheckThrowNoDateException(() => dataReader.GetFieldValue<object>(0), nameof(MockDbDataReader.GetFieldValue));
        }

        [Test]
        public void TestMethodParam()
        {
            MockDbDataReader dataReader = new MockDbDataReader();
            Assert.Throws<ArgumentNullException>(() => dataReader.GetValues(null), "Should throw ArgumentNullException when values is null for GetValues");
            Assert.Throws<ArgumentNullException>(() => dataReader.GetBytes(0, 0, null, 0, 1), "Should throw ArgumentNullException when buffer is null for GetBytes");
            Assert.Throws<ArgumentNullException>(() => dataReader.GetChars(0, 0, null, 0, 1), "Should throw ArgumentNullException when buffer is null for GetChars");
        }

        [Test]
        public void TestFieldCount()
        {
            MockDbDataReader dataReader;
            DataTable dataTable = new DataTable();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.FieldCount, Is.EqualTo(dataTable.Columns.Count));

            dataTable.Columns.Add("Col1", typeof(string));
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.FieldCount, Is.EqualTo(dataTable.Columns.Count));

            dataTable.Columns.Add("Col2", typeof(string));
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.FieldCount, Is.EqualTo(dataTable.Columns.Count));

            dataTable.Columns.Add("Col3", typeof(string));
            dataTable.Columns.Add("Col4", typeof(string));
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.FieldCount, Is.EqualTo(dataTable.Columns.Count));
        }
        [Test]
        public void TestGetOrdinal()
        {
            DataTable dataTable = BuildEmptySample();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                Assert.That(dataReader.GetOrdinal(dataTable.Columns[col].ColumnName), Is.EqualTo(col), $"Not the expected value for column {col}");
            }
            Assert.That(dataReader.GetOrdinal("Col5"), Is.EqualTo(-1));
        }
        [Test]
        public void TestGetDataTypeName()
        {
            DataTable dataTable = BuildEmptySample();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                Assert.That(dataReader.GetDataTypeName(col), Is.EqualTo(dataTable.Columns[col].DataType.ToString()), $"Not the expected value for column {col}");
            }
        }
        [Test]
        public void TestGetFieldType()
        {
            DataTable dataTable = BuildEmptySample();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                Assert.That(dataReader.GetFieldType(col), Is.EqualTo(dataTable.Columns[col].DataType), $"Not the expected value for column {col}");
            }
        }
        [Test]
        public void TestGetName()
        {
            DataTable dataTable = BuildEmptySample();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                Assert.That(dataReader.GetName(col), Is.EqualTo(dataTable.Columns[col].ColumnName), $"Not the expected value for column {col}");
            }
        }
        [Test]
        public void TestGet()
        {
            DataTable dataTable = BuildSampleWithMostDataType();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                int col = -1;
                Assert.That(dataReader.GetBoolean(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetByte(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetChar(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetDateTime(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetDecimal(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetDouble(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetFloat(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetGuid(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetInt16(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetInt32(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetInt64(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetString(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                Assert.That(dataReader.GetValue(++col), Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }
        [Test]
        public void TestGetValues()
        {
            DataTable dataTable = BuildSampleWithMostDataType();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                object[] data = new object[dataTable.Columns.Count];
                Assert.That(dataReader.GetValues(data), Is.EqualTo(data.Length), $"Not the expected number of value read for row {row}");
                Assert.That(data, Is.EqualTo(dataTable.Rows[row].ItemArray), $"Not the expected value for row {row}");
            }

            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }

        [Test]
        public void TestGetBytes()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col0", typeof(byte[]));
            dataTable.Columns.Add("Col1", typeof(byte[]));
            byte[] source = new byte[] { 5, 3, 2, 0, 10, 25 };
            dataTable.Rows.Add(new byte[1], source);
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.Read(), Is.True, $"Should be True");
            byte[] data = new byte[40];
            int offset = 0;
            Assert.That(dataReader.GetBytes(1, 0, data, offset, source.Length), Is.EqualTo(source.Length), "Not the expected number of value read for full");
            Assert.That(data.Take(source.Length), Is.EqualTo(source));

            offset = 10;
            Assert.That(dataReader.GetBytes(1, 0, data, offset, source.Length), Is.EqualTo(source.Length), "Not the expected number of value read for offset");
            Assert.That(data.Skip(offset).Take(source.Length), Is.EqualTo(source));
            offset = 20;
            int subreadlength = 2;
            Assert.That(dataReader.GetBytes(1, 0, data, offset, subreadlength), Is.EqualTo(subreadlength), "Not the expected number of value read for length");
            Assert.That(data.Skip(offset).Take(source.Length), Is.EqualTo(source.Take(subreadlength).Concat(new byte[source.Length - subreadlength])));

            offset = 30;
            int sourceoffset = 1;
            Assert.That(dataReader.GetBytes(1, sourceoffset, data, offset, source.Length - sourceoffset), Is.EqualTo(source.Length - sourceoffset), "Not the expected number of value read for dataoffset");
            Assert.That(data.Skip(offset).Take(source.Length - sourceoffset), Is.EqualTo(source.Skip(sourceoffset)));
        }

        [Test]
        public void TestGetChars()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col0", typeof(char[]));
            dataTable.Columns.Add("Col1", typeof(char[]));

            char[] source = new char[] { 'a', '2', '\t', '$', '*', ' ' };
            dataTable.Rows.Add(new char[5], source);

            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);

            Assert.That(dataReader.Read(), Is.True, $"Should be True");
            char[] data = new char[40];

            int offset = 0;
            Assert.That(dataReader.GetChars(1, 0, data, offset, source.Length), Is.EqualTo(source.Length), "Not the expected number of value read for full");
            Assert.That(data.Take(source.Length), Is.EqualTo(source));

            offset = 10;
            Assert.That(dataReader.GetChars(1, 0, data, offset, source.Length), Is.EqualTo(source.Length), "Not the expected number of value read for offset");
            Assert.That(data.Skip(offset).Take(source.Length), Is.EqualTo(source));

            offset = 20;
            int subreadlength = 2;
            Assert.That(dataReader.GetChars(1, 0, data, offset, subreadlength), Is.EqualTo(subreadlength), "Not the expected number of value read for length");
            Assert.That(data.Skip(offset).Take(source.Length), Is.EqualTo(source.Take(subreadlength).Concat(new char[source.Length - subreadlength])));

            offset = 30;
            int sourceoffset = 1;
            Assert.That(dataReader.GetChars(1, sourceoffset, data, offset, source.Length - sourceoffset), Is.EqualTo(source.Length - sourceoffset), "Not the expected number of value read for dataoffset");
            Assert.That(data.Skip(offset).Take(source.Length - sourceoffset), Is.EqualTo(source.Skip(sourceoffset)));
        }
        [Test]
        public void TestIsNull()
        {
            DataTable dataTable = BuildEmptySample();
            dataTable.Rows.Add(null, 5, 3.5, null);
            dataTable.Rows.Add("sdfghjkl", null, 4.0, new DateTime());
            dataTable.Rows.Add("", 12, null, new DateTime(2022, 5, 31));
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    Assert.That(dataReader.IsDBNull(col), Is.EqualTo(dataTable.Rows[row].IsNull(col)), $"Not the expected value for row {row} column {col}");
                }
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }

        [Test]
        public void TestThisInt()
        {
            DataTable dataTable = BuildSampleWithMostDataType();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    Assert.That(dataReader[col], Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                }
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }
        [Test]
        public void TestThisString()
        {
            DataTable dataTable = BuildSampleWithMostDataType();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    string name = dataTable.Columns[col].ColumnName;
                    Assert.That(dataReader[name], Is.EqualTo(dataTable.Rows[row][name]), $"Not the expected value for row {row} column {col} => {name}");
                }
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }

        [Test]
        public void TestNextResult()
        {
            MockDbDataReader dataReader;
            MockDbResult mockDbResult;
            //No data
            dataReader = new MockDbDataReader();
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with no table");

            //One table no data read
            DataTable dataTable = BuildSampleWithMostDataType();
            mockDbResult = new MockDbResult(dataTable);
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with one table no data read");

            //One table some data read
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.Read(), Is.True, $"Should be True");
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with one table some data read");

            //One table all data read
            dataReader = new MockDbDataReader(mockDbResult);
            while (dataReader.Read()) { }
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with one table all data read");

            //Multi tables no data read
            DataTable[] dataTables = new DataTable[] { BuildSampleWithMostDataType(), BuildEmptySample(), new DataTable() };
            mockDbResult = new MockDbResult(dataTables);
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on first call");
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on second call");
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with three table on third call");

            //Multi table some data read
            dataTable = BuildSampleWithMostDataType();
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.Read(), Is.True, $"Should be True");
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on first call");
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on second call");
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with three table on third call");

            //Multi table all data read
            dataTable = BuildSampleWithMostDataType();
            dataReader = new MockDbDataReader(mockDbResult);
            while (dataReader.Read()) { }
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on first call");
            Assert.That(dataReader.NextResult, Is.True, $"Should be True with three table on second call");
            Assert.That(dataReader.NextResult, Is.False, $"Should be False with three table on third call");
        }

        [Test]
        public void TestDataConstistentAfterNextResultWithMultipleResult()
        {
            DataTable dataTable1 = BuildSampleWithMostDataType();
            DataTable dataTable2 = BuildEmptySample();
            dataTable2.Rows.Add(null, 5, 3.5, null);
            dataTable2.Rows.Add("sdfghjkl", null, 4.0, new DateTime());
            dataTable2.Rows.Add("", 12, null, new DateTime(2022, 5, 31));

            DataTable[] dataTables = new DataTable[] { dataTable1, dataTable2 };
            MockDbResult mockDbResult = new MockDbResult(dataTables);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);

            CheckStructure(dataTable1, dataReader, "table1");
            Assert.That(dataReader.NextResult, Is.True, $"Should be True");
            CheckStructure(dataTable2, dataReader, "table2");

            Assert.That(dataReader.NextResult, Is.False, $"Should be False");
        }
        [Test]
        public void TestHasRow()
        {
            MockDbDataReader dataReader;
            MockDbResult mockDbResult;

            //No data
            dataReader = new MockDbDataReader();
            Assert.That(dataReader.HasRows, Is.False, $"Should be False with no table");

            //One table no data
            DataTable dataTable = BuildEmptySample();
            mockDbResult = new MockDbResult(dataTable);
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.HasRows, Is.False, $"Should be False with one table no data");

            //One table some data no read
            dataTable = BuildSampleWithMostDataType();
            mockDbResult = new MockDbResult(dataTable);
            dataReader = new MockDbDataReader(mockDbResult);
            Assert.That(dataReader.HasRows, Is.True, $"Should be True with one table with data before read");
            Assert.That(dataReader.Read(), Is.True, $"Should be True");
            Assert.That(dataReader.HasRows, Is.True, $"Should be True with one table with data after read");
            while (dataReader.Read()) { }
            Assert.That(dataReader.HasRows, Is.True, $"Should be True with one table with data at the end");
        }
        [Test]
        public void TestGetEnumerator()
        {
            DataTable dataTable = BuildSampleWithMostDataType();
            MockDbResult mockDbResult = new MockDbResult(dataTable);
            MockDbDataReader dataReader = new MockDbDataReader(mockDbResult);
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row}");
                IEnumerator enumerator = dataReader.GetEnumerator();
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {

                    Assert.That(enumerator.MoveNext(), Is.True, $"Should be True for Rows {row} column {col}");
                    Assert.That(enumerator.Current, Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for row {row} column {col}");
                }
                Assert.That(enumerator.MoveNext(), Is.False, $"Should be False for Rows {row} at the end");
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward");
        }
        private static DataTable BuildSampleWithMostDataType()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1", typeof(bool));
            dataTable.Columns.Add("Col2", typeof(byte));
            dataTable.Columns.Add("Col3", typeof(char));
            dataTable.Columns.Add("Col4", typeof(DateTime));
            dataTable.Columns.Add("Col5", typeof(decimal));
            dataTable.Columns.Add("Col6", typeof(double));
            dataTable.Columns.Add("Col7", typeof(float));
            dataTable.Columns.Add("Col8", typeof(Guid));
            dataTable.Columns.Add("Col9", typeof(short));
            dataTable.Columns.Add("Col10", typeof(int));
            dataTable.Columns.Add("Col11", typeof(long));
            dataTable.Columns.Add("Col12", typeof(string));
            dataTable.Columns.Add("Col13", typeof(object));
            dataTable.Rows.Add(false, (byte)2, 'a', new DateTime(2022, 10, 8), 3.5m, 7.2d, 5.32f, Guid.NewGuid(), (short)42, 256, 10254232L, "sdfghjk", "ffghhj");
            dataTable.Rows.Add(true, (byte)5, 'z', new DateTime(2022, 11, 8), 4.5m, 8.2d, 95.32f, Guid.NewGuid(), (short)3, 12, 1234567890L, "xcvbn,", null);
            dataTable.Rows.Add(false, (byte)10, '5', new DateTime(2022, 12, 8), 5.5m, 9.2d, 52.32f, Guid.NewGuid(), (short)21, 69, 11111L, "hh hhaa", 12.5);
            return dataTable;
        }

        private static DataTable BuildEmptySample()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1", typeof(string));
            dataTable.Columns.Add("Col2", typeof(int));
            dataTable.Columns.Add("Col3", typeof(double));
            dataTable.Columns.Add("Col4", typeof(DateTime));
            return dataTable;
        }
        private static void CheckStructure(DataTable dataTable, MockDbDataReader dataReader, string name)
        {
            //Columns Count
            Assert.That(dataReader.FieldCount, Is.EqualTo(dataTable.Columns.Count), $"Not the expected value for FieldCount for table {name}");
            Assert.That(dataReader.HasRows, Is.EqualTo(dataTable.Rows.Count > 0), $"Not the expected value for HasRows for table {name}");
            //Columns type/name
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                Assert.That(dataReader.GetFieldType(col), Is.EqualTo(dataTable.Columns[col].DataType), $"Not the expected value for column Type {col} for table {name}");
                Assert.That(dataReader.GetName(col), Is.EqualTo(dataTable.Columns[col].ColumnName), $"Not the expected value for column Name {col} for table {name}");
            }
            //Rows/Value
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                Assert.That(dataReader.Read(), Is.True, $"Should be True for Rows {row} for table {name}");
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    Assert.That(dataReader.IsDBNull(col), Is.EqualTo(dataTable.Rows[row].IsNull(col)), $"Not the expected value for IsNull row {row} column {col} for table {name}");
                    Assert.That(dataReader[col], Is.EqualTo(dataTable.Rows[row][col]), $"Not the expected value for Value row {row} column {col} for table {name}");
                }
            }
            Assert.That(dataReader.Read(), Is.False, $"Should be False afterward for table {name}");
        }

        private static void CheckThrowCloseException(TestDelegate testDelegate, string method)
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(testDelegate, $"Expecting InvalidOperationException while calling {method}");
            Assert.That(ex.Message, Is.EqualTo("Could not access while reader is closed"), $"Not the expected exception while calling {method}");
        }

        private static void CheckThrowNoDateException(TestDelegate testDelegate, string method)
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(testDelegate, "Expecting InvalidOperationException while calling " + method);
            Assert.That(ex.Message, Is.EqualTo("no data"), "Not the expected exception while calling " + method);
        }
    }
}