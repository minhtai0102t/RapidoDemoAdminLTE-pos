using DemoAdminLTE.Models;
using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.Resources.Views.ProfileViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoAdminLTE.ViewModels
{
    public class ProfileView
    {
        [Display(ResourceType = typeof(Titles), Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Phone")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Comment")]
        public string Comment { get; set; }

        public ProfileView(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Phone = user.Phone;
            Email = user.Email;
            Comment = user.Comment;
        }

        public ProfileView()
        {

        }
    }
}