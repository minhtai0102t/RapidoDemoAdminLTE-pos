using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DemoAdminLTE.Controller.Helpers
{
    public class ApiHelper : IApiHelper
    {
        private static readonly HttpClient _httpClient;

        static ApiHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ApiHelper(string baseAddress)
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public async Task<string> GetAsync(string endpoint)
        {
            return await SendRequestAsync(HttpMethod.Get, endpoint);
        }

        public async Task<string> PostAsync(string endpoint, string jsonContent)
        {
            return await SendRequestAsync(HttpMethod.Post, endpoint, jsonContent);
        }

        public async Task<string> PutAsync(string endpoint, string jsonContent)
        {
            return await SendRequestAsync(HttpMethod.Put, endpoint, jsonContent);
        }

        public async Task<string> DeleteAsync(string endpoint)
        {
            return await SendRequestAsync(HttpMethod.Delete, endpoint);
        }

        public void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> SendRequestAsync(HttpMethod method, string endpoint, string jsonContent = null)
        {
            try
            {
                using (var request = new HttpRequestMessage(method, endpoint))
                {
                    if (jsonContent != null)
                    {
                        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    }

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException e)
            {
                // Log the exception (e.g., using a logging framework)
                throw new ApplicationException("Request error: " + e.Message, e);
            }
        }

        public void Dispose()
        {
            // Implement IDisposable, but do not dispose _httpClient as it is a singleton and shared across the application.
        }
    }
}