
namespace Common.UnitTests
{
    using System;

    using Common.Libray.Enums;

    using NUnit.Framework;

    [TestFixture]
    public class MatcherTest
    {
        [Flags]
        public enum EnumWithFlag
        {
            Value0 = 0,
            Value1 = 1,
            Value2 = 1<<1,
            Value3 = 1<<2,
            Value4 = 1<<3,
            Value5 = 1<<4,
            Value6 = 1<<5,
        }
        public enum EnumWithNoFlag
        {
            Value0 = 0,
            Value1 = 1,
            Value2 = 2,
            Value3 = 3,
            Value4 = 4,
            Value5 = 5,
            Value6 = 6,
        }

        #region TestCase List
        //Test
        [TestCase(EnumWithFlag.Value1, EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2, EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value2, EnumWithFlag.Value2, true)]

        [TestCase(EnumWithFlag.Value1 ,EnumWithFlag.Value1 | EnumWithFlag.Value2,  true)]
        [TestCase(EnumWithFlag.Value2 ,EnumWithFlag.Value1 | EnumWithFlag.Value2,  true)]
        [TestCase(EnumWithFlag.Value3 ,EnumWithFlag.Value1 | EnumWithFlag.Value2,  false)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value2, EnumWithFlag.Value1 | EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value3, EnumWithFlag.Value1 | EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value2 | EnumWithFlag.Value3, EnumWithFlag.Value1 | EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value4 | EnumWithFlag.Value4, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]


        //Work with value out of enum
        [TestCase(132, 2, false)]
        [TestCase(132, 4, true)]
        //0 always false
        [TestCase(0, EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2, 0, false)]
        [TestCase(0, 0, false)]
        #endregion
        public void TestHasValueWithFlag(EnumWithFlag value, EnumWithFlag check, bool exceptedValue)
        {
            bool hasValue = Matcher<EnumWithFlag>.HasValue(value, check);
            Assert.AreEqual(exceptedValue, hasValue, "Call HasValue on {0} and {1} return {2} while excepted {3}", value, check, hasValue, exceptedValue);
        }
        #region TestCase List
        [TestCase(EnumWithNoFlag.Value1, EnumWithNoFlag.Value2, false)]
        [TestCase(EnumWithNoFlag.Value2, EnumWithNoFlag.Value2, true)]

        //Work with value out of enum
        [TestCase(132, 2, false)]
        [TestCase(132, 4, false)]

        [TestCase(0, EnumWithNoFlag.Value2, false)]
        [TestCase(EnumWithNoFlag.Value2, 0, false)]
        [TestCase(0, 0, true)]
        #endregion
        public void TestHasValueWithNoFlag(EnumWithNoFlag value, EnumWithNoFlag check, bool exceptedValue)
        {
            bool hasValue = Matcher<EnumWithNoFlag>.HasValue(value, check);
            Assert.AreEqual(exceptedValue, hasValue, "Call HasValue on {0} and {1} return {2} while excepted {3}", value, check, hasValue, exceptedValue);
        }

        #region TestCase List
        //Test
        [TestCase(EnumWithFlag.Value1, EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2, EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value2, EnumWithFlag.Value2, true)]

        [TestCase(EnumWithFlag.Value1, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value3, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value2, EnumWithFlag.Value1 | EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value3, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2 | EnumWithFlag.Value3, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value4 | EnumWithFlag.Value4, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value2 | EnumWithFlag.Value3 , EnumWithFlag.Value1 | EnumWithFlag.Value2, true)]
        [TestCase(EnumWithFlag.Value1 | EnumWithFlag.Value3 | EnumWithFlag.Value4, EnumWithFlag.Value1 | EnumWithFlag.Value2, false)]

        //Work with value out of enum
        [TestCase(132, 2, false)]
        [TestCase(132, 4, true)]
        //0 always false
        [TestCase(0, EnumWithFlag.Value2, false)]
        [TestCase(EnumWithFlag.Value2, 0, false)]
        [TestCase(0, 0, false)]
        #endregion
        public void TestIncludeValueWithFlag(EnumWithFlag value, EnumWithFlag check, bool exceptedValue)
        {
            bool includeValue = Matcher<EnumWithFlag>.IncludeValue(value, check);
            Assert.AreEqual(exceptedValue, includeValue, "Call IncludeValue on {0} and {1} return {2} while excepted {3}", value, check, includeValue, exceptedValue);
        }
        #region TestCase List
        [TestCase(EnumWithNoFlag.Value1, EnumWithNoFlag.Value2, false)]
        [TestCase(EnumWithNoFlag.Value2, EnumWithNoFlag.Value2, true)]

        //Work with value out of enum
        [TestCase(132, 2, false)]
        [TestCase(132, 4, false)]

        [TestCase(0, EnumWithNoFlag.Value2, false)]
        [TestCase(EnumWithNoFlag.Value2, 0, false)]
        [TestCase(0, 0, true)]
        #endregion
        public void TestIncludeValueWithNoFlag(EnumWithNoFlag value, EnumWithNoFlag check, bool exceptedValue)
        {
            bool includeValue = Matcher<EnumWithNoFlag>.IncludeValue(value, check);
            Assert.AreEqual(exceptedValue, includeValue, "Call IncludeValue on {0} and {1} return {2} while excepted {3}", value, check, includeValue, exceptedValue);
        }
    }
}
