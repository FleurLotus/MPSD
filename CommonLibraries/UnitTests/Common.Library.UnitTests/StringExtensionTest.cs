namespace Common.Library.UnitTests
{
    using NUnit.Framework;

    using Common.Library.Extension;

    [TestFixture]
    public class StringExtensionTest
    {
        [Test]
        public void StringExtensionHtmlTrimNullParameter()
        {
            string str = null;
            Assert.That(str.HtmlTrim(), Is.Null);
        }
        [Test]
        public void StringExtensionHtmlTrim()
        {
            string str = " \r\n\t&nbsp;azerty \r\n\t&nbsp;azerty \r\n\t&nbsp;";
            Assert.That(str.HtmlTrim(), Is.EqualTo("azerty \r\n\t azerty"));
        }
        [Test]
        public void StringExtensionHtmlRemoveFormatTagNullParameter()
        {
            string str = null;
            Assert.That(str.HtmlRemoveFormatTag(), Is.Null);
        }
        [Test]
        public void StringExtensionHtmlRemoveFormatTag()
        {
            string str = "<p><i><b><div><strong><em><small><tr><mark><del><ins><sub><sup><h1><h2><h3><h4><h5><h6>aaaa</H6></H5></H4></H3></H2></H1></SUP></SUB></INS></DEL></MARK></TR></SMALL></EM></STRONG></DIV></B></I></P>";
            Assert.That(str.HtmlRemoveFormatTag(), Is.EqualTo("<div><tr>aaaa</TR></DIV>"));
        }
    }
}
