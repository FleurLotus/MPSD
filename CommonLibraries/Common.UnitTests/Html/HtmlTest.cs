namespace Common.UnitTests.Html
{
    using System;

    using Common.Library.Exception;
    using Common.Library.Html;

    using NUnit.Framework;

    [TestFixture]
    public class HtmlTest
    {
        [Test]
        public void TestNotTableTag()
        {
            IHtmlTable ret = HtmlTableParser.Parse("azertyuiop");
            Assert.IsNull(ret, "ret must be empty");
        }
        [Test]
        public void TestMultipleTableTag()
        {
            Assert.Throws<HtmlTableParserMultiTableException>(() => HtmlTableParser.Parse("<TABLE><TABLE>"), "A HtmlTableParserMultiTableException should have be thrown");
        }
        [Test]
        public void TestNoCloseTableTag()
        {
            Assert.Throws<HtmlTableParserNoTableClosingTagException>(() => HtmlTableParser.Parse("<TABLE>skdsdkskdksd"), "A HtmlTableParserNoTableClosingTagException should have be thrown");
        }
        [Test]
        public void TestNoCloseRowTag()
        {
            Assert.Throws<HtmlTableParserNoRowClosingTagException>(() => HtmlTableParser.Parse("<TABLE><TR></TABLE>"), "A HtmlTableParserNoRowClosingTagException should have be thrown");
        }
        [Test]
        public void TestNoCloseCellTag()
        {
            Assert.Throws<HtmlTableParserNoCellClosingTagException>(() => HtmlTableParser.Parse("<TABLE><TR><TD></TR></TABLE>"), "A HtmlTableParserNoCellClosingTagException should have be thrown");
        }
        [Test]
        public void TestNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new HtmlTable(null), "Null argument should have thrown ArgumentNullException");
        }
        [Test]
        public void TestInvalidCell()
        {
            Assert.Throws<HtmlTableParserNoTagEndException>(() => HtmlTableParser.ExtractCell("<td"), "Should throw HtmlTableParserNoTagEndException");
        }
        [Test]
        public void TestGetColCount()
        {
            IHtmlTable ret = HtmlTableParser.Parse(@"<TABLE><TR><TH>H1</TH><TH>H2</TH></TR><TR><TD>C11</TD><TD>C12</TD></TR></TABLE>");
            Assert.IsNotNull(ret, "ret must be not null");

            Assert.AreEqual(ret.GetColCount(-1), 0, "Negative index for Row should return 0");
            Assert.AreEqual(ret.GetColCount(2), 0, "Too large index for Row should return 0");

            Assert.AreEqual(ret.GetColCount(0), 2, "Not the expected value for GetColCount");
            Assert.AreEqual(ret.GetColCount(1), 2, "Not the expected value for GetColCount");
        }

        [Test]
        public void TestRangeGet()
        {
            IHtmlTable ret = HtmlTableParser.Parse(@"<TABLE><TR><TH>H1</TH><TH>H2</TH></TR><TR><TD>C11</TD><TD>C12</TD></TR></TABLE>");
            Assert.IsNotNull(ret, "ret must be not null");
            
            Assert.IsNull(ret[-1, 0], "Negative index for Row should return null");
            Assert.IsNull(ret[0, -1], "Negative index for Col should return null");
            Assert.IsNull(ret[2, 0], "Too large index for Row should return null");
            Assert.IsNull(ret[0, 2], "Too large index for Col should return null");

            Assert.IsNotNull(ret[0,0] , "In range index for Row and Col should not return null");
            Assert.IsNotNull(ret[1,0] , "In range index for Row and Col should not return null");
            Assert.IsNotNull(ret[0,1] , "In range index for Row and Col should not return null");
            Assert.IsNotNull(ret[1,1] , "In range index for Row and Col should not return null");

        }

        #region TestCase List
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 0, 25)]
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 11, 25)]
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 26, -1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 0, -1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 11, -1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 20, 26)]
        [TestCase("azertyuiop <a azertyuiop / <azertyuiop", 0, -1)]
        [TestCase("azertyuiop <a azertyuiop / <azertyuiop", 11, -1)]
        [TestCase("azertyuiop <a azertyuiop / < azertyuiop", 26, -1)]
        #endregion
        public void TestGetAutoCloseIndex(string text, int startindex, int expectedreturn)
        {
            int index = HtmlTableParser.GetAutoCloseIndex(text, startindex);
            if (index != expectedreturn)
            {
                throw new Exception(string.Format("The found index {0} is differente from the excepted one {1} for {2}", index, expectedreturn, text));
            }
        }

        #region TestCase List
        //AUTO - CLOSE
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 0, "</a>", 27)]
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 11, "</a>", 27)]
        [TestCase("azertyuiop <a azertyuiop /> azertyuiop", 26, "</a>", - 1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 0, "</a>", - 1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 11,"</a>", - 1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azertyuiop", 20,"</a>", 28)]
        [TestCase("azertyuiop <a azertyuiop / < azertyuiop", 0, "</a>", - 1)]
        [TestCase("azertyuiop <a azertyuiop / < azertyuiop", 11, "</a>", - 1)]
        [TestCase("azertyuiop <a azertyuiop / < azertyuiop", 26, "</a>", - 1)]
        //AUTO - CLOSE vs CLOSE
        [TestCase("azertyuiop <a azertyuiop /> azer</a>tyuiop", 0, "</a>", 27)]
        [TestCase("azertyuiop <a azertyuiop /> azer</a>tyuiop", 11, "</a>", 27)]
        [TestCase("azertyuiop <a azertyuiop /> azer</a>tyuiop", 26, "</a>", 36)]
        [TestCase("azertyuiop <a azertyuiop /> azer</a>tyuiop", 35, "</a>", -1)]
        [TestCase("azertyuiop <a azer>tyuiop /> azer</a>tyuiop", 0, "</a>", 37)]
        [TestCase("azertyuiop <a azer>tyuiop /> azer</a>tyuiop", 11,"</a>", 37)]
        [TestCase("azertyuiop <a azer>tyuiop /> azer</a>tyuiop", 20, "</a>", 28)]
        [TestCase("azertyuiop <a azertyuiop / < azer</a>tyuiop", 0, "</a>",37)]
        [TestCase("azertyuiop <a azertyuiop / < azer</a>tyuiop", 11, "</a>", 37)]
        [TestCase("azertyuiop <a azertyuiop / < azer</a>tyuiop", 26, "</a>", 37)]
        [TestCase("azertyuiop <a azer</a>tyuiop /> azertyuiop", 0, "</a>", 22)]
        [TestCase("azertyuiop <a azer</a>tyuiop /> azertyuiop", 11, "</a>", 22)]
        [TestCase("azertyuiop <a azer</a>tyuiop /> azertyuiop", 20, "</a>", -1)]
        [TestCase("azertyuiop <a azer</a>tyuiop /> azertyuiop", 24, "</a>", 31)]
        [TestCase("azertyuiop <a azer</a>tyuiop /> azertyuiop", 35, "</a>", -1)]
        [TestCase("azertyuiop <a azer></a>tyuiop /> azertyuiop", 0, "</a>", 23)]
        [TestCase("azertyuiop <a azer></a>tyuiop /> azertyuiop", 11, "</a>", 23)]
        [TestCase("azertyuiop <a azer></a>tyuiop /> azertyuiop", 20, "</a>", -1)]
        [TestCase("azertyuiop <a azer></a>tyuiop /> azertyuiop", 24, "</a>", 32)]
        [TestCase("azertyuiop <a azer</a>tyuiop / < azertyuiop", 0, "</a>", 22)]
        [TestCase("azertyuiop <a azer</a>tyuiop / < azertyuiop", 11, "</a>", 22)]
        [TestCase("azertyuiop <a azer</a>tyuiop / < azertyuiop", 26, "</a>", -1)]
        #endregion
        public void TestGetPostClosingIndex(string text, int startindex, string wantedtag, int expectedreturn)
        {
            int index = HtmlTableParser.GetPostClosingIndex(text, startindex, wantedtag);
            if (index != expectedreturn)
            {
                throw new Exception(string.Format("The found index {0} is differente from the excepted one {1} for {2}", index, expectedreturn, text));
            }

            index = HtmlTableParser.GetPostClosingIndex(text.ToLower(), startindex, wantedtag.ToUpper());
            if (index != expectedreturn)
            {
                throw new Exception("Case sensitivity problem, result must the same");
            }
        }

        #region TestCase List
        [TestCase("<TH>H1</TH>", "H1", 1, 1, true)]
        [TestCase("<TD colspan=2>C21</TD>", "C21", 1, 2, false)]
        [TestCase("<TD rowspan=2>C52</TD>", "C52", 2, 1, false)]
        [TestCase("<TD rowspan=3 colspan=4>CA4</TD>", "CA4", 3, 4, false)]
        [TestCase("<TH />", "", 1, 1, true)]
        #endregion
        public void TestExtractCell(string text, string expectedText, int expectedRowSpan, int expectedColSpan, bool expectedIsHeader)
        {
            IHtmlCell cell = HtmlTableParser.ExtractCell(text);
            if (cell.InnerText != expectedText)
            {
                throw new Exception(string.Format("The found InnerText {0} is differente from the excepted one {1} for {2}", cell.InnerText, expectedText, text));
            }

            if (cell.IsHeader != expectedIsHeader)
            {
                throw new Exception(string.Format("The found IsHeader {0} is differente from the excepted one {1} for {2}", cell.IsHeader, expectedIsHeader, text));
            }

            if (cell.RowSpan != expectedRowSpan)
            {
                throw new Exception(string.Format("The found RowSpan {0} is differente from the excepted one {1} for {2}", cell.RowSpan, expectedRowSpan, text));
            }

            if (cell.ColSpan != expectedColSpan)
            {
                throw new Exception(string.Format("The found ColSpan {0} is differente from the excepted one {1} for {2}", cell.ColSpan, expectedColSpan, text));
            }
        }
        
        [Test]
        public void TestCellsInfos()
        {
            //Test when table is not the first caracters
            //Row 1 : test header
            //Row 2 : test cell
            //Row 3 : test colspan
            //Row 4-5: test rowspan and next row completion
            //Row 6-7: test rowspan and next row missing data
            //Row 8-9: test rowspan and colspan averlap
            //Row 10-13: test multi rowspan with overlap

            IHtmlTable ret = HtmlTableParser.Parse(@"azertyui 
<Table>
<TR><TH>H1</TH><TH>H2</TH></TR>
<TR><TD>C11</TD><TD>C12</TD></TR>

<TR><TD colspan=2>C21</TD><TD>C23</TD></TR>

<TR><TD>C31</TD><TD rowspan=2>C32</TD></TR>
<TR><TD>C41</TD><TD>C43</TD></TR>

<TR><TD>C51</TD><TD rowspan=2>C52</TD></TR>
<TR></TR>

<TR><TD>C71</TD><TD rowspan=2>C72</TD></TR>
<TR><TD colspan=2>C81</TD><TD>C83</TD></TR>

<TR><TD rowspan=3 colspan=3>C91</TD></TR>
<TR><TD rowspan=3 colspan=3>CA4</TD></TR>
</Table>");
            Assert.IsNotNull(ret, "ret must be not null");

            Assert.AreEqual(ret.RowCount, 13, "RowCount must be 13");

            string[,] expectedValue =
                {
                    { "H1", "H2", null, null, null, null, null },
                    { "C11", "C12", null, null, null, null, null },
                    { "C21", "C21", "C23", null, null, null, null },
                    { "C31", "C32", null, null, null, null, null },
                    { "C41", "C32", "C43", null, null, null, null },
                    { "C51", "C52", null, null, null, null, null },
                    { null, "C52", null, null, null, null, null },
                    { "C71", "C72", null, null, null, null, null },
                    { "C81", "C81", "C83", null, null, null, null },
                    { "C91", "C91", "C91", null, null, null, null },
                    { "C91", "C91", "C91", "CA4", "CA4", "CA4", null },
                    { "C91", "C91", "C91", "CA4", "CA4", "CA4", null },
                    { null, null, null, "CA4", "CA4", "CA4", null }
                };


            for (int i = 0; i < expectedValue.GetLength(0); i++)
            {
                for (int j = 0; j < expectedValue.GetLength(1); j++)
                {
                    IHtmlCell cell = ret[i, j];
                    string value = cell?.InnerText;
                    Assert.AreEqual(expectedValue[i, j], value, "Value different for ({0},{1}) : {2} vs {3}", i, j, value, expectedValue[i, j]);
                    Assert.AreEqual(cell?.ToString(), value);
                }
            }
        }
    }
}
