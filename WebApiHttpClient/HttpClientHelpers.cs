using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiHttpClient
{
    public class HttpClientHelpers
    {
        private readonly Uri _baseUrl;
        private readonly RestClient _restClient;

        public HttpClientHelpers(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
            _restClient = new RestClient(_baseUrl);
        }

        public HttpClientHelpers(string baseUrl, string header, string value)
        {
            _baseUrl = new Uri(baseUrl);
            _restClient = new RestClient(_baseUrl);
            _restClient.AddDefaultHeader(header, value);
        }

        private RestClient GetRestClient(string header = null, string value = null)
        {
            var client = new RestClient();
            client.BaseUrl = _baseUrl;
            client.AddDefaultHeader(header, value);

            return client;
        }

        public void AddRequestHeader(string header = null, string value = null)
        {
            _restClient.AddDefaultHeader(header, value);
        }

        public T Get<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.GET);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return Execute<T>(request);
        }

        public T Post<T>(string url, T param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.POST);

            AddHeader(request, header, headerValue);
            AddJsonBody(request, param);

            return Execute<T>(request);
        }

        public T Put<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.PUT);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return Execute<T>(request);
        }

        public T Patch<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.PATCH);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return Execute<T>(request);
        }

        public T Delete<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.DELETE);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return Execute<T>(request);
        }

        private RestRequest AddHeader(RestRequest request, string header, string value)
        {
            if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(value)) request.AddHeader(header, value);

            return request;
        }

        private RestRequest AddParam(RestRequest request, object param)
        {
            if (param != null) request.AddObject(param);

            return request;
        }

        private RestRequest AddJsonBody(RestRequest request, object param)
        {
            if (param != null) request.AddJsonBody(param);

            return request;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = _baseUrl;

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var appException = new ApplicationException(message, response.ErrorException);
                throw appException;
            }
            return response.Data;
        }

        #region Async        

        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            T result = default(T);
            var response = await _restClient.ExecuteTaskAsync<T>(request);
            result = JsonConvert.DeserializeObject<T>(response.Content);
            return result;
        }

        public async Task<T> GetAsync<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.GET);

            request.AddHeader(header, headerValue);
            AddParam(request, param);

            return await ExecuteAsync<T>(request);
        }

        public async Task<T> PostAsync<T>(string url, T param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.POST);

            AddHeader(request, header, headerValue);
            AddJsonBody(request, param);

            return await ExecuteAsync<T>(request);
        }

        public async Task<T> PutAsync<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.PUT);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return await ExecuteAsync<T>(request);
        }

        public async Task<T> PatchAsync<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.PATCH);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return await ExecuteAsync<T>(request);
        }

        public async Task<T> DeleteAsync<T>(string url, object param, string header = null, string headerValue = null) where T : new()
        {
            var request = new RestRequest(url, Method.DELETE);

            AddHeader(request, header, headerValue);
            AddParam(request, param);

            return await ExecuteAsync<T>(request);
        }

        #endregion

    }
}
