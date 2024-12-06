namespace Common.Web.UnitTests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Threading;

    using Newtonsoft.Json;
    using Moq;
    using Moq.Protected;
    using Moq.Language;
    using Moq.Language.Flow;

    public static class MoqExtensions
    {
        //From https://daninacan.com/how-to-mock-httpclient-in-c-using-moq/
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupSendAsync(this Mock<HttpMessageHandler> handler, HttpMethod requestMethod, string requestUrl)
        {
            return handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == requestMethod &&
                    r.RequestUri != null &&
                    r.RequestUri.ToString() == requestUrl
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
        public static ISetupSequentialResult<Task<HttpResponseMessage>> SetupSequenceSendAsync(this Mock<HttpMessageHandler> handler, HttpMethod requestMethod, string requestUrl)
        {
            return handler.Protected().SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == requestMethod &&
                    r.RequestUri != null &&
                    r.RequestUri.ToString() == requestUrl
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
        public static IReturnsResult<HttpMessageHandler> ReturnsHttpResponseAsync(this ISetup<HttpMessageHandler, Task<HttpResponseMessage>> moqSetup, object responseBody, HttpStatusCode responseCode)
        {
            var serializedResponse = JsonConvert.SerializeObject(responseBody);
            var stringContent = new StringContent(serializedResponse ?? string.Empty);

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = responseCode,
                Content = stringContent
            };

            return moqSetup.ReturnsAsync(responseMessage);
        }
        public static ISetupSequentialResult<Task<HttpResponseMessage>> ReturnsHttpResponseAsync(this ISetupSequentialResult<Task<HttpResponseMessage>> moqSetup, object responseBody, HttpStatusCode responseCode)
        {
            var serializedResponse = JsonConvert.SerializeObject(responseBody);
            var stringContent = new StringContent(serializedResponse ?? string.Empty);

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = responseCode,
                Content = stringContent
            };

            return moqSetup.ReturnsAsync(responseMessage);
        }
    }
}