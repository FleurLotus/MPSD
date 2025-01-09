namespace MockDbData.UnitTests
{
    using System;
    using System.Data;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbParameterCollection
    {
        [Test]
        public void TestConstructorByValue()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.That(parameters, Is.Not.Null);
            Assert.That(parameters.Count, Is.EqualTo(0));
            Assert.That(parameters.IsFixedSize, Is.False);
            Assert.That(parameters.IsReadOnly, Is.False);
            Assert.That(parameters.IsSynchronized, Is.False);
            Assert.That(parameters.SyncRoot, Is.Null);
        }
        [Test]
        public void TestAddByType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.That(parameters, Is.Not.Null);
            Assert.That(parameters.Count, Is.EqualTo(0));

            parameters.Add("azerty", DbType.String);
            Assert.That(parameters.Count, Is.EqualTo(1));

            MockDbParameter param = parameters["azerty"];
            Assert.That(param, Is.Not.Null);
            Assert.That(param.DbType, Is.EqualTo(DbType.String));
            Assert.That(param.ParameterName, Is.EqualTo("azerty"));
            Assert.That(parameters[0], Is.EqualTo(param));
        }
        [Test]
        public void TestAddByValue()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.That(parameters, Is.Not.Null);
            Assert.That(parameters.Count, Is.EqualTo(0));

            parameters.AddWithValue("qsdfg", 42);
            Assert.That(parameters.Count, Is.EqualTo(1));

            MockDbParameter param = parameters["qsdfg"];
            Assert.That(param, Is.Not.Null);
            Assert.That(param.Value, Is.EqualTo(42));
            Assert.That(param.ParameterName, Is.EqualTo("qsdfg"));
            Assert.That(parameters[0], Is.EqualTo(param));
        }
        [Test]
        public void TestAddWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.Add("tatat"));
        }
        [Test]
        public void TestAdd()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add(parameter);
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters[0], Is.EqualTo(parameter));
            Assert.That(parameters[parameter.ParameterName], Is.EqualTo(parameter));
        }
        [Test]
        public void TestAddObject()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add((object)parameter);
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters[0], Is.EqualTo(parameter));
            Assert.That(parameters[parameter.ParameterName], Is.EqualTo(parameter));
        }
        [Test]
        public void TestAddRangeWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.AddRange(new[] { "tatat" }));
        }
        [Test]
        public void TestAddRange()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter{ Direction = ParameterDirection.Output};
            MockDbParameter parameter2 = new MockDbParameter("aaaaa", "zzzz");

            parameters.AddRange(new[] { parameter, parameter2});
            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters[0], Is.EqualTo(parameter));
            Assert.That(parameters[1], Is.EqualTo(parameter2));
            Assert.That(parameters[parameter2.ParameterName], Is.EqualTo(parameter2));
        }
        [Test]
        public void TestAddRangeArray()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter { Direction = ParameterDirection.Output };
            MockDbParameter parameter2 = new MockDbParameter("aaaaa", "zzzz");

            parameters.AddRange(new object[] { parameter, parameter2 });
            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters[0], Is.EqualTo(parameter));
            Assert.That(parameters[1], Is.EqualTo(parameter2));
            Assert.That(parameters[parameter2.ParameterName], Is.EqualTo(parameter2));
        }
        [Test]
        public void TestClear()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter { Direction = ParameterDirection.Output };
            MockDbParameter parameter2 = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add(parameter);
            parameters.Add(parameter2);

            Assert.That(parameters.Count, Is.EqualTo(2));

            parameters.Clear();
            Assert.That(parameters.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestContains()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add(parameter);

            Assert.That(parameters.Contains(parameter.ParameterName), Is.True);
            Assert.That(parameters.Contains("bbbb"), Is.False);
        }
        [Test]
        public void TestContainsObject()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add(parameter);

            Assert.That(parameters.Contains(parameter), Is.True);
            Assert.That(parameters.Contains(new MockDbParameter()), Is.False);
        }
        [Test]
        public void TestContainsWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.Contains(new object()));
        }
        [Test]
        public void TestRemoveAtString()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");

            parameters.Add(parameter);
            Assert.That(parameters.Count, Is.EqualTo(1));
            parameters.RemoveAt(parameter.ParameterName);
            Assert.That(parameters.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestRemoveAtStringNotFound()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            parameters.Add(parameter);

            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.RemoveAt("bbbb"));
        }
        [Test]
        public void TestRemoveAtInt()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            parameters.Add(parameter);
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            parameters.Add(parameter2);

            Assert.That(parameters.Count, Is.EqualTo(2));
            parameters.RemoveAt(0);
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters[0], Is.EqualTo(parameter2));

            parameters.RemoveAt(0);
            Assert.That(parameters.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestRemoveAtIntNotFound()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            parameters.Add(parameter);

            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.RemoveAt(1));
        }
        [Test]
        public void TestRemove()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            parameters.Add(parameter);
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            parameters.Add(parameter2);

            Assert.That(parameters.Count, Is.EqualTo(2));
            parameters.Remove(parameter);
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters[0], Is.EqualTo(parameter2));

            parameters.Remove(parameter2);
            Assert.That(parameters.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestRemoveWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.Remove(new object()));
        }
        [Test]
        public void TestIndexOf()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            parameters.Add(parameter);
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            parameters.Add(parameter2);

            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters.IndexOf(parameter), Is.EqualTo(0));
            Assert.That(parameters.IndexOf(parameter2), Is.EqualTo(1));
            Assert.That(parameters.IndexOf(new MockDbParameter()), Is.EqualTo(-1));
        }
        [Test]
        public void TestIndexOfWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.IndexOf(new object()));
        }
        [Test]
        public void TestInsert()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            MockDbParameter parameter3 = new MockDbParameter("ccccc", "xxxx");

            Assert.That(parameters.Count, Is.EqualTo(0));
            parameters.Insert(0, parameter);
            parameters.Insert(0, parameter2);
            parameters.Insert(2, parameter3);
            Assert.That(parameters.Count, Is.EqualTo(3));
            Assert.That(parameters[0], Is.EqualTo(parameter2));
            Assert.That(parameters[1], Is.EqualTo(parameter));
            Assert.That(parameters[2], Is.EqualTo(parameter3));
        }
        [Test]
        public void TestInsertWrongIndex()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Insert(1, new MockDbParameter()));
        }
        [Test]
        public void TestInsertWrongType()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<InvalidCastException>(() => parameters.Insert(0, new object()));
        }
        [Test]
        public void TestIndexerIntSet()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            MockDbParameter parameter3 = new MockDbParameter("ccccc", "xxxx");

            parameters.Add(parameter);
            parameters.Add(parameter2);
            Assert.That(parameters.Count, Is.EqualTo(2));

            parameters[0] = parameter3;
            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters[0], Is.EqualTo(parameter3));
            Assert.That(parameters[1], Is.EqualTo(parameter2));
        }
        [Test]
        public void TestIndexerIntSetWrongIndex()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters[1] = new MockDbParameter());
        }
        [Test]
        public void TestIndexerStringSet()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");
            MockDbParameter parameter3 = new MockDbParameter("aaaaa", "xxxx");

            parameters.Add(parameter);
            parameters.Add(parameter2);
            Assert.That(parameters.Count, Is.EqualTo(2));

            parameters["aaaaa"] = parameter3;
            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters["aaaaa"], Is.EqualTo(parameter3));
            Assert.That(parameters["bbbbb"], Is.EqualTo(parameter2));
        }
        [Test]
        public void TestGetEnumerator()
        {
            MockDbParameterCollection parameters = new MockDbParameterCollection();
            MockDbParameter parameter = new MockDbParameter("aaaaa", "zzzz");
            MockDbParameter parameter2 = new MockDbParameter("bbbbb", "yyyy");

            parameters.Add(parameter);
            parameters.Add(parameter2);

            Assert.That(parameters, Is.EquivalentTo(new [] { parameter, parameter2 }));
        }
    }
}