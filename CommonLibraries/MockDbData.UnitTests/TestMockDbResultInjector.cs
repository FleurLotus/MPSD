namespace MockDbData.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbResultInjector
    {
        [Test]
        public void TestAddResultArgument()
        {
            MockDbMatchingRule mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            MockDbResult mockDbResult = new MockDbResult(new DataTable());
            MockDbResultInjector mockDbResultInjector = new MockDbResultInjector();
            Assert.Throws<ArgumentNullException>(() => mockDbResultInjector.AddResult(null, mockDbResult), "Should throw ArgumentNullException when matchingRule is null");
            Assert.Throws<ArgumentNullException>(() => mockDbResultInjector.AddResult(mockDbMatchingRule, null), "Should throw ArgumentNullException when mockDbResult is null");
            Assert.DoesNotThrow(() => mockDbResultInjector.AddResult(mockDbMatchingRule, mockDbResult), "Should not throw when both args are not null");
        }
        [Test]
        public void TestAddGlobalResultArgument()
        {
            MockDbResult mockDbResult = new MockDbResult(new DataTable());
            MockDbResultInjector mockDbResultInjector = new MockDbResultInjector();
            Assert.Throws<ArgumentNullException>(() => mockDbResultInjector.AddGlobalResult(null), "Should throw ArgumentNullException when mockDResult is null");
            Assert.DoesNotThrow(() => mockDbResultInjector.AddGlobalResult(mockDbResult), "Should not throw when mockDbResult is not null");
        }
        [Test]
        public void TestGetMockDbResultArgument()
        {
            MockDbResultInjector mockDbResultInjector = new MockDbResultInjector();
            Assert.Throws<ArgumentNullException>(() => mockDbResultInjector.GetMockDbResult(null), "Should throw ArgumentNullException when command is null");
        }

        [TestCaseSource("TestGetMockDbResultSource")]
        public MockDbResult TestGetMockDbResult(MockDbResultInjector mockDbResultInjector, DbCommand cmd)
        {
            return mockDbResultInjector.GetMockDbResult(cmd); ;
        }
        public static IEnumerable<TestCaseData> TestGetMockDbResultSource()
        {
            MockDbResultInjector mockDbResultInjector;
            MockDbCommand mockDbCommand;
            MockDbMatchingRule mockDbMatchingRule;

            MockDbResult globalMockDbResult = new MockDbResult(new DataTable());
            MockDbResult mockDbResult = new MockDbResult(new DataTable());
            MockDbResult typeMockDbResult = new MockDbResult(new DataTable());
            MockDbResult mockDbResult1 = new MockDbResult(new DataTable());
            MockDbResult mockDbResult2 = new MockDbResult(new DataTable());
            MockDbResult textMockDbResult = new MockDbResult(new DataTable());
            MockDbResult parameterValueMockDbResult = new MockDbResult(new DataTable());
            MockDbResult parameterValueMockDbResult2 = new MockDbResult(new DataTable());
            MockDbResult parameterValueMockDbResult3 = new MockDbResult(new DataTable());
            MockDbResult parameterCountMockDbResult = new MockDbResult(new DataTable());

            mockDbResultInjector = new MockDbResultInjector();
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(null).SetName("No Data");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(mockDbResult);
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(mockDbResult).SetName("Only global");


            mockDbResultInjector = new MockDbResultInjector();
            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            mockDbResultInjector.AddResult(mockDbMatchingRule, mockDbResult);
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(mockDbResult).SetName("One matching rule");
            mockDbCommand = new MockDbCommand { CommandType = CommandType.StoredProcedure, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(null).SetName("One not matching Rule");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            mockDbResultInjector.AddResult(mockDbMatchingRule, mockDbResult);
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(mockDbResult).SetName("One matching rule and global");
            mockDbCommand = new MockDbCommand { CommandType = CommandType.StoredProcedure, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(globalMockDbResult).SetName("One not matching rule and global");

            mockDbResultInjector = new MockDbResultInjector();
            MockDbMatchingRule mockDbMatchingRule1 = MockDbMatchingRule.CreateRule(CommandType.Text).SetCommandTextReg("TEST1");
            mockDbResultInjector.AddResult(mockDbMatchingRule1, mockDbResult1);
            MockDbMatchingRule mockDbMatchingRule2 = MockDbMatchingRule.CreateRule(CommandType.Text).SetCommandTextReg("TEST2");
            mockDbResultInjector.AddResult(mockDbMatchingRule2, mockDbResult2);
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(mockDbResult1).SetName("Multiple rules - One matching rule and global (1)");
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST2" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(mockDbResult2).SetName("Multiple rules - One matching rule and global (2)");
            mockDbCommand = new MockDbCommand { CommandType = CommandType.StoredProcedure, CommandText = "TEST1" };
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(globalMockDbResult).SetName("Multiple rules - not matching rule and global");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbCommand = new MockDbCommand { CommandType = CommandType.Text, CommandText = "TEST1" };
            mockDbCommand.Parameters.Add("AAA", DbType.String).Value = "aaaaa";
            mockDbCommand.Parameters.Add("BBB", DbType.String).Value = "aaaaa";
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(globalMockDbResult).SetName("Multiple rules - Global");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            MockDbMatchingRule typeMockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text);
            mockDbResultInjector.AddResult(typeMockDbMatchingRule, typeMockDbResult);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(typeMockDbResult).SetName("Multiple rules - Type over global");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbResultInjector.AddResult(typeMockDbMatchingRule, typeMockDbResult);
            MockDbMatchingRule parameterValueMockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterValue(0, "aaaaa");
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule, parameterValueMockDbResult);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(parameterValueMockDbResult).SetName("Multi matching rules - Parameter value over CommandType");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbResultInjector.AddResult(typeMockDbMatchingRule, typeMockDbResult);
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule, parameterValueMockDbResult);
            MockDbMatchingRule parameterCountMockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterCount(2);
            mockDbResultInjector.AddResult(parameterCountMockDbMatchingRule, parameterCountMockDbResult);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(parameterCountMockDbResult).SetName("Multi matching rule - Parameter Count over Parameter value");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddGlobalResult(globalMockDbResult);
            mockDbResultInjector.AddResult(typeMockDbMatchingRule, typeMockDbResult);
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule, parameterValueMockDbResult);
            mockDbResultInjector.AddResult(parameterCountMockDbMatchingRule, parameterCountMockDbResult);
            MockDbMatchingRule textMockDbMatchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetCommandTextReg("TEST1");
            mockDbResultInjector.AddResult(textMockDbMatchingRule, textMockDbResult);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(textMockDbResult).SetName("Multi matching rule - Text over other");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule, parameterValueMockDbResult);
            MockDbMatchingRule parameterValueMockDbMatchingRule2 = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterValue(1, "aaaaa").SetParameterValue(0, "aaaaa");
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule2, parameterValueMockDbResult2);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(parameterValueMockDbResult2).SetName("Multi matching rule - greater parameter value");

            mockDbResultInjector = new MockDbResultInjector();
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule, parameterValueMockDbResult);
            MockDbMatchingRule parameterValueMockDbMatchingRule3 = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterValue(1, "aaaaa");
            mockDbResultInjector.AddResult(parameterValueMockDbMatchingRule3, parameterValueMockDbResult3);
            yield return new TestCaseData(mockDbResultInjector, mockDbCommand).Returns(parameterValueMockDbResult).SetName("Multi matching rule - first if all same number of parameter value");

        }
    }
}