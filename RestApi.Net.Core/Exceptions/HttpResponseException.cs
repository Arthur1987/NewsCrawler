using System;
using System.Net;

namespace RestApi.Net.Core.Exceptions
{
    /// <summary>
    /// Represent HTTP response exception
    /// </summary>
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// HTTP Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Constructor of HttpResponseException
        /// </summary>
        /// <param name="statusCode">HTTP Status Code</param>
        /// <param name="content">content of error message</param>
        public HttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}
