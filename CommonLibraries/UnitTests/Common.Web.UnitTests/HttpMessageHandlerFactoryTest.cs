namespace Common.Web.UnitTests
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;

    [TestFixture]
    public class HttpMessageHandlerFactoryTest
    {
        [Test]
        public void TestDefault()
        {
            HttpMessageHandler handler = new HttpMessageHandlerFactory().Create(null);
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler, Is.AssignableFrom<HttpClientHandler>());
            HttpClientHandler clientHandler = handler as HttpClientHandler;
            Assert.That(clientHandler.UseDefaultCredentials, Is.True);
        }

        [Test]
        public void TestCredentials()
        {
            ICredentials credentials = new NetworkCredential { UserName = "User", Password = "Password" };

            HttpMessageHandler handler = new HttpMessageHandlerFactory().Create(credentials);
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler, Is.AssignableFrom<HttpClientHandler>());
            HttpClientHandler clientHandler = handler as HttpClientHandler;
            Assert.That(clientHandler.UseDefaultCredentials, Is.False);
            Assert.That(clientHandler.Credentials, Is.EqualTo(credentials));
        }
    }
}