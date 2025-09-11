namespace Common.Web
{
    using System.Net;
    using System.Net.Http;

    public interface IHttpMessageHandlerFactory
    {
        HttpMessageHandler Create(ICredentials credentials);
    }
}