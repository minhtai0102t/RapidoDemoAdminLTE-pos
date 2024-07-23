using System;

namespace DemoAdminLTE.Helpers
{
    public interface IApiHelper : IDisposable
    {
        T Get<T>(string endpoint);
        T Post<T>(string endpoint, object jsonContent);
        T Put<T>(string endpoint, object jsonContent);
        T Delete<T>(string endpoint);
        void SetAuthorizationHeader(string token);
    }
}
