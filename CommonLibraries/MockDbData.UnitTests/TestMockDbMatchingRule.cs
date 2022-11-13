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
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(10);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("cfvvbn", 5m);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(3);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(3, "");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, 25);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterValue | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("cfvvbn", 5m);
            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(10);
            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("gfhxfhdhfdh");
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText | MatchingLevel.CommandParameterCount | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
        }
        [Test]
        public void TestSetCommandTextReg()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            Assert.AreEqual(CommandType.Text, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("sdfghjklm");
            Assert.AreEqual(CommandType.Text, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual("sdfghjklm", mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);

            mockDbMatchingRule = mockDbMatchingRule.SetCommandTextReg("pppee");
            Assert.AreEqual(CommandType.Text, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual("pppee", mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
        }

        [Test]
        public void TestSetParameterCount()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(5);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(5, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterCount(15);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(15, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
        }
        [Test]
        public void TestSetParameterValue()
        {
            Dictionary<object, object> expected = new Dictionary<object, object>();
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, "fff");
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(1, mockDbMatchingRule.ParameterValues.Count);
            expected[5] = "fff";
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(5, "tttetetet");
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(1, mockDbMatchingRule.ParameterValues.Count);
            expected[5] = "tttetetet";
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue(6, "tttetetet");
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(2, mockDbMatchingRule.ParameterValues.Count);
            expected[6] = "tttetetet";
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("a", 25);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(3, mockDbMatchingRule.ParameterValues.Count);
            expected["a"] = 25;
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("a", null);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(3, mockDbMatchingRule.ParameterValues.Count);
            expected["a"] = null;
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            mockDbMatchingRule = mockDbMatchingRule.SetParameterValue("b", new DateTime());
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(4, mockDbMatchingRule.ParameterValues.Count);
            expected["b"] = new DateTime();
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);
        }

        [Test]

        public void TestImmutable()

        {
            MockDbMatchingRule mockDbMatchingRule2;
            Dictionary<object, object> expected2;
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.StoredProcedure);
            Dictionary<object, object> expected = new Dictionary<object, object>();
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            // SevParameterValue(T)
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterValue(5, "fff");
            expected2 = new Dictionary<object, object>() { { 5, "fff" } };
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule2.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule2.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule2.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule2.ParameterCount);
            Assert.AreEqual(1, mockDbMatchingRule2.ParameterValues.Count);
            CollectionAssert.AreEqual(expected2, mockDbMatchingRule2.ParameterValues);

            // SetParameterValue(string)
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterValue("a", 25);
            expected2 = new Dictionary<object, object>() { { "a", 25 } };
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule2.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterValue, mockDbMatchingRule2.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule2.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule2.ParameterCount);
            Assert.AreEqual(1, mockDbMatchingRule2.ParameterValues.Count);
            CollectionAssert.AreEqual(expected2, mockDbMatchingRule2.ParameterValues);

            //SetCommandTextReg
            mockDbMatchingRule2 = mockDbMatchingRule.SetCommandTextReg("agzgagzag");
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);

            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule2.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandText, mockDbMatchingRule2.MatchingLevel);
            Assert.AreEqual("agzgagzag", mockDbMatchingRule2.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule2.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule2.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule2.ParameterValues);

            //SetParameterCount
            mockDbMatchingRule2 = mockDbMatchingRule.SetParameterCount(5);
            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType, mockDbMatchingRule.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule.CommandTextReg);
            Assert.AreEqual(null, mockDbMatchingRule.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule.ParameterValues);

            Assert.AreEqual(CommandType.StoredProcedure, mockDbMatchingRule2.CommandType);
            Assert.AreEqual(MatchingLevel.CommandType | MatchingLevel.CommandParameterCount, mockDbMatchingRule2.MatchingLevel);
            Assert.AreEqual(null, mockDbMatchingRule2.CommandTextReg);
            Assert.AreEqual(5, mockDbMatchingRule2.ParameterCount);
            Assert.AreEqual(0, mockDbMatchingRule2.ParameterValues.Count);
            CollectionAssert.AreEqual(expected, mockDbMatchingRule2.ParameterValues);
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
