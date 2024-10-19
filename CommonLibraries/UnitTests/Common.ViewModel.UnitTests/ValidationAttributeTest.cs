namespace Common.ViewModel.UnitTests
{
    using System;
    using System.Text.RegularExpressions;
    using Common.ViewModel.Validation.Attributes;

    using NUnit.Framework;

    [TestFixture]
    public class ValidationAttributeTest
    {
        [Test]
        public void TestValidationAttributeArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeAttribute(null), "Null message must thrown ArgumentNullException");
            Assert.Throws<ArgumentNullException>(() => new FakeAttribute(string.Empty), "Empty message must thrown ArgumentNullException");
            Assert.Throws<ArgumentNullException>(() => new FakeAttribute("        "), "Blank message must thrown ArgumentNullException");
        }
        [Test]
        public void TestStringMaxLenValidationAttributeArgument()
        {
            Assert.Throws<ArgumentException>(() => new StringMaxLenValidationAttribute(-5), "negative value must not be valid argument");
            Assert.Throws<ArgumentException>(() => new StringMaxLenValidationAttribute(0), "0 must not be valid argument");
            Assert.That(new StringMaxLenValidationAttribute(3), Is.Not.Null, "Strictly positive value is valid argument");
        }
        [Test]
        public void TestStringMinLenValidationAttributeArgument()
        {
            Assert.Throws<ArgumentException>(() => new StringMinLenValidationAttribute(-5), "negative value must not be valid argument");
            Assert.That(new StringMinLenValidationAttribute(0), Is.Not.Null, "0 is valid argument");
            Assert.That(new StringMinLenValidationAttribute(3), Is.Not.Null, "Positive value is valid argument");
        }
        [Test]
        public void TestStringRegExValidationAttributeArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new StringRegExValidationAttribute(null), "Null reg string must not be valid argument");
            Assert.Throws<ArgumentNullException>(() => new StringRegExValidationAttribute(string.Empty), "Empty reg string must not be valid argument");
            Assert.Throws<ArgumentNullException>(() => new StringRegExValidationAttribute("   "), "Blank reg string must not be valid argument");
            Assert.Throws<RegexParseException>(() => new StringRegExValidationAttribute("azertyuiop^["), "Invalid reg string must not be valid argument");
            Assert.That(new StringRegExValidationAttribute(".*"), Is.Not.Null, "0 is valid argument");
        }

        #region TestCase List
        [TestCase(1, null, true)]
        [TestCase(1, "", true)]
        [TestCase(1, "azerty", false)]
        [TestCase(1, "    ", false)]
        [TestCase(4, null, true)]
        [TestCase(4, "", true)]
        [TestCase(4, "azerty", false)]
        [TestCase(4, "    ", true)]
        [TestCase(5, null, true)]
        [TestCase(5, "", true)]
        [TestCase(5, "azerty", false)]
        [TestCase(5, "    ", true)]
        #endregion
        public void TestMaxLenValidationAttribute(int maxLen, string input, bool isValid)
        {
            StringMaxLenValidationAttribute att = new StringMaxLenValidationAttribute(maxLen);
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for maxLen = {maxLen} and input = {input}");
        }

        #region TestCase List
        [TestCase(0, null, false)]
        [TestCase(0, "", true)]
        [TestCase(0, "azerty", true)]
        [TestCase(0, "    ", true)]
        [TestCase(1, null, false)]
        [TestCase(1, "", false)]
        [TestCase(1, "azerty", true)]
        [TestCase(1, "    ", true)]
        [TestCase(4, null, false)]
        [TestCase(4, "", false)]
        [TestCase(4, "azerty", true)]
        [TestCase(4, "    ", true)]
        [TestCase(5, null, false)]
        [TestCase(5, "", false)]
        [TestCase(5, "azerty", true)]
        [TestCase(5, "    ", false)]
        #endregion
        public void TestMinLenValidationAttribute(int minLen, string input, bool isValid)
        {
            StringMinLenValidationAttribute att = new StringMinLenValidationAttribute(minLen);
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for minLen = {minLen} and input = {input}");
        }

        #region TestCase List
        [TestCase(null, false)]
        [TestCase("", true)]
        [TestCase("azerty", true)]
        [TestCase("    ", true)]
        [TestCase(0, true)]
        [TestCase(5.25, true)]
        #endregion
        public void TestNotNullValidationAttribute(object input, bool isValid)
        {
            NotNullValidationAttribute att = new NotNullValidationAttribute();
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for input = {input}");
        }

        #region TestCase List
        [TestCase("za*b?c+", null, false)]
        [TestCase("za*b?c+", "", false)]
        [TestCase("za*b?c+", "zaaac", true)]
        [TestCase("za*b?c+", "zAc", false)]
        #endregion
        public void TestStringRegExValidationAttribute(string regexp, string input, bool isValid)
        {
            StringRegExValidationAttribute att = new StringRegExValidationAttribute(regexp);
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for regexp = {regexp} and input = {input}");
        }

        #region TestCase List
        [TestCase(0, false, null, false)]
        [TestCase(0, true, null, false)]
        [TestCase(0, false, "12", false)]
        [TestCase(0, true, "12", false)]
        [TestCase(5.25, false, 5, false)]
        [TestCase(5.25, true, 5, false)]
        [TestCase(5.25, false, 10, true)]
        [TestCase(5.25, true, 10, true)]
        [TestCase(5.0, false, 5, false)]
        [TestCase(5.0, true, 5, true)]
        #endregion
        public void TestGreaterThanValidationAttribute(double minValue, bool allowEquals, object input, bool isValid)
        {
            GreaterThanValidationAttribute att = new GreaterThanValidationAttribute(minValue, allowEquals);
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for minvalue = {minValue} and allowEquals = {allowEquals} and input = {input}");
        }
        #region TestCase List
        [TestCase(0, false, null, false)]
        [TestCase(0, true, null, false)]
        [TestCase(0, false, "12", false)]
        [TestCase(0, true, "12", false)]
        [TestCase(5.25, false, 5, true)]
        [TestCase(5.25, true, 5, true)]
        [TestCase(5.25, false, 10, false)]
        [TestCase(5.25, true, 10, false)]
        [TestCase(5.0, false, 5, false)]
        [TestCase(5.0, true, 5, true)]
        #endregion
        public void TestLessThanValidationAttribute(double maxValue, bool allowEquals, object input, bool isValid)
        {
            LessThanValidationAttribute att = new LessThanValidationAttribute(maxValue, allowEquals);
            Assert.That(string.IsNullOrEmpty(att.Validate(input)), Is.EqualTo(isValid), $"Expected {isValid} for maxvalue = {maxValue} and allowEquals = {allowEquals} and input = {input}");
        }


        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedMember.Local
        private class FakeAttribute : ValidationAttribute
        {
            public FakeAttribute(string errorMessage)
                : base(errorMessage)
            {
            }
            protected override bool IsValide(object instance)
            {
                throw new NotImplementedException();
            }
        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local
    }
}