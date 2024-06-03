using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoLoginView
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string DeviceToken { get; set; }
    }
}