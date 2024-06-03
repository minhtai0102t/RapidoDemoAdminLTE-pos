using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.Resources.Views.LoginViews;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class LoginView
    {
        [Display(Name = "Username", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(Name = "Password", ResourceType = typeof(Titles))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Titles))]
        public bool RememberMe { get; set; }
    }
}
