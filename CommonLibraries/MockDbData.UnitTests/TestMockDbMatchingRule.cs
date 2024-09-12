namespace MockDbData.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using NUnit.Framework;


    [TestFixture]
    public class TestMockDbMatchingRule
    {
        [Test]
        public void TestSetParameterValueArgument()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.Throws<ArgumentException>(() => mockDbMatchingRule.SetParameterValue(-5, "aaaa"), "Should throw ArgumentException when parameterPosition<0 for string param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue(0, "aaaa"), "Should not throw when parameterPosition=0 for string param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue(2, "aaaa"), "Should not throw when parameterPosition>0 for string param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue(2, null), "Should not throw when string is null");

            Assert.Throws<ArgumentException>(() => mockDbMatchingRule.SetParameterValue(-5, 25), "Should throw ArgumentException when parameterPosition<0 for struct param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue(0, "aaaa"), "Should not throw when parameterPosition=0 for struct param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue(2, "aaaa"), "Should not throw when parameterPosition>0 for struct param");

            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetParameterValue(null, "aaaa"), "Should throw ArgumentNullException when parameter name is null for string param");
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetParameterValue(string.Empty, "aaaa"), "Should throw ArgumentNullException when parameter name is null for string param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue("bbbb", "aaaa"), "Should not throw when parameter name is set for string param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue("bbbb", null), "Should not throw when string is null");
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetParameterValue(null, 25), "Should throw ArgumentNullException when parameter name is null for struct param");
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetParameterValue(string.Empty, 25), "Should throw ArgumentNullException when parameter name is null for struct param");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterValue("bbbb", 25), "Should not throw when parameter name is set for struct param");
        }

        [Test]
        public void TestSetCommandTextRegArgument()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetCommandTextReg(null), "Should throw ArgumentNullException when text is null");
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.SetCommandTextReg(string.Empty), "Should throw ArgumentNullException when text is empty");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetCommandTextReg("aaaa"), "Should not throw when text is set");
        }
        [Test]
        public void TestSetParameterCountArgument()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.Throws<ArgumentException>(() => mockDbMatchingRule.SetParameterCount(-5), "Should throw ArgumentException when parameterCount<0");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterCount(0), "Should not throw when parameterCount=0");
            Assert.DoesNotThrow(() => mockDbMatchingRule.SetParameterCount(2), "Should not throw when parameterCount>0");
        }

        [Test]
        public void TestMatchingLevel()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText));
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(10);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue));
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("cfvvbn", 5m);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount));
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount));

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount));
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("cfvvbn", 5m);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(10);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue));
        }
        [Test]
        public void TestSetCommandTextReg()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo("sdfghjklm"));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo("pppee"));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestSetParameterCount()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(5));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(15);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(15));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestSetParameterValue()
        {
            Dictionary<object, object> expected = new Dictionary<object, object>();
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, "fff");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(1));
            expected[5] = "fff";
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, "tttetetet");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(1));
            expected[5] = "tttetetet";
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(6, "tttetetet");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(2));
            expected[6] = "tttetetet";
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("a", 25);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(3));
            expected["a"] = 25;
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("a", null);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(3));
            expected["a"] = null;
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("b", new DateTime());
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(4));
            expected["b"] = new DateTime();
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));
        }

        [Test]

        public void TestImmutable()

        {
            MockDbMatchingRule mockDbMatchingRule2;
            Dictionary<object, object> expected2;
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Dictionary<object, object> expected = new Dictionary<object, object>();
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            // SevParameterValue(T)
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterValue(5, "fff");
            expected2 = new Dictionary<object, object>() { { 5, "fff" } };
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            Assert.That(mockDbMatchingRule2.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule2.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule2.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterValues.Count, Is.EqualTo(1));
            Assert.That(mockDbMatchingRule2.ParameterValues, Is.EqualTo(expected2));

            // SetParameterValue(string)
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterValue("a", 25);
            expected2 = new Dictionary<object, object>() { { "a", 25 } };
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            Assert.That(mockDbMatchingRule2.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule2.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue));
            Assert.That(mockDbMatchingRule2.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterValues.Count, Is.EqualTo(1));
            Assert.That(mockDbMatchingRule2.ParameterValues, Is.EqualTo(expected2));

            //SetCommandTextReg
            mockDbMatchingRule2 = mockDbMatchingRule.SetCommandTextReg("agzgagzag");
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));

            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));
            Assert.That(mockDbMatchingRule2.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule2.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandText));
            Assert.That(mockDbMatchingRule2.CommandTextReg, Is.EqualTo("agzgagzag"));
            Assert.That(mockDbMatchingRule2.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule2.ParameterValues, Is.EqualTo(expected));

            //SetParameterCount
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterCount(5);
            Assert.That(mockDbMatchingRule.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType));
            Assert.That(mockDbMatchingRule.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterCount, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule.ParameterValues, Is.EqualTo(expected));

            Assert.That(mockDbMatchingRule2.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            Assert.That(mockDbMatchingRule2.MatchingLevel, Is.EqualTo(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount));
            Assert.That(mockDbMatchingRule2.CommandTextReg, Is.EqualTo(null));
            Assert.That(mockDbMatchingRule2.ParameterCount, Is.EqualTo(5));
            Assert.That(mockDbMatchingRule2.ParameterValues.Count, Is.EqualTo(0));
            Assert.That(mockDbMatchingRule2.ParameterValues, Is.EqualTo(expected));
        }

        [Test]

        public void TestMatch()
        {
            MockDbMatchingRule mockDbMatchingRule;
            MockDbCommand cmd = null;

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.Throws<ArgumentNullException>(() => mockDbMatchingRule.Match(cmd), "Should throw ArgumentNullException when Command is null");

            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for CommandType.StoredProcedure");

            cmd = new MockDbCommand { CommandType = CommandType.Text };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.Text");

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterCount(2);
            cmd = new MockDbCommand { CommandType = CommandType.Text };

            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.Text");
            for (int i = 1; i < 4; i++)
            {
                cmd.Parameters.Add("Param" + i, DbType.String);
                Assert.That(mockDbMatchingRule.Match(cmd), Is.EqualTo(i == 2), $"Not the expected value for CommandType.ParameterCount={i}");
            }

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetCommandTextReg("TEST");
            cmd = new MockDbCommand { CommandType = CommandType.Text };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.Text only");
            cmd.CommandText = "aaa";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetCommandTextReg with aaa");
            cmd.CommandText = "TEST";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetCommandTextReg with TEST");
            cmd.CommandText = "test";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetCommandTextReg with test");
            cmd.CommandText = "aTESTb";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetCommandTextReg with aTESTb");

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure).SetParameterValue(1, 25);
            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.StoredProcedure only");
            cmd.Parameters.Add("a", DbType.Int32);
            cmd.Parameters.Add("b", DbType.Int32);
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue");
            cmd.Parameters["a"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (1) with a=25");
            cmd.Parameters["b"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (1) with a=25 and b=25");
            cmd.Parameters["a"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (1) with a=\"25\" and b=25");
            cmd.Parameters["b"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (1) with a=\"25\" and b=\"25\"");

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure).SetParameterValue(1, 25).SetParameterValue(0, "25");
            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.StoredProcedure only");
            cmd.Parameters.Add("a", DbType.Int32);
            cmd.Parameters.Add("b", DbType.Int32);
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue");
            cmd.Parameters["a"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (2) with a=25");
            cmd.Parameters["b"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (2) with a=25 and b=25");
            cmd.Parameters["a"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (2) with a=\"25\" and b=25");
            cmd.Parameters["b"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (2) with a=\"25\" and b=\"25\"");

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure).SetParameterValue("b", 25);
            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.StoredProcedure only");
            cmd.Parameters.Add("a", DbType.Int32);
            cmd.Parameters.Add("b", DbType.Int32);
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue");
            cmd.Parameters["a"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (3) with a=25");
            cmd.Parameters["b"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (3) with a=25 and b=25");
            cmd.Parameters["a"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (3) with a=\"25\" and b=25");
            cmd.Parameters["b"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (3) with a=\"25\" and b=\"25\"");

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure).SetParameterValue("b", 25).SetParameterValue("a", "25");
            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.StoredProcedure only");
            cmd.Parameters.Add("a", DbType.Int32);
            cmd.Parameters.Add("b", DbType.Int32);
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue");
            cmd.Parameters["a"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (4) with a=25");
            cmd.Parameters["b"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (4) with a=25 and b=25");
            cmd.Parameters["a"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (4) with a=\"25\" and b=25");
            cmd.Parameters["b"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (4) with a=\"25\" and b=\"25\"");


            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure).SetParameterValue("b", 25).SetParameterValue(0, "25");
            cmd = new MockDbCommand { CommandType = CommandType.StoredProcedure };
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for CommandType.StoredProcedure only");
            cmd.Parameters.Add("a", DbType.Int32);
            cmd.Parameters.Add("b", DbType.Int32);
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue");
            cmd.Parameters["a"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (5) with a=25");
            cmd.Parameters["b"].Value = 25;
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (5) with a=25 and b=25");
            cmd.Parameters["a"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.True, "Not the expected value for SetParameterValue (5) with a=\"25\" and b=25");
            cmd.Parameters["b"].Value = "25";
            Assert.That(mockDbMatchingRule.Match(cmd), Is.False, "Not the expected value for SetParameterValue (5) with a=\"25\" and b=\"25\"");

        }
    }
}
