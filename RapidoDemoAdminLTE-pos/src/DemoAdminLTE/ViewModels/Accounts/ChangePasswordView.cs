using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.Resources.Views.ChangePasswordViews;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class ChangePasswordView
    {
        [Display(ResourceType = typeof(Titles), Name = "OldPassword")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string OldPassword { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "NewPassword")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string NewPassword { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "ConfirmPassword")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Compare")]
        public string ConfirmPassword { get; set; }
    }
}