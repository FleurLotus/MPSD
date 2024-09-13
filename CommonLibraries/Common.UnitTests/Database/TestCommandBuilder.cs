namespace Common.UnitTests.Database
{
    using System;
    using System.Data;
    using System.Data.Common;

    using Common.Database;
    using MockDbData;

    using NUnit.Framework;

    [TestFixture]
    public class TestCommandBuilder
    {
        [DbTable]
        private class DbClass1
        {
            [DbColumn()]
            public string Col1 { get; set; }
        }

        [Test]
        public void TestParameterIsNotNull()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClass1)));
            IDbCommand cnx = new MockDbCommand();

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildSelectAllCommand(null), "Null Command should have thrown ArgumentNullException for BuildSelectAllCommand");

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildSelectOneCommand(null, new object()), "Null Command should have thrown ArgumentNullException for BuildSelectOneCommand");
            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildSelectOneCommand(cnx, null), "Null input should have thrown ArgumentNullException for BuildSelectOneCommand");

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildDeleteAllCommand(null), "Null Command should have thrown ArgumentNullException for BuildDeleteAllCommand");
            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildDeleteOneCommand(null, new object()), "Null Command should have thrown ArgumentNullException for BuildDeleteOneCommand");

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildDeleteOneCommand(cnx, null), "Null input should have thrown ArgumentNullException for BuildDeleteOneCommand");

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildInsertOneCommand(null, new object()), "Null Command should have thrown ArgumentNullException for BuildInsertOneCommand");
            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildInsertOneCommand(cnx, null), "Null input should have thrown ArgumentNullException for BuildInsertOneCommand");

            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildUpdateOneCommand(null, new object()), "Null Command should have thrown ArgumentNullException for BuildUpdateOneCommand");
            Assert.Throws<ArgumentNullException>(() => commandBuilder.BuildUpdateOneCommand(cnx, null), "Null input should have thrown ArgumentNullException for BuildUpdateOneCommand");
        }

        [Test]
        public void TestNoOneUpdateInsertDeleteIfNoKey()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClass1)));
            IDbCommand cnx = new MockDbCommand();
            DbClass1 o = new DbClass1 { Col1 = "1" };

            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteAllCommand(cnx), "BuildDeleteAllCommand should not throw when no Key");
            Assert.Throws<AttributedTypeException>(() => commandBuilder.BuildDeleteOneCommand(cnx, o), "BuildDeleteOneCommand should throw when no Key");
            Assert.Throws<AttributedTypeException>(() => commandBuilder.BuildUpdateOneCommand(cnx, o), "BuildUpdateOneCommand should throw when no Key");
            Assert.DoesNotThrow(() => commandBuilder.BuildInsertOneCommand(cnx, o), "BuildInsertOneCommand should not throw when no Key");
            Assert.Throws<AttributedTypeException>(() => commandBuilder.BuildSelectOneCommand(cnx, o), "BuildSelectOneCommand should throw when no Key");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectAllCommand(cnx), "BuildSelectAllCommand should not throw when no Key");
        }

        [DbTable]
        private class DbClassNoRestriction
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }

        [DbTable]
        [DbRestictedDml(Restriction.Update)]
        private class DbClassNoUpdate
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }

        [DbTable]
        [DbRestictedDml(Restriction.Insert)]
        private class DbClassNoInsert
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }

        [DbTable]
        [DbRestictedDml(Restriction.Delete)]
        private class DbClassNoDelete

        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }

        [Test]
        public void TestDMLRestrictionNoRestriction()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClassNoRestriction)));
            IDbCommand cnx = new MockDbCommand();
            DbClassNoRestriction o = new DbClassNoRestriction { Col1 = "1", Col2 = "2" };

            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteAllCommand(cnx), "BuildDeleteAllCommand should not throw for DbClassNoRestriction");
            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteOneCommand(cnx, o), "BuildDeleteOneCommand should not throw for DbClassNoRestriction");
            Assert.DoesNotThrow(() => commandBuilder.BuildUpdateOneCommand(cnx, o), "BuildUpdateOneCommand should not throw for DbClassNoRestriction");
            Assert.DoesNotThrow(() => commandBuilder.BuildInsertOneCommand(cnx, o), "BuildInsertOneCommand should not throw for DbClassNoRestriction");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectOneCommand(cnx, o), "BuildSelectOneCommand should not throw for DbClassNoRestriction");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectAllCommand(cnx), "BuildSelectAllCommand should not throw for DbClassNoRestriction");
        }

        [Test]
        public void TestDMLRestrictionNoDelete()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClassNoDelete)));
            IDbCommand cnx = new MockDbCommand();
            DbClassNoDelete o = new DbClassNoDelete { Col1 = "1", Col2 = "2" };

            Assert.Throws<RestrictedDmlException>(() => commandBuilder.BuildDeleteAllCommand(cnx), "BuildDeleteAllCommand should throw for DbClassNoDelete");
            Assert.Throws<RestrictedDmlException>(() => commandBuilder.BuildDeleteOneCommand(cnx, o), "BuildDeleteOneCommand should throw for DbClassNoDelete");
            Assert.DoesNotThrow(() => commandBuilder.BuildUpdateOneCommand(cnx, o), "BuildUpdateOneCommand should not throw for DbClassNoDelete");
            Assert.DoesNotThrow(() => commandBuilder.BuildInsertOneCommand(cnx, o), "BuildInsertOneCommand should not throw for DbClassNoDelete");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectOneCommand(cnx, o), "BuildSelectOneCommand should not throw for DbClassNoDelete");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectAllCommand(cnx), "BuildSelectAllCommand should not throw for DbClassNoDelete");
        }

        [Test]
        public void TestDMLRestrictionNoInsert()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClassNoInsert)));
            IDbCommand cnx = new MockDbCommand();
            DbClassNoInsert o = new DbClassNoInsert { Col1 = "1", Col2 = "2" };

            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteAllCommand(cnx), "BuildDeleteAllCommand should not throw for DbClassNoInsert");
            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteOneCommand(cnx, o), "BuildDeleteOneCommand should not throw for DbClassNoInsert");
            Assert.DoesNotThrow(() => commandBuilder.BuildUpdateOneCommand(cnx, o), "BuildUpdateOneCommand should not throw for DbClassNoInsert");
            Assert.Throws<RestrictedDmlException>(() => commandBuilder.BuildInsertOneCommand(cnx, o), "BuildInsertOneCommand should throw for DbClassNoInsert");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectOneCommand(cnx, o), "BuildSelectOneCommand should not throw for DbClassNoInsert");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectAllCommand(cnx), "BuildSelectAllCommand should not throw for DbClassNoInsert");
        }

        [Test]
        public void TestDMLRestrictionNoUpdate()
        {
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbClassNoUpdate)));
            IDbCommand cnx = new MockDbCommand();
            DbClassNoUpdate o = new DbClassNoUpdate { Col1 = "1", Col2 = "2" };

            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteAllCommand(cnx), "BuildDeleteAllCommand should not throw for DbClassNoUpdate");
            Assert.DoesNotThrow(() => commandBuilder.BuildDeleteOneCommand(cnx, o), "BuildDeleteOneCommand should not throw for DbClassNoUpdate");
            Assert.Throws<RestrictedDmlException>(() => commandBuilder.BuildUpdateOneCommand(cnx, o), "BuildUpdateOneCommand should throw for DbClassNoUpdate");
            Assert.DoesNotThrow(() => commandBuilder.BuildInsertOneCommand(cnx, o), "BuildInsertOneCommand should not throw for DbClassNoUpdate");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectOneCommand(cnx, o), "BuildSelectOneCommand should not throw for DbClassNoUpdate");
            Assert.DoesNotThrow(() => commandBuilder.BuildSelectAllCommand(cnx), "BuildSelectAllCommand should not throw for DbClassNoUpdate");

        }
        [DbTable(Name = "MyTable")]
        private class DbWithAlias
        {
            [DbColumn(Name = "MyKey", Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn(Name = "MyValue")]
            public string Col2 { get; set; }
        }

        [Test]
        public void TestWithAlias()
        {
            IDbCommand cnx;
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithAlias)));
            DbWithAlias o = new DbWithAlias { Col1 = "1", Col2 = "2" };
            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [MyTable] WHERE ([MyKey] = @MyKey)"), "Not the expected DeleteOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@MyKey"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyKey"]).Value, Is.EqualTo("1"), "Not the expected value for @MyKey");

            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [MyTable]"), "Not the expected DeleteAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [MyKey], [MyValue] FROM [MyTable] WHERE ([MyKey] = @MyKey)"), "Not the expected SelectOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@MyKey"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyKey"]).Value, Is.EqualTo("1"), "Not the expected value for @MyKey");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [MyKey], [MyValue] FROM [MyTable]"), "Not the expected SelectAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildUpdateOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("UPDATE [MyTable] SET [MyValue] = @MyValue WHERE ([MyKey] = @MyKey)"), "Not the expected UpdateOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(2), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@MyKey"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyKey"]).Value, Is.EqualTo("1"), "Not the expected value for @MyKey");
            Assert.That(cnx.Parameters.Contains("@MyValue"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyValue"]).Value, Is.EqualTo("2"), "Not the expected value for @MyValue");

            cnx = new MockDbCommand();
            commandBuilder.BuildInsertOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("INSERT INTO [MyTable] ([MyKey], [MyValue]) VALUES (@MyKey, @MyValue)"), "Not the expected InsertOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(2), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@MyKey"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyKey"]).Value, Is.EqualTo("1"), "Not the expected value for @MyKey");
            Assert.That(cnx.Parameters.Contains("@MyValue"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@MyValue"]).Value, Is.EqualTo("2"), "Not the expected value for @MyValue");
        }

        [DbTable()]
        private class DbWithIdentity
        {
            [DbColumn(Kind = ColumnKind.Identity)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }

        [Test]
        public void TestWithIdentity()
        {
            IDbCommand cnx;
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithIdentity)));
            DbWithIdentity o = new DbWithIdentity { Col1 = "1", Col2 = "2" };

            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithIdentity] WHERE ([Col1] = @Col1)"), "Not the expected DeleteOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");

            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithIdentity]"), "Not the expected DeleteAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2] FROM [DbWithIdentity] WHERE ([Col1] = @Col1)"), "Not the expected SelectOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2] FROM [DbWithIdentity]"), "Not the expected SelectAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildUpdateOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("UPDATE [DbWithIdentity] SET [Col2] = @Col2 WHERE ([Col1] = @Col1)"), "Not the expected UpdateOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(2), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");

            cnx = new MockDbCommand();
            commandBuilder.BuildInsertOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("INSERT INTO [DbWithIdentity] ([Col2]) VALUES (@Col2)"), "Not the expected InsertOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");
        }

        [DbTable()]
        private class DbWithPrimaryMultiColumn
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col3 { get; set; }
        }

        [Test]
        public void TestWithPrimaryMultiColumn()
        {
            IDbCommand cnx;
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithPrimaryMultiColumn)));
            DbWithPrimaryMultiColumn o = new DbWithPrimaryMultiColumn { Col1 = "1", Col2 = "2", Col3 = "3" };
            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithPrimaryMultiColumn] WHERE ([Col1] = @Col1) AND ([Col3] = @Col3)"), "Not the expected DeleteOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(2), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo("3"), "Not the expected value for @Col3");

            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithPrimaryMultiColumn]"), "Not the expected DeleteAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2], [Col3] FROM [DbWithPrimaryMultiColumn] WHERE ([Col1] = @Col1) AND ([Col3] = @Col3)"), "Not the expected SelectOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(2), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo("3"), "Not the expected value for @Col3");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2], [Col3] FROM [DbWithPrimaryMultiColumn]"), "Not the expected SelectAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildUpdateOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("UPDATE [DbWithPrimaryMultiColumn] SET [Col2] = @Col2 WHERE ([Col1] = @Col1) AND ([Col3] = @Col3)"), "Not the expected UpdateOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(3), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo("3"), "Not the expected value for @Col3");

            cnx = new MockDbCommand();
            commandBuilder.BuildInsertOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("INSERT INTO [DbWithPrimaryMultiColumn] ([Col1], [Col2], [Col3]) VALUES (@Col1, @Col2, @Col3)"), "Not the expected InsertOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(3), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo("3"), "Not the expected value for @Col3");
        }

        [DbTable()]
        private class DbWithMultiColumn
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
            [DbColumn()]
            public int Col3 { get; set; }
            [DbColumn()]
            public double Col4 { get; set; }
            [DbColumn()]
            public bool Col5 { get; set; }
        }
        [Test]
        public void TesWithMultipleColumn()
        {
            IDbCommand cnx;
            CommandBuilder commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithMultiColumn)));
            DbWithMultiColumn o = new DbWithMultiColumn { Col1 = "1", Col2 = "2", Col3 = 3, Col4 = 4.0, Col5 = true };
            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithMultiColumn] WHERE ([Col1] = @Col1)"), "Not the expected DeleteOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");

            cnx = new MockDbCommand();
            commandBuilder.BuildDeleteAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("DELETE FROM [DbWithMultiColumn]"), "Not the expected DeleteAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2], [Col3], [Col4], [Col5] FROM [DbWithMultiColumn] WHERE ([Col1] = @Col1)"), "Not the expected SelectOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(1), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");

            cnx = new MockDbCommand();
            commandBuilder.BuildSelectAllCommand(cnx);
            Assert.That(cnx.CommandText, Is.EqualTo("SELECT [Col1], [Col2], [Col3], [Col4], [Col5] FROM [DbWithMultiColumn]"), "Not the expected SelectAll CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(0), "Not the expected number of parameters");

            cnx = new MockDbCommand();
            commandBuilder.BuildUpdateOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("UPDATE [DbWithMultiColumn] SET [Col2] = @Col2, [Col3] = @Col3, [Col4] = @Col4, [Col5] = @Col5 WHERE ([Col1] = @Col1)"), "Not the expected UpdateOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(5), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo(3), "Not the expected value for @Col3");
            Assert.That(cnx.Parameters.Contains("@Col4"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col4"]).Value, Is.EqualTo(4.0), "Not the expected value for @Col4");
            Assert.That(cnx.Parameters.Contains("@Col5"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col5"]).Value, Is.True, "Not the expected value for @Col5");

            cnx = new MockDbCommand();
            commandBuilder.BuildInsertOneCommand(cnx, o);
            Assert.That(cnx.CommandText, Is.EqualTo("INSERT INTO [DbWithMultiColumn] ([Col1], [Col2], [Col3], [Col4], [Col5]) VALUES (@Col1, @Col2, @Col3, @Col4, @Col5)"), "Not the expected InsertOne CommandText");
            Assert.That(cnx.Parameters.Count, Is.EqualTo(5), "Not the expected number of parameters");
            Assert.That(cnx.Parameters.Contains("@Col1"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col1"]).Value, Is.EqualTo("1"), "Not the expected value for @Col1");
            Assert.That(cnx.Parameters.Contains("@Col2"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col2"]).Value, Is.EqualTo("2"), "Not the expected value for @Col2");
            Assert.That(cnx.Parameters.Contains("@Col3"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col3"]).Value, Is.EqualTo(3), "Not the expected value for @Col3");
            Assert.That(cnx.Parameters.Contains("@Col4"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col4"]).Value, Is.EqualTo(4.0), "Not the expected value for @Col4");
            Assert.That(cnx.Parameters.Contains("@Col5"), Is.True, "Not expected parameter");
            Assert.That(((DbParameter)cnx.Parameters["@Col5"]).Value, Is.True, "Not the expected value for @Col5");
        }

        [Test]
        public void TestGetIdKey()
        {
            CommandBuilder commandBuilder;

            commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithIdentity)));
            Assert.That(commandBuilder.GetIdKeyPropertyInfo(), Is.EqualTo(typeof(DbWithIdentity).GetProperty(nameof(DbWithIdentity.Col1))), "Not the expected value for GetIdKeyPropertyInfo with identity");

            commandBuilder = new CommandBuilder(DbAttributAnalyser.Analyse(typeof(DbWithAlias)));
            Assert.That(commandBuilder.GetIdKeyPropertyInfo(), Is.Null, "GetIdKeyPropertyInfo should be null when no identity");
        }
    }
}