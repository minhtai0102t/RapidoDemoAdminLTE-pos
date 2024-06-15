
using System.Threading.Tasks;
namespace DemoAdminLTE.Controller.Helpers
{
    public interface IApiHelper : IDisposable
    {
        Task<string> GetAsync(string endpoint);
        Task<string> PostAsync(string endpoint, string jsonContent);
        Task<string> PutAsync(string endpoint, string jsonContent);
        Task<string> DeleteAsync(string endpoint);
        void SetAuthorizationHeader(string token);
    }
}
