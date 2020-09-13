namespace Common.UnitTests.Library
{
    using System;
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
            Assert.AreEqual("aaa", value, "Value is not the expected one for AAA" );
            value = dic.GetOrDefault("BBB");
            Assert.AreEqual("bbb", value, "Value is not the expected one for BBB");
            value = dic.GetOrDefault("bbb");
            Assert.AreEqual(null, value, "Value is not the expected one for bbb");
        }
        [Test]
        public void DictionaryExtensionAddRangeTest()
        {
            IDictionary<string, string> dic = new Dictionary<string, string> { { "AAA", "aaa" }, { "BBB", "bbb" } };
            IDictionary<string, string> dic2 = new Dictionary<string, string> { { "CCC", "ccc" }, { "DDD", "ddd" }} ;
            dic.AddRange(dic2);

            Assert.IsTrue(dic.Count == 4 , "Not the good count");
            foreach (KeyValuePair<string, string> kv in dic2)
            {
                Assert.IsTrue(dic.ContainsKey(kv.Key), "Missing key {0}", kv.Key);
                Assert.IsTrue(dic[kv.Key] == kv.Value, "Not the good value for key {0} expecting {1}", kv.Key, kv.Value);
            }
        }
    }
}
