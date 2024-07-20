using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DemoAdminLTE
{
    public class ApiHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ApiHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ApiHelper(string baseAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public T Get<T>(string endpoint)
        {
            return SendRequest<T>(HttpMethod.Get, endpoint);
        }

        public T Post<T>(string endpoint, object jsonContent)
        {
            return SendRequest<T>(HttpMethod.Post, endpoint, jsonContent);
        }

        public T Put<T>(string endpoint, object jsonContent)
        {
            return SendRequest<T>(HttpMethod.Put, endpoint, jsonContent);
        }

        public T Delete<T>(string endpoint)
        {
            return SendRequest<T>(HttpMethod.Delete, endpoint);
        }

        public void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private T SendRequest<T>(HttpMethod method, string endpoint, object jsonContentObj = null)
        {
            using (var request = new HttpRequestMessage(method, endpoint))
            {
                if (jsonContentObj != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(jsonContentObj), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = _httpClient.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                string responseContent = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
        }

        public void Dispose()
        {
            // Implement IDisposable, but do not dispose _httpClient as it is a singleton and shared across the application.
        }
    }
}