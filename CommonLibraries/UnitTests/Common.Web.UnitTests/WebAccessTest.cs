namespace Common.Web.UnitTests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class WebAccessTest
    {
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private Mock<HttpMessageHandler> _httpMessageHandlerWithLogin;
        private Mock<IHttpMessageHandlerFactory> _httpMessageHandlerFactory;
        private const string FakeAddress = "http://localhost:9999/";
        private const string Login = "Login";

        [SetUp]
        public void SetUp()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandlerWithLogin = new Mock<HttpMessageHandler>();

            _httpMessageHandlerFactory = new Mock<IHttpMessageHandlerFactory>();
            _httpMessageHandlerFactory.Setup(h => h.Create(null)).Returns(_httpMessageHandler.Object);
            _httpMessageHandlerFactory.Setup(h => h.Create(It.Is<ICredentials>(c => c is NetworkCredential && ((NetworkCredential) c).UserName == Login))).Returns(_httpMessageHandlerWithLogin.Object);
        }
        [Test]
        public void TestConstructor()
        {
            WebAccess access = new WebAccess();

            Assert.That(access, Is.Not.Null);
        }
        [Test]
        public void TestConstructorWithTimeout()
        {
            WebAccess access = new WebAccess(TimeSpan.FromMilliseconds(10));

            Assert.That(access, Is.Not.Null);
        }
        [Test]
        public void TestGetHtml()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);
            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);

            string ret = access.GetHtml(FakeAddress);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));
        }

        [Test]
        public void TestGetHtmlWithReload()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);
            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);

            string ret = access.GetHtml(FakeAddress);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));

            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test2", HttpStatusCode.OK);
            ret = access.GetHtml(FakeAddress);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));

            ret = access.GetHtml(FakeAddress, true);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test2")));
        }
        [Test]
        public void TestGetHtmlWithProxyRetry()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);

            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ThrowsAsync(new WebException("407"));
            _httpMessageHandlerWithLogin.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);

            access.CredentialRequiered += (o, e) =>
            {
                e.Data.Login = Login;
                e.Data.Password = "*****";
            };

            string ret = access.GetHtml(FakeAddress);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));
        }
        [Test]
        public void TestGetHtmlNoRetryIfNoCredential()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);
            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ThrowsAsync(new WebException("407"));

            bool called = false;

            access.CredentialRequiered += (o, e) => called = true;

            Assert.Throws<WebException>(() => access.GetHtml(FakeAddress));
            Assert.That(called, Is.True);
        }
        [Test]
        public void TestGetFile()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);
            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);

            byte[] bs = access.GetFile(FakeAddress);
            string ret = Encoding.Default.GetString(bs);
            Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));
        }
        [Test]
        public void TestDownloadFile()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);
            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);
            string tempfile = Path.GetRandomFileName();

            try
            {
                access.DownloadFile(FakeAddress, tempfile);
                Assert.That(File.Exists(tempfile), Is.True);
                string ret = File.ReadAllText(tempfile);
                Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));
            }
            finally
            {
                if (File.Exists(tempfile))
                {
                    File.Delete(tempfile);
                }
            }
        }
        [Test]
        public void TestDownloadFileWithProxyRetry()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);

            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ThrowsAsync(new WebException("407"));
            _httpMessageHandlerWithLogin.SetupSendAsync(HttpMethod.Get, FakeAddress).ReturnsHttpResponseAsync("test", HttpStatusCode.OK);

            access.CredentialRequiered += (o, e) =>
            {
                e.Data.Login = Login;
                e.Data.Password = "*****";
            };

            string tempfile = Path.GetRandomFileName();

            try
            {
                access.DownloadFile(FakeAddress, tempfile);
                Assert.That(File.Exists(tempfile), Is.True);
                string ret = File.ReadAllText(tempfile);
                Assert.That(ret, Is.EqualTo(JsonConvert.SerializeObject("test")));
            }
            finally
            {
                if (File.Exists(tempfile))
                {
                    File.Delete(tempfile);
                }
            }
        }
        [Test]
        public void TestDownloadFileNoRetryIfNoCredential()
        {
            WebAccess access = new WebAccess(_httpMessageHandlerFactory.Object);

            _httpMessageHandler.SetupSendAsync(HttpMethod.Get, FakeAddress).ThrowsAsync(new WebException("407"));

            bool called = false;

            access.CredentialRequiered += (o, e) => called = true;

            string tempfile = Path.GetRandomFileName();

            try
            {
                Assert.Throws<WebException>(() => access.DownloadFile(FakeAddress, tempfile));
                Assert.That(called, Is.True);
            }
            finally
            {
                if (File.Exists(tempfile))
                {
                    File.Delete(tempfile);
                }
            }
        }
    }
}