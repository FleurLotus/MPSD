namespace Common.Web
{
    using System.Net;
    using System.Net.Http;

    internal class HttpMessageHandlerFactory : IHttpMessageHandlerFactory
    {
        public HttpMessageHandler Create(ICredentials credentials)
        {
            if (credentials == null)
            {
                return new HttpClientHandler { UseDefaultCredentials = true };
            }

            return new HttpClientHandler { Credentials = credentials };
        }
    }
}