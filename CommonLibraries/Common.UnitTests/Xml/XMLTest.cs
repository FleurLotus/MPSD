namespace Common.UnitTests.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Common.XML;

    using NUnit.Framework;

    [TestFixture]
    public class XMLTest
    {
        [Test]
        public void TestNullThrowsException()
        {
            XmlTextReader reader = null;
            // ReSharper disable ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => reader.GetAttributes());
            // ReSharper restore ExpressionIsAlwaysNull
        }

        [Test]
        public void TestAttributes()
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(@"<Element a=""123"" b=""458"" c=""azerty"" />")))
            {
                Assert.IsTrue(reader.Read(), "Can't read");
                Assert.IsTrue(reader.Name == "Element", "Not the good tag");
                IDictionary<string, string> attributes = reader.GetAttributes();

                Assert.IsNotNull(attributes, "attributes is null");
                Assert.IsTrue(attributes.Count == 3, "Not the good number of attributes");
                Assert.IsTrue(attributes.ContainsKey("a"), "Has not key a");
                Assert.IsTrue(attributes["a"]=="123","Value is wrong for key a");
                Assert.IsTrue(attributes.ContainsKey("b"), "Has not key b");
                Assert.IsTrue(attributes["b"] == "458", "Value is wrong for key b");
                Assert.IsTrue(attributes.ContainsKey("c"), "Has not key c");
                Assert.IsTrue(attributes["c"] == "azerty", "Value is wrong for key c");
                Assert.IsTrue(reader.Name == "Element", "Not back to the element");
            }
        }
    }
}
