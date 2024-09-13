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
                Assert.That(reader.Read(), Is.True, "Can't read");
                Assert.That(reader.Name, Is.EqualTo("Element"), "Not the good tag");
                IDictionary<string, string> attributes = reader.GetAttributes();

                Assert.That(attributes, Is.Not.Null, "attributes is null");
                Assert.That(attributes.Count, Is.EqualTo(3), "Not the good number of attributes");
                Assert.That(attributes.ContainsKey("a"), Is.True, "Has not key a");
                Assert.That(attributes["a"], Is.EqualTo("123"), "Value is wrong for key a");
                Assert.That(attributes.ContainsKey("b"), Is.True, "Has not key b");
                Assert.That(attributes["b"], Is.EqualTo("458"), "Value is wrong for key b");
                Assert.That(attributes.ContainsKey("c"), Is.True, "Has not key c");
                Assert.That(attributes["c"], Is.EqualTo("azerty"), "Value is wrong for key c");
                Assert.That(reader.Name, Is.EqualTo("Element"), "Not back to the element");
            }
        }
        [Test]
        public void TestNoAttributes()
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(@"<Element />")))
            {
                Assert.That(reader.Read(), Is.True, "Can't read");
                Assert.That(reader.Name, Is.EqualTo("Element"), "Not the good tag");
                IDictionary<string, string> attributes = reader.GetAttributes();

                Assert.That(attributes, Is.Not.Null, "attributes is null");
                Assert.That(attributes.Count, Is.EqualTo(0), "Not the good number of attributes");
                Assert.That(reader.Name, Is.EqualTo("Element"), "Not back to the element");
            }
        }
    }
}
