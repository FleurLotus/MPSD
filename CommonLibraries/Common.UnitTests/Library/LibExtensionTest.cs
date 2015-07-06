namespace Common.UnitTests.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Common.Library.Extension;

    using NUnit.Framework;

    [TestFixture]
    public class LibExtensionTest
    {
        #region TestCase List
        //Test
        [TestCase("", "azerty", "123456", StringComparison.InvariantCulture, "")]
        [TestCase(null, "azerty", "123456", StringComparison.InvariantCulture, null)]
        [TestCase("ABC", null, "123456", StringComparison.InvariantCulture, "ABC")]
        [TestCase("azertyazerty", "ZER", "123456", StringComparison.InvariantCulture, "azertyazerty")]
        [TestCase("azertyazerty", "ZER", "123456", StringComparison.InvariantCultureIgnoreCase, "a123456tya123456ty")]
        [TestCase("azertyazerty", "ZER", null, StringComparison.InvariantCultureIgnoreCase, "atyaty")]
        #endregion

        public void StringExtensionReplaceTest(string source, string old, string replace, StringComparison comparison, string expectedResult)
        {
            string value = source.Replace(old, replace, comparison);
            Assert.AreEqual(expectedResult, value, "Value is not the expected one: result '{0}', expectedResult '{1}'", expectedResult, value);
        }
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

        [Test]
        public void ReflectionExtensionGetPublicInstancePropertiesTest()
        {
            PropertyInfo[] publicInstanceProperties = typeof(ReflectionInfo).GetPublicInstanceProperties();
            PropertyInfo[] publicInstanceProperties2 = (new ReflectionInfo()).GetPublicInstanceProperties();

            Assert.IsNotNull(publicInstanceProperties);
            Assert.AreSame(publicInstanceProperties, publicInstanceProperties2);

            string[] names = publicInstanceProperties.Select(pi => pi.Name).ToArray();

            Assert.IsTrue(names.Length == 11, "Not the could number of property found");
            Assert.IsTrue(names.All(s => s.StartsWith("Member") && (s.Contains("PublicGet") || s.Contains("PublicSet"))), "Must find only member public");
        }
        
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable ValueParameterNotUsed
        private class ReflectionInfo
        {
            public static string StaticPublicGetSet { get; set; }
            private static string StaticPrivateGetSet { get; set; }
            internal static string StaticInternalGetSet { get; set; }
            protected static string StaticProtectedGetSet { get; set; }
            protected internal static string StaticProtectedInternalGetSet { get; set; }

            public static string StaticPublicGet
            {
                get { return null; }
            }
            private static string StaticPrivateGet
            {
                get { return null; }
            }
            internal static string StaticInternalGet
            {
                get { return null; }
            }
            protected static string StaticProtectedGet
            {
                get { return null; }
            }
            protected internal static string StaticProtectedInternalGet
            {
                get { return null; }
            }

            public static string StaticPublicSet
            {
                set { }
            }
            private static string StaticPrivateSet
            {
                set { }
            }
            internal static string StaticInternalSet
            {
                set { }
            }
            protected static string StaticProtectedSet
            {
                set { }
            }
            protected internal static string StaticProtectedInternalSet
            {
                set { }
            }


            public static string StaticPublicGetPrivateSet { get; private set; }

            public static string StaticPrivateGetPublicGet { private get; set; }
            public static string StaticPublicGetInternalSet { get; internal set; }
            public static string StaticInternalGetPublicSet { internal get; set; }
            public static string StaticPublicGetProtectedSet { get; protected set; }
            public static string StaticProtectedGetPublicSet { protected get; set; }
            public static string StaticPublicGetProtectedInternalSet { get; protected internal set; }
            public static string StaticProtectedInternalGetPublicSet { protected internal get; set; }

            internal static string StaticInternalGetPrivateSet { get; private set; }
            internal static string StaticPrivateGetInternalGet { private get; set; }

            protected internal static string StaticProtectedInternalGetPrivateSet { get; private set; }
            protected internal static string StaticPrivateSetProtectedInternalSet { private get; set; }
            protected internal static string StaticProtectedInternalSetInternalSet { get; internal set; }
            protected internal static string StaticInternalSetProtectedInternalSet { internal get; set; }
            protected internal static string StaticProtectedInternalGetProtectedSet { get; protected set; }
            protected internal static string StaticProtectedSetProtectedInternalSet { protected get; set; }

            protected static string StaticProtectedGetPrivateSet { get; private set; }
            protected static string StaticPrivateGetProtectedGet { private get; set; }


            public string MemberPublicGetSet { get; set; }
            private string MemberPrivateGetSet { get; set; }
            internal string MemberInternalGetSet { get; set; }
            protected string MemberProtectedGetSet { get; set; }
            protected internal string MemberProtectedInternalGetSet { get; set; }

            public string MemberPublicGet
            {
                get { return null; }
            }
            private string MemberPrivateGet
            {
                get { return null; }
            }
            internal string MemberInternalGet
            {
                get { return null; }
            }
            protected string MemberProtectedGet
            {
                get { return null; }
            }
            protected internal string MemberProtectedInternalGet
            {
                get { return null; }
            }

            public string MemberPublicSet
            {
                set { }
            }
            private string MemberPrivateSet
            {
                set { }
            }
            internal string MemberInternalSet
            {
                set { }
            }
            protected string MemberProtectedSet
            {
                set { }
            }
            protected internal string MemberProtectedInternalSet
            {
                set { }
            }

            public string MemberPublicGetPrivateSet { get; private set; }
            public string MemberPrivateGetPublicGet { private get; set; }
            public string MemberPublicGetInternalSet { get; internal set; }
            public string MemberInternalGetPublicSet { internal get; set; }
            public string MemberPublicGetProtectedSet { get; protected set; }
            public string MemberProtectedGetPublicSet { protected get; set; }
            public string MemberPublicGetProtectedInternalSet { get; protected internal set; }
            public string MemberProtectedInternalGetPublicSet { protected internal get; set; }

            internal string MemberInternalGetPrivateSet { get; private set; }
            internal string MemberPrivateGetInternalGet { private get; set; }

            protected internal string MemberProtectedInternalGetPrivateSet { get; private set; }
            protected internal string MemberPrivateSetProtectedInternalSet { private get; set; }
            protected internal string MemberProtectedInternalSetInternalSet { get; internal set; }
            protected internal string MemberInternalSetProtectedInternalSet { internal get; set; }
            protected internal string MemberProtectedInternalGetProtectedSet { get; protected set; }
            protected internal string MemberProtectedSetProtectedInternalSet { protected get; set; }

            protected string MemberProtectedGetPrivateSet { get; private set; }
            protected string MemberPrivateGetProtectedGet { private get; set; }
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore ValueParameterNotUsed
    }
}
