using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.Resources.Views.RegistrationViews;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class RegistrationView
    {
        [Display(Name = "Username", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string LastName { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"[+]?[0-9]*", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Titles))]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(Titles))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Compare")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "AgreeTheTerms", ResourceType = typeof(Titles))]
        public bool AgreeTheTerms { get; set; }
    }
}
