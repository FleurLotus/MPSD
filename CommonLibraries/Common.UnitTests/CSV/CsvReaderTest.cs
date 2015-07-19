namespace Common.UnitTests.CSV
{

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    using Common.Library.CSV;
    using Common.Library.Exception;

    using NUnit.Framework;

    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class CsvReaderTest
    {

        #region Args Tests
        [Test]
        public void TestNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvReader((string)null, false), "Null string arg should throw ArgumentNullException");
        }
        [Test]
        public void TestNullStreamArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvReader((Stream)null, false), "Null stream arg should throw ArgumentNullException");
        }
        [Test]
        public void TestUnexistingPathArgument()
        {
            Assert.Throws<FileNotFoundException>(() => new CsvReader("degdsqhcgscsdhj", false), "Unexisting file arg should throw FileNotFoundException");
        }
        [Test]
        public void TestInvalidSeparatorArgument()
        {
            Assert.Throws<ArgumentException>(() => new CsvReader(new MemoryStream(), true, '"'), " \" separator should throw ArgumentException");
        }
        #endregion

        #region Behavior
        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void TestCheckDisposed()
        {
            ICsvReader reader = new CsvReader(new MemoryStream(), false);
            reader.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { bool b = reader.WithHeader; }, "Access to WithHeader should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => { char c = reader.Separator; }, "Access to Separator should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => reader.GetCurrentLine(), "Access to GetCurrentLine() should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => reader.GetColumnsCount(), "Access to GetColumnsCount() should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => reader.GetHeaders(), "Access to GetHeaders() should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => reader.GetValue(0), "Access to GetValue() should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => { bool b = reader.EndOfStream; }, "Access to EndOfStream should throw ObjectDisposedException after call to Dispose");
            Assert.Throws<ObjectDisposedException>(() => reader.Read(), "Access to Read() should throw ObjectDisposedException after call to Dispose");
        }
        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void TestCheckNoRead([Values(true, false)] bool withHeader)
        {
            const string input = "azerty\nazerty";

            using (ICsvReader reader = CreateCsvReader(input, withHeader))
            {
                Assert.DoesNotThrow(() => { bool b = reader.WithHeader; }, "Access to WithHeader should not throw exception before call to Read");
                Assert.DoesNotThrow(() => { char c = reader.Separator; }, "Access to Separator should not throw exception before call to Read");
                Assert.DoesNotThrow(() => reader.GetCurrentLine(), "Access to GetCurrentLine() should not throw exception before call to Read");
                if (withHeader)
                {
                    Assert.DoesNotThrow(() => reader.GetColumnsCount(), "Access to GetColumnsCount should not throw exception before call to Read");
                }
                else
                {
                    Assert.Throws<CsvReaderNoReadCallException>(() => reader.GetColumnsCount(), "Access to GetColumnsCount should throw CsvReaderNoReadCallException before call to Read");
                }
                Assert.DoesNotThrow(() => reader.GetHeaders(), "Access to GetHeaders() should not throw exception before call to Read");
                Assert.Throws<CsvReaderNoReadCallException>(() =>  reader.GetValue(0), "Access to GetValue should throw CsvReaderNoReadCallException before call to Read");
                Assert.DoesNotThrow(() => { bool b = reader.EndOfStream; }, "Access to EndOfStream should not throw exception before call to Read");

                Assert.DoesNotThrow(() => reader.Read(), "Call to Read() should not throw exception");

                Assert.DoesNotThrow(() => { bool b = reader.WithHeader; }, "Access to WithHeader should not throw exception after call to Read");
                Assert.DoesNotThrow(() => { char c = reader.Separator; }, "Access to Separator should not throw exception after call to Read");
                Assert.DoesNotThrow(() => reader.GetCurrentLine(), "Access to GetCurrentLine() should not throw exception after call to Read");
                Assert.DoesNotThrow(() => reader.GetColumnsCount(), "Access to GetColumnsCount should not throw exception  after call to Read");
                Assert.DoesNotThrow(() => reader.GetHeaders(), "Access to GetHeaders() should not throw exception after call to Read");
                Assert.DoesNotThrow(() => reader.GetValue(0), "Access to GetValue should not throw exception  after call to Read");
                Assert.DoesNotThrow(() => { bool b = reader.EndOfStream; }, "Access to EndOfStream should not throw exception before after to Read");
            }
        }
        [Test]
        public void TestCheckNumberOfColumns([Values(true, false)] bool withHeader)
        {
            const string input = "azerty\nazerty,azerty";

            using (ICsvReader reader = CreateCsvReader(input, withHeader))
            {
                if (!withHeader)
                {
                    Assert.IsTrue(reader.Read(), "Can't read first line");
                }

                Assert.Throws<CsvReaderWrongNumberOfColumnsException>(() => reader.Read(), "Read should throw CsvReaderWrongNumberOfColumnsException when line has not the expected number of colums");
                TestErrorState(reader, "CsvReaderWrongNumberOfColumnsException");
            }
        }
        [Test]
        public void TestGetCurrentLine([Values(true, false)] bool withHeader)
        {
            const int count = 10;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append("a\r\n");
            }

            using (ICsvReader reader = CreateCsvReader(sb.ToString(), withHeader))
            {
                int expectedLine = -1;
                Assert.IsTrue(reader.GetCurrentLine() == expectedLine, "No the expected current line");
                //
                for (int i = 0; i < (count - (withHeader ? 1 : 0)); i++)
                {
                    Assert.IsTrue(reader.Read(), "Can't read!");
                    expectedLine++;
                    Assert.IsTrue(reader.GetCurrentLine() == expectedLine, "No the expected current line");
                }

                Assert.IsFalse(reader.Read(), "Should be end of file");
                Assert.IsTrue(reader.GetCurrentLine() == expectedLine, "Current must not change after and of file");
            }
        }
        [Test]
        public void TestGetHeaders([Values(true, false)] bool withHeader)
        {
            const string input = "azerty,azerty\r\ntttt,ttttt";

            string[] exceptedResult = withHeader ? new[] { "azerty", "azerty" } : new string[0];
            
            using (ICsvReader reader = CreateCsvReader(input, withHeader))
            {
                string[] header = reader.GetHeaders();
                Assert.AreEqual(exceptedResult.Length, header.Length, "Not the excepted length");
                for (int i = 0; i < exceptedResult.Length; i++)
                {
                    Assert.AreEqual(exceptedResult[i], header[i], "Not the excepted value at i={0}", i);
                }
                Assert.IsTrue(reader.Read(), "Can't read first line");
                header = reader.GetHeaders();
                Assert.AreEqual(exceptedResult.Length, header.Length, "Not the excepted length after read");
                for (int i = 0; i < exceptedResult.Length; i++)
                {
                    Assert.AreEqual(exceptedResult[i], header[i], "Not the excepted value at i={0} after read", i);
                }
            }
        }
        [Test]
        public void TestGetColumnsCount([Values(true, false)] bool withHeader)
        {
            const string input = "azerty,azerty\r\ntttt,ttttt";

            using (ICsvReader reader = CreateCsvReader(input, withHeader))
            {
                if (withHeader)
                {
                    Assert.AreEqual(2, reader.GetColumnsCount(), "Not the excepted ColumnsCount before call to Read");
                }
                else
                {
                    Assert.Throws<CsvReaderNoReadCallException>(() => reader.GetColumnsCount(), "Access to GetColumnsCount should throw CsvReaderNoReadCallException before call to Read");
                }
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual(2, reader.GetColumnsCount(), "Not the excepted ColumnsCount after call to Read");
            }
        }
        [Test]
        public void TestGetValueIndex()
        {
            const string input = "azerty,azerty";

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.Throws<IndexOutOfRangeException>(() => reader.GetValue(-1), "GetValue(-1) should thrown IndexOutOfRangeException");
                Assert.DoesNotThrow(() => reader.GetValue(0), "GetValue(0) should not thrown IndexOutOfRangeException");
                Assert.DoesNotThrow(() => reader.GetValue(1), "GetValue(1) should not thrown IndexOutOfRangeException");
                Assert.Throws<IndexOutOfRangeException>(() => reader.GetValue(2), "GetValue(2) should thrown IndexOutOfRangeException");
            }
        }
        #endregion

        #region Parameters
        [Test]
        public void TestAllKindOfEndLineAndFile([Values("\n", "\r", "\r\n")] string eol1, [Values("\n", "\r", "\r\n")] string eol2, [Values("\n", "\r", "\r\n", null, "")] string eof)
        {
            string input = string.Format("azerty,azerty2{0}tttt,tttt2{1}aaaaa,aaaaa2{2}", eol1, eol2, eof);

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual("azerty", reader.GetValue(0), "Not the excepted value for line 0 column 0");
                Assert.AreEqual("azerty2", reader.GetValue(1), "Not the excepted value for line 0 column 1");
                Assert.IsTrue(reader.Read(), "Can't read second line");
                Assert.AreEqual("tttt", reader.GetValue(0), "Not the excepted value for line 1 column 0");
                Assert.AreEqual("tttt2", reader.GetValue(1), "Not the excepted value for line 1 column 1");
                Assert.IsTrue(reader.Read(), "Can't read third line");
                Assert.AreEqual("aaaaa", reader.GetValue(0), "Not the excepted value for line 2 column 0");
                Assert.AreEqual("aaaaa2", reader.GetValue(1), "Not the excepted value for line 2 column 1");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
        }
        [Test]
        public void TestChangeSeparator([Values('\t', ';', ',', ':', '.', 's')] char separator)
        {
            byte[] data = Encoding.Default.GetBytes(string.Format("azerty{0}azerty2\ntttt{0}tttt2\naaaaa{0}aaaaa2", separator));

            using (ICsvReader reader = new CsvReader(new MemoryStream(data), false, separator))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual("azerty", reader.GetValue(0), "Not the excepted value for line 0 column 0");
                Assert.AreEqual("azerty2", reader.GetValue(1), "Not the excepted value for line 0 column 1");
                Assert.IsTrue(reader.Read(), "Can't read second line");
                Assert.AreEqual("tttt", reader.GetValue(0), "Not the excepted value for line 1 column 0");
                Assert.AreEqual("tttt2", reader.GetValue(1), "Not the excepted value for line 1 column 1");
                Assert.IsTrue(reader.Read(), "Can't read third line");
                Assert.AreEqual("aaaaa", reader.GetValue(0), "Not the excepted value for line 2 column 0");
                Assert.AreEqual("aaaaa2", reader.GetValue(1), "Not the excepted value for line 2 column 1");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
        }
        #endregion

        #region Parsing
        [Test]
        public void TestBadClosingEscape()
        {
            const string input = "azerty,azerty,\"azert\" ,azert";

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                Assert.Throws<CsvReaderBadClosingEscapeException>(() => reader.Read(), "Read(2) should thrown CsvReaderBadClosingEscapeException");
                TestErrorState(reader, "CsvReaderBadClosingEscapeException");
            }
        }
        [Test]
        public void TestUnclosedEscape()
        {
            const string input = "azerty,azerty,\"azert";

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                Assert.Throws<CsvReaderUnclosedEscapeException>(() => reader.Read(), "Read() should thrown CsvReaderUnclosedEscapeException");
                TestErrorState(reader, "CsvReaderUnclosedEscapeException");
            }
        }
        [Test]
        public void TestEscapingSimple()
        {
            const string input = "azerty00,\"azerty01\",\"azerty02\"\r\"azerty10\",azerty11,\"azerty12\"\n\"azerty20\",\"azerty21\",azerty22\r\n\"azerty30\",\"azerty31\",\"azerty32\"";

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                for (int l = 0; l < 4; l++)
                {
                    Assert.IsTrue(reader.Read(), "Can't read line {0}", l);
                    for (int f = 0; f < 3; f++)
                    {
                        Assert.AreEqual(string.Format("azerty{0}{1}", l, f), reader.GetValue(f), "Not the excepted value for line {0} column {1}", l, f);
                    }
                }
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
        }
        [Test]
        public void TestEscapingWithSpecialCharacters()
        {
            string[] specialChars = { " ", "_", "\t", "\r", "\n", "\r\n", ",", "\"" };
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < specialChars.Length; index++)
            {
                string s = specialChars[index];
                if (index != 0)
                {
                    sb.Append(',');
                }
                sb.AppendFormat("\"azerty{0}azerty\"", s.Replace("\"", "\"\""));
            }

            using (ICsvReader reader = CreateCsvReader(sb.ToString(), false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                for (int index = 0; index < specialChars.Length; index++)
                {
                    Assert.AreEqual(string.Format("azerty{0}azerty", specialChars[index]), reader.GetValue(index), "Not the excepted value for column {0}", index);
                }
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
        }
        [Test]
        public void TestEscapingMultiQuote()
        {
            // '"'
            using (ICsvReader reader = CreateCsvReader("\"", false))
            {
                Assert.Throws<CsvReaderUnclosedEscapeException>(() => reader.Read(), "Read() should thrown CsvReaderUnclosedEscapeException for 1 DQuote");
            }
            // '""'
            using (ICsvReader reader = CreateCsvReader("\"\"", false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual(string.Empty, reader.GetValue(0), "Not the excepted value");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
            // '"""'
            using (ICsvReader reader = CreateCsvReader("\"\"\"", false))
            {
                Assert.Throws<CsvReaderUnclosedEscapeException>(() => reader.Read(), "Read() should thrown CsvReaderUnclosedEscapeException for 3 DQuote");
            }
            // '""""'
            using (ICsvReader reader = CreateCsvReader("\"\"\"\"", false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual("\"", reader.GetValue(0), "Not the excepted value");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
            // '"""""'
            using (ICsvReader reader = CreateCsvReader("\"\"\"\"\"", false))
            {
                Assert.Throws<CsvReaderUnclosedEscapeException>(() => reader.Read(), "Read() should thrown CsvReaderUnclosedEscapeException for 5 DQuote");
            }
            // '""""""'
            using (ICsvReader reader = CreateCsvReader("\"\"\"\"\"\"", false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual("\"\"", reader.GetValue(0), "Not the excepted value");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
            // ReSharper restore UnusedVariable
        }
        [Test]
        public void TestNoEscapingWhenFirstCharIsNotEscapingChare()
        {
            const string input = " \"azerty\",t\"\",\t\"\n";

            using (ICsvReader reader = CreateCsvReader(input, false))
            {
                Assert.IsTrue(reader.Read(), "Can't read first line");
                Assert.AreEqual(" \"azerty\"", reader.GetValue(0), "Not the excepted value for column 0");
                Assert.AreEqual("t\"\"", reader.GetValue(1), "Not the excepted value for column 1");
                Assert.AreEqual("\t\"", reader.GetValue(2), "Not the excepted value for column 2");
                Assert.IsFalse(reader.Read(), "Must be end of file");
            }
        }
        #endregion
        
        private ICsvReader CreateCsvReader(string input, bool withHeader)
        {
            byte[] data = Encoding.Default.GetBytes(input);

            return new CsvReader(new MemoryStream(data), withHeader);
        }
        private void TestErrorState(ICsvReader reader, string exceptionName)
        {
            Assert.Throws<CsvReaderErrorStateException>(() => reader.Read(), "Read should throw CsvReaderErrorStateException when call Read after {0}", exceptionName);
            Assert.Throws<CsvReaderErrorStateException>(() =>reader.GetValue(0), "GetValue() should throw CsvReaderErrorStateException when call Read after {0}", exceptionName);
        }

    }
}
