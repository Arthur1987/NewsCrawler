using Newtonsoft.Json;
using RestApi.Net.Core.Constants;
using RestApi.Net.Core.Enums;
using RestApi.Net.Core.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestApi.Net.Core.Http
{
    /// <summary>
    /// HTTP client helper
    /// Wrapped HTTP client methods
    /// </summary>
    public class RestApiClient : IDisposable
    {
        #region Private Fields

        // Flag: Has Dispose already been called?
        private bool _disposed;
        private HttpClient _client;
        private MediaType _contenType;
        private MediaType _acceptType;
        private JsonSerializerSettings _jsonSerializerSettings;
        private JsonSerializerSettings _jsonDeSerializerSettings;

        #endregion

        #region Constructor/ Destructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="baseAddress">base Address</param>
        /// <param name="proxy">proxy</param>
        /// <param name="contenType">Media type of content</param>
        /// <param name="acceptType">Media type of accept</param>
        public RestApiClient(string baseAddress, MediaType contenType, MediaType acceptType, IWebProxy proxy = null)
        {
            
            _contenType = contenType;
            _acceptType = acceptType;
            _client = new HttpClient(GetHttpClientHandler(proxy), true) { BaseAddress = new UriBuilder(baseAddress).Uri };
            AddDefaultHeaders();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="baseAddress">base Address</param>
        /// <param name="proxy">proxy</param>
        public RestApiClient(string baseAddress, IWebProxy proxy = null) 
            : this(baseAddress, MediaType.Json, MediaType.Json, proxy)
        {
            
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~RestApiClient()
        {
            Dispose(false);
        }

        #endregion

        #region HTTP methods

        /// <summary>
        /// Send HTTP post request
        /// </summary>
        /// <typeparam name="TRequestModel">request Model</typeparam>
        /// <typeparam name="TResponseModel">response tModel</typeparam>
        ///<param name="requestUri">requestUri to send post request</param>
        /// <param name="requestModel">request Model</param>
        /// <returns></returns>
        public async Task<TResponseModel> PostAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel requestModel) where TResponseModel : class
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            HttpResponseMessage response;
            using (var byteContent = GetByteArrayContent(requestModel))
            {
                response = await _client.PostAsync(requestUri, byteContent).ConfigureAwait(false);
            }

            return await ResponseModel<TResponseModel>(response);
        }

        /// <summary>
        /// Send HTTP post request
        /// </summary>
        /// <typeparam name="TResponseModel">response tModel</typeparam>
        ///<param name="requestUri">requestUri to send post request</param>
        /// <returns>Response Model</returns>
        public async Task<TResponseModel> PostAsync<TResponseModel>(string requestUri) where TResponseModel : class
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            HttpResponseMessage response = await _client.PostAsync(requestUri, null).ConfigureAwait(false);

            return await ResponseModel<TResponseModel>(response);
        }

        /// <summary>
        /// Send HTTP put request
        /// </summary>
        /// <typeparam name="TRequestModel">request Model</typeparam>
        /// <param name="requestUri">requestUri to send put request</param>
        /// <param name="requestModel">model</param>
        /// <returns></returns>
        public async Task PutAsync<TRequestModel>(string requestUri, TRequestModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            HttpResponseMessage response;
            using (var byteContent = GetByteArrayContent(requestModel))
            {
                response = await _client.PutAsync(requestUri, byteContent).ConfigureAwait(false);
            }
            response.EnsureSuccessResponseStatusCode();
        }

        /// <summary>
        /// Send HTTP put request with response
        /// </summary>
        /// <typeparam name="TResponseModel">Response Model</typeparam>
        /// <param name="requestUri">requestUri to send put request</param>
        /// <returns>TResponseModel</returns>
        public async Task<TResponseModel> PutAsync<TResponseModel>(string requestUri) where TResponseModel : class
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            HttpResponseMessage response = await _client.PutAsync(requestUri, null).ConfigureAwait(false);

            return await ResponseModel<TResponseModel>(response);

        }

        /// <summary>
        /// Send HTTp put request
        /// </summary>
        /// <param name="requestUri">requestUri to send put request</param>
        /// <returns></returns>
        public async Task PutAsync(string requestUri)
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            using (var response = await _client.PutAsync(requestUri, null).ConfigureAwait(false))
            {
                response.EnsureSuccessResponseStatusCode();
            }
        }

        /// <summary>
        /// Send HTTP put request with response
        /// </summary>
        /// <typeparam name="TRequestModel"></typeparam>
        /// <typeparam name="TResponseModel"></typeparam>
        /// <param name="requestUri">requestUri to send put request</param>
        /// <param name="requestModel">request Model</param>
        /// <returns></returns>
        public async Task<TResponseModel> PutAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel requestModel) where TResponseModel : class
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            HttpResponseMessage response;
            using (var byteContent = GetByteArrayContent(requestModel))
            {
                response = await _client.PutAsync(requestUri, byteContent).ConfigureAwait(false);
            }

            return await ResponseModel<TResponseModel>(response);
        }

        /// <summary>
        /// Send HTTP Get request
        /// </summary>
        /// <typeparam name="TResponseModel">Response model</typeparam>
        /// <param name="requestUri">requestUri to send get request</param>
        /// <returns></returns>
        public async Task<TResponseModel> GetAsync<TResponseModel>(string requestUri)
            where TResponseModel : class
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            var response = await _client.GetAsync(requestUri).ConfigureAwait(false);

            return await ResponseModel<TResponseModel>(response);
        }

        /// <summary>
        /// Returns HTTP Response Content as a string
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<string> GetContentAsStringAsync(string requestUri)
        {
            HttpResponseMessage response = null;
            try
            {
                using (response = await _client.GetAsync(requestUri))
                {
                    if (response.StatusCode == HttpStatusCode.Moved)
                    {
                        return await GetContentAsStringAsync(response.RequestMessage.RequestUri.LocalPath);
                    }

                    using (var content = response.Content)
                    {
                        var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false);
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var streamReader = new StreamReader(decompressedStream))
                        {
                            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                response?.Content?.Dispose();
                response?.Dispose();
            }
        }


        ///<summary>Send a GET request to the specified Uri and return the response body as a byte</summary>
        /// <param name="requestUri"> The Uri the request is sent to.</param>
        /// <returns>byte array</returns>
        public async Task<byte[]> GetByteArrayAsync(string requestUri)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            return await _client.GetByteArrayAsync(new Uri(requestUri)).ConfigureAwait(false);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set JSON DeSerializer Setting
        /// </summary>
        /// <param name="deSerializerSettings">deserializer Settings</param>
        public void SetJsonDeSerializerSettings(JsonSerializerSettings deSerializerSettings)
        {
            _jsonDeSerializerSettings = deSerializerSettings;
        }

        /// <summary>
        /// Set JSON Serializer Setting
        /// </summary>
        /// <param name="serializerSettings">Specializer Settings</param>
        public void SetJsonSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            _jsonSerializerSettings = serializerSettings;
        }

        /// <summary>
        /// Returns instance of IBaseHttpClientHelper and set custom header
        /// </summary>
        /// <param name="name">custom header name</param>
        /// <param name="value">custom header value</param>
        /// <returns>IBaseHttpClientHelper</returns>
        public RestApiClient SetCustomHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (_client.DefaultRequestHeaders.Contains(name))
            {
                _client.DefaultRequestHeaders.Remove(name);
            }

            _client.DefaultRequestHeaders.Add(name, value);

            return this;
        }

        /// <summary>
        /// Set Access token in HTTP header
        /// </summary>
        /// <param name="accessToken">access Token</param>
        public RestApiClient SetAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);

            return this;
        }

        /// <summary>
        /// Set Content Type header
        /// </summary>
        /// <param name="value"></param>
        public void SetContenTypeHeader(MediaType value)
        {
            _contenType = value;
        }

        /// <summary>
        /// Set Accept header 
        /// </summary>
        /// <param name="value"></param>
        public void SetAcceptTypeHeader(MediaType value)
        {
            _acceptType = value;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(GetApplicationHeaderValue(value)));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                _client.Dispose();
            }

            _disposed = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get byte array Content from request model
        /// </summary>
        /// <typeparam name="TRequestModel"></typeparam>
        /// <param name="requestModel"></param>
        /// <returns>Returns Byte Array Content</returns>
        private ByteArrayContent GetByteArrayContent<TRequestModel>(TRequestModel requestModel)
        {
            // we don't need to serialize string
            string content = requestModel as string;
            if (requestModel != null && content != null)
            {
                return GetByteArrayContent(content);
            }

            switch (_contenType)
            {
                case MediaType.Json:
                    content = JsonConvert.SerializeObject(requestModel, _jsonSerializerSettings);
                    break;
                case MediaType.Xml:
                    content = requestModel.XmlSerializeToString();
                    break;
                default:
                    throw new NotSupportedException($"Content Type '{_contenType}' is not supported.");
            }

            var buffer = Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(GetApplicationHeaderValue(_contenType));

            return byteContent;
        }

        /// <summary>
        /// Get byte array Content from request model
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>Returns Byte Array Content</returns>
        private ByteArrayContent GetByteArrayContent(string requestModel)
        {
            var buffer = Encoding.UTF8.GetBytes(requestModel);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(GetApplicationHeaderValue(_contenType));

            return byteContent;
        }

        /// <summary>
        /// Add Default header
        /// </summary>
        private void AddDefaultHeaders(bool addDefaultHeader = false)
        {
            if (addDefaultHeader)
            {
                ClearHeader();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(GetApplicationHeaderValue(_acceptType)));
            }
        }

        /// <summary>
        /// Returns HttpClientHandler
        /// </summary>
        /// <param name="proxy">Web Proxy</param>
        /// <returns>HttpClientHandler</returns>
        private HttpClientHandler GetHttpClientHandler(IWebProxy proxy)
        {
            var useDefault = proxy == null;
            // Now create a client handler which uses that proxy
            return new HttpClientHandler
            {
                Proxy = proxy ?? WebRequest.DefaultWebProxy,
                UseProxy = true,
                UseDefaultCredentials = useDefault,
                PreAuthenticate = true,
                AllowAutoRedirect = false
            };
        }

        /// <summary>
        /// Returns TResponseModel De serialize from response
        /// </summary>
        /// <typeparam name="TResponseModel"></typeparam>
        /// <param name="response">HttpResponseMessage</param>
        /// <returns>TResponseModel</returns>
        private async Task<TResponseModel> ResponseModel<TResponseModel>(HttpResponseMessage response)
            where TResponseModel : class
        {

            var responseContent = await ReadAsStringAsync(response);
            if (typeof(TResponseModel) == typeof(string))
            {
                //If we want the raw result, then do not deserialize 
                return responseContent as TResponseModel;
            }

            switch (_contenType)
            {
                case MediaType.Json:
                    return JsonConvert.DeserializeObject<TResponseModel>(responseContent, _jsonDeSerializerSettings);
                case MediaType.Xml:
                    return responseContent.XmlDeserializeFromString<TResponseModel>();
                default:
                    return JsonConvert.DeserializeObject<TResponseModel>(responseContent, _jsonDeSerializerSettings);
            }
        }

        /// <summary>
        /// ReadAsStringAsync
        /// </summary>
        /// <param name="response">HTTP Response Message</param>
        /// <returns>string content</returns>
        private async Task<string> ReadAsStringAsync(HttpResponseMessage response)
        {
            string responseContent;
            try
            {
                response.EnsureSuccessResponseStatusCode();
                responseContent = await response.Content.ReadAsStringAsync();
            }
            finally
            {
                response.Content.Dispose();
                response.Dispose();
            }

            return responseContent;
        }

        /// <summary>
        /// returns media type Application name
        /// </summary>
        /// <param name="mediaType">mediaType</param>
        /// <returns></returns>
        private string GetApplicationHeaderValue(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Json:
                    return ConfigurationConstants.ApplicationJson;
                case MediaType.Xml:
                    return ConfigurationConstants.ApplicationXml;
                default:
                    throw new NotSupportedException(nameof(mediaType));
            }
        }

        /// <summary>
        /// Clear header
        /// </summary>
        private void ClearHeader()
        {
            _client?.DefaultRequestHeaders.Clear();
            _client?.DefaultRequestHeaders.Accept.Clear();
            _client?.DefaultRequestHeaders.UserAgent.Clear();
        }

        #endregion
    }
}
