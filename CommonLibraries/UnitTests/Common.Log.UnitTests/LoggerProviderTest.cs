namespace Common.Log.UnitTests
{
    using System;
    using System.IO;

    using Common.Log;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    [TestFixture]
    public class LoggerProviderTest
    {
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _path = Path.GetTempFileName();
            Assert.That(File.Exists(_path), Is.True, "Log file should be created");
        }
        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_path))
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
                    // Ignore exceptions during cleanup
                }
            }
        }

        [Test]
        public void TestLogWithExtension()
        {
            FileLoggerProvider provider = new FileLoggerProvider(_path);
            provider.CreateLogger("Test").LogInformation("Test message");

            string content = File.ReadAllLines(_path)[0];
            Assert.That(content, Does.Contain("Test message"), "Log file should contain the logged message");
        }
        [Test]
        public void TestLog()
        {
            FileLoggerProvider provider = new FileLoggerProvider(_path);
            EventId eventId = new EventId(1, "TestEvent");
            provider.CreateLogger("Test").Log(LogLevel.Information, eventId, "Test message", (Exception) null, null);

            string content = File.ReadAllLines(_path)[0];
            Assert.That(content, Does.Contain("Test message"), "Log file should contain the logged message");
        }
        [Test]
        public void TestLogException()
        {
            FileLoggerProvider provider = new FileLoggerProvider(_path);
            EventId eventId = new EventId(1, "TestEvent");
            provider.CreateLogger("Test").Log(LogLevel.Information, eventId, (string) null, new Exception("Test Error"), null);

            string content = File.ReadAllLines(_path)[0];
            Assert.That(content, Does.Contain("Test Error"), "Log file should contain the logged message");
        }
        [Test]
        public void TestNoLog()
        {
            FileLoggerProvider provider = new FileLoggerProvider(_path);
            EventId eventId = new EventId(1, "TestEvent");
            provider.CreateLogger("Test").Log(LogLevel.None, eventId, "Test message", (Exception) null, null);

            string content = File.ReadAllText(_path);
            Assert.That(content, Is.Empty, "Log file should be empty when log level is None");
        }
    }
}
