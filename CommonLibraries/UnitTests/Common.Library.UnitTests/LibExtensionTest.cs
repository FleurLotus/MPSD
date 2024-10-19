namespace Common.Library.UnitTests
{
    using System.Collections.Generic;

    using Common.Library.Extension;

    using NUnit.Framework;

    [TestFixture]
    public partial class LibExtensionTest
    {
        [Test]
        public void DictionaryExtensionGetOrDefaultTest()
        {
            IDictionary<string, string> dic = new Dictionary<string, string> { { "AAA", "aaa" }, { "BBB", "bbb" } };

            string value = dic.GetOrDefault("AAA");
            Assert.That(value, Is.EqualTo("aaa"), "Value is not the expected one for AAA");
            value = dic.GetOrDefault("BBB");
            Assert.That(value, Is.EqualTo("bbb"), "Value is not the expected one for BBB");
            value = dic.GetOrDefault("bbb");
            Assert.That(value, Is.Null, "Value is not the expected one for bbb");
        }
        [Test]
        public void DictionaryExtensionAddRangeTest()
        {
            IDictionary<string, string> dic = new Dictionary<string, string> { { "AAA", "aaa" }, { "BBB", "bbb" } };
            IDictionary<string, string> dic2 = new Dictionary<string, string> { { "CCC", "ccc" }, { "DDD", "ddd" } };
            dic.AddRange(dic2);

            Assert.That(dic.Count , Is.EqualTo(4), "Not the good count");
            foreach (KeyValuePair<string, string> kv in dic2)
            {
                Assert.That(dic.ContainsKey(kv.Key), Is.True, "Missing key {kv.Key}");
                Assert.That(dic[kv.Key] , Is.EqualTo(kv.Value), $"Not the good value for key {kv.Key} expecting {kv.Value}");
            }
        }
    }
}
