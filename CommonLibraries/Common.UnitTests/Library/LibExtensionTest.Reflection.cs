namespace Common.UnitTests.Library
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Common.Library.Extension;

    using NUnit.Framework;

    public partial class LibExtensionTest
    {
        [Test]
        public void ReflectionExtensionGetPublicInstancePropertiesTest()
        {
            PropertyInfo[] publicInstanceProperties = typeof(ReflectionInfo).GetPublicInstanceProperties();
            PropertyInfo[] publicInstanceProperties2 = (new ReflectionInfo()).GetPublicInstanceProperties();

            Assert.That(publicInstanceProperties, Is.Not.Null);
            Assert.That(publicInstanceProperties, Is.EqualTo(publicInstanceProperties2));

            string[] names = publicInstanceProperties.Select(pi => pi.Name).ToArray();

            Assert.That(names.Length, Is.EqualTo(11), "Not the could number of property found");
            Assert.That(names.All(s => s.StartsWith("Member") && (s.Contains("PublicGet") || s.Contains("PublicSet"))), Is.True, "Must find only member public");
        }
    
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable ValueParameterNotUsed
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        private class MotherAttribute : Attribute
        {


        }
        private class ChildAttribute : MotherAttribute
        {
        }

        private class MotherClass
        {
            [Mother]
            public object Property { get; set; }

            [Child]
            public object Property2 { get; set; }

        }


        private class ChildClass : MotherClass
        {
            [Mother]
            public object Property3 { get; set; }

            [Child]
            public object Property4 { get; set; }
        }

        private class ReflectionInfo
        {
            public static string StaticPublicGetSet { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
            private static string StaticPrivateGetSet { get; set; }
            internal static string StaticInternalGetSet { get; set; }
            protected static string StaticProtectedGetSet { get; set; }
            protected internal static string StaticProtectedInternalGetSet { get; set; }

            public static string StaticPublicGet
            {
                get { return null; }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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


            public static string StaticPublicGetPrivateSet { get; }

            public static string StaticPrivateGetPublicGet { private get; set; }
            public static string StaticPublicGetInternalSet { get; internal set; }
            public static string StaticInternalGetPublicSet { internal get; set; }
            public static string StaticPublicGetProtectedSet { get; protected set; }
            public static string StaticProtectedGetPublicSet { protected get; set; }
            public static string StaticPublicGetProtectedInternalSet { get; protected internal set; }
            public static string StaticProtectedInternalGetPublicSet { protected internal get; set; }

            internal static string StaticInternalGetPrivateSet { get; }
            internal static string StaticPrivateGetInternalGet { private get; set; }

            protected internal static string StaticProtectedInternalGetPrivateSet { get; }
            protected internal static string StaticPrivateSetProtectedInternalSet { private get; set; }
            protected internal static string StaticProtectedInternalSetInternalSet { get; internal set; }
            protected internal static string StaticInternalSetProtectedInternalSet { internal get; set; }
            protected internal static string StaticProtectedInternalGetProtectedSet { get; protected set; }
            protected internal static string StaticProtectedSetProtectedInternalSet { protected get; set; }

            protected static string StaticProtectedGetPrivateSet { get; }
            protected static string StaticPrivateGetProtectedGet { private get; set; }


            public string MemberPublicGetSet { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
            private string MemberPrivateGetSet { get; set; }
            internal string MemberInternalGetSet { get; set; }
            protected string MemberProtectedGetSet { get; set; }
            protected internal string MemberProtectedInternalGetSet { get; set; }

            public string MemberPublicGet
            {
                get { return null; }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

            public string MemberPublicGetPrivateSet { get; }
            public string MemberPrivateGetPublicGet { private get; set; }
            public string MemberPublicGetInternalSet { get; internal set; }
            public string MemberInternalGetPublicSet { internal get; set; }
            public string MemberPublicGetProtectedSet { get; protected set; }
            public string MemberProtectedGetPublicSet { protected get; set; }
            public string MemberPublicGetProtectedInternalSet { get; protected internal set; }
            public string MemberProtectedInternalGetPublicSet { protected internal get; set; }

            internal string MemberInternalGetPrivateSet { get; }
            internal string MemberPrivateGetInternalGet { private get; set; }

            protected internal string MemberProtectedInternalGetPrivateSet { get; }
            protected internal string MemberPrivateSetProtectedInternalSet { private get; set; }
            protected internal string MemberProtectedInternalSetInternalSet { get; internal set; }
            protected internal string MemberInternalSetProtectedInternalSet { internal get; set; }
            protected internal string MemberProtectedInternalGetProtectedSet { get; protected set; }
            protected internal string MemberProtectedSetProtectedInternalSet { protected get; set; }

            protected string MemberProtectedGetPrivateSet { get; }
            protected string MemberPrivateGetProtectedGet { private get; set; }
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore ValueParameterNotUsed
    }
}
