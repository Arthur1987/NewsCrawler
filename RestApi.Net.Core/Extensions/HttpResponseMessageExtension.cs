using System.Net.Http;
using RestApi.Net.Core.Exceptions;

namespace RestApi.Net.Core.Extensions
{
    /// <summary>
    /// Represent HttpResponse Message Extension
    /// </summary>
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Ensure that HTTP response is success otherwise throw exceptionResponseStatusCode
        /// </summary>
        /// <param name="response">HTTP Response Message</param>
        public static void EnsureSuccessResponseStatusCode(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.Content?.Dispose();

            throw new HttpResponseException(response.StatusCode, content);
        }
    }
}
