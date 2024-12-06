namespace Common.SQL.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class CaseSensitivityTest
    {
        [TestCaseSource(nameof(TestToKeyStringSource), new object[] { nameof(TestToKeyString) })]
        public void TestToKeyString(string str, CaseSensitivity caseSensitivity, IConstraint constraint)
        {
            Assert.That(CaseSensitivity.ToKeyString(str, caseSensitivity), constraint);
        }

        public static IEnumerable<TestCaseData> TestToKeyStringSource(string methodCaller)
        {
            CaseSensitivity caseSensitivity = new CaseSensitivity(false);

            yield return new TestCaseData(null, caseSensitivity, Is.Null).SetName($"{methodCaller} (Null - CaseSentitivity = False)");
            yield return new TestCaseData(string.Empty, caseSensitivity, Is.Empty).SetName($"{methodCaller} (Empty - CaseSentitivity = False)");
            yield return new TestCaseData("AzErTy", caseSensitivity, Is.EqualTo("azerty")).SetName($"{methodCaller} (Value - CaseSentitivity = False)");

            caseSensitivity = new CaseSensitivity(true);

            yield return new TestCaseData(null, caseSensitivity, Is.Null).SetName($"{methodCaller} (Null- CaseSentitivity = True)");
            yield return new TestCaseData(string.Empty, caseSensitivity, Is.Empty).SetName($"{methodCaller} (Empty - CaseSentitivity = True)");
            yield return new TestCaseData("AzErTy", caseSensitivity, Is.EqualTo("AzErTy")).SetName($"{methodCaller} (Value - CaseSentitivity = True)");

            caseSensitivity = null;

            yield return new TestCaseData(null, caseSensitivity, Is.Null).SetName($"{methodCaller} (Null - CaseSentitivity = null)");
            yield return new TestCaseData(string.Empty, caseSensitivity, Is.Empty).SetName($"{methodCaller} (Empty - CaseSentitivity = null)");
            yield return new TestCaseData("AzErTy", caseSensitivity, Is.EqualTo("AzErTy")).SetName($"{methodCaller} (Value - CaseSentitivity = null)");
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(string strA, string strB, CaseSensitivity caseSensitivity, IConstraint constraint)
        {
            Assert.That(CaseSensitivity.Compare(strA, strB, caseSensitivity), constraint);
        }

        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            CaseSensitivity caseSensitivity = new CaseSensitivity(false);

            yield return new TestCaseData(null, null, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Null vs Null - CaseSentitivity = False)");
            yield return new TestCaseData(null, string.Empty, caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Null vs Empty - CaseSentitivity = False)");
            yield return new TestCaseData(string.Empty, null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Empty vs Null - CaseSentitivity = False)");
            yield return new TestCaseData(string.Empty, string.Empty, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Empty vs Empty - CaseSentitivity = False)");
            yield return new TestCaseData("AzErTy", null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Null - CaseSentitivity = False)");
            yield return new TestCaseData("AzErTy", string.Empty, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Empty - CaseSentitivity = False)");
            yield return new TestCaseData("AzErTy", "azerty", caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Value vs value - CaseSentitivity = False)");
            yield return new TestCaseData("AzErTy", "BcdEF", caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Different values - CaseSentitivity = False)");

            caseSensitivity = new CaseSensitivity(true);

            yield return new TestCaseData(null, null, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Null vs Null - CaseSentitivity = True)");
            yield return new TestCaseData(null, string.Empty, caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Null vs Empty - CaseSentitivity = True)");
            yield return new TestCaseData(string.Empty, null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Empty vs Null - CaseSentitivity = True)");
            yield return new TestCaseData(string.Empty, string.Empty, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Empty vs Empty - CaseSentitivity = True)");
            yield return new TestCaseData("AzErTy", null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Null - CaseSentitivity = True)");
            yield return new TestCaseData("AzErTy", string.Empty, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Empty - CaseSentitivity = True)");
            yield return new TestCaseData("AzErTy", "azerty", caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Value vs value - CaseSentitivity = True)");
            yield return new TestCaseData("AzErTy", "BcdEF", caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Different values - CaseSentitivity = True)");

            caseSensitivity = null;

            yield return new TestCaseData(null, null, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Null vs Null - CaseSentitivity = null)");
            yield return new TestCaseData(null, string.Empty, caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Null vs Empty - CaseSentitivity = null)");
            yield return new TestCaseData(string.Empty, null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Empty vs Null - CaseSentitivity = null)");
            yield return new TestCaseData(string.Empty, string.Empty, caseSensitivity, Is.EqualTo(0)).SetName($"{methodCaller} (Empty vs Empty - CaseSentitivity = null)");
            yield return new TestCaseData("AzErTy", null, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Null - CaseSentitivity = null)");
            yield return new TestCaseData("AzErTy", string.Empty, caseSensitivity, Is.GreaterThan(0)).SetName($"{methodCaller} (Value vs Empty - CaseSentitivity = null)");
            yield return new TestCaseData("AzErTy", "azerty", caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Value vs value - CaseSentitivity = null)");
            yield return new TestCaseData("AzErTy", "BcdEF", caseSensitivity, Is.LessThan(0)).SetName($"{methodCaller} (Different values - CaseSentitivity = null)");
        }
    }
}