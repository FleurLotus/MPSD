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

            Assert.IsNotNull(publicInstanceProperties);
            Assert.AreSame(publicInstanceProperties, publicInstanceProperties2);

            string[] names = publicInstanceProperties.Select(pi => pi.Name).ToArray();

            Assert.IsTrue(names.Length == 11, "Not the could number of property found");
            Assert.IsTrue(names.All(s => s.StartsWith("Member") && (s.Contains("PublicGet") || s.Contains("PublicSet"))), "Must find only member public");
        }
        [Test]
        public void ReflectionExtensionGetCustomAttributes()
        {
            PropertyInfo[] mother = typeof(MotherClass).GetPublicInstanceProperties();
            PropertyInfo[] child = typeof(ChildClass).GetPublicInstanceProperties();


            Assert.IsTrue(mother.Length == 2, "Not the expected number of property");
            Assert.IsTrue(child.Length == 4, "Not the expected number of property");

            foreach (var propArray in new[] { mother, child })
            {
                foreach (var propertyInfo in propArray)
                {
                    var resm = propertyInfo.GetCustomAttributes<MotherAttribute>();
                    var resm2 = propertyInfo.GetCustomAttributes<MotherAttribute>(true);
                    var resc = propertyInfo.GetCustomAttributes<ChildAttribute>();
                    var resc2 = propertyInfo.GetCustomAttributes<ChildAttribute>(true);

                    string name = propertyInfo.Name;
                    //Child and Mother are a Mother
                    Assert.IsTrue(resm.Length == 1, "Not the expected number of {0} on GetCustomAttributes<MotherAttribute>() call", name);
                    Assert.IsTrue(resm2.Length == 1, "Not the expected number of {0} on GetCustomAttributes<MotherAttribute>(true) call", name);
                    
                    switch (name)
                    {
                        case "Property2":
                        case "Property4":
                            //Child is a child
                            Assert.IsTrue(resc.Length == 1, "Not the expected number of {0} on GetCustomAttributes<ChildAttribute>() call", name);
                            Assert.IsTrue(resc2.Length == 1, "Not the expected number of {0} on GetCustomAttributes<ChildAttribute>(true) call", name);
                            break;

                        case "Property":
                        case "Property3":
                            //Mother is not a child
                            Assert.IsTrue(resc.Length == 0, "Not the expected number of {0} on GetCustomAttributes<ChildAttribute>() call", name);
                            Assert.IsTrue(resc2.Length == 0, "Not the expected number of {0} on GetCustomAttributes<ChildAttribute>(true) call", name);
                            break;

                        default:
                            throw new Exception("Unknow property");
                    }
                }
            }
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
