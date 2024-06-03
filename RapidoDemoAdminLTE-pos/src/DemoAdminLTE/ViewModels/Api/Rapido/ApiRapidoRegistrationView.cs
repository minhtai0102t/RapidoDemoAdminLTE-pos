using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoRegistrationView
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"[+]?[0-9]*")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string DeviceToken { get; set; }
    }
}