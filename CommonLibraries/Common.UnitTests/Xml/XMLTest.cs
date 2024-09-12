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
                Assert.That(reader.Read(), "Can't read");
                Assert.That(reader.Name == "Element", "Not the good tag");
                IDictionary<string, string> attributes = reader.GetAttributes();

                Assert.That(attributes, Is.Not.Null, "attributes is null");
                Assert.That(attributes.Count == 3, "Not the good number of attributes");
                Assert.That(attributes.ContainsKey("a"), "Has not key a");
                Assert.That(attributes["a"] == "123", "Value is wrong for key a");
                Assert.That(attributes.ContainsKey("b"), "Has not key b");
                Assert.That(attributes["b"] == "458", "Value is wrong for key b");
                Assert.That(attributes.ContainsKey("c"), "Has not key c");
                Assert.That(attributes["c"] == "azerty", "Value is wrong for key c");
                Assert.That(reader.Name == "Element", "Not back to the element");
            }
        }
        [Test]
        public void TestNoAttributes()
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(@"<Element />")))
            {
                Assert.That(reader.Read(), "Can't read");
                Assert.That(reader.Name == "Element", "Not the good tag");
                IDictionary<string, string> attributes = reader.GetAttributes();

                Assert.That(attributes, Is.Not.Null, "attributes is null");
                Assert.That(attributes.Count == 0, "Not the good number of attributes");
                Assert.That(reader.Name == "Element", "Not back to the element");
            }
        }
    }
}
