using DemoAdminLTE.Resources.Views.UserViews;
using DemoAdminLTE.Resources.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAdminLTE.Models
{
    public class User : BaseModel
    {

        [Display(ResourceType = typeof(Titles), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Phone")]
        [RegularExpression(@"[+]?[0-9]*", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "ActivationCode")]
        public Guid ActivationCode { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "RoleId")]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Role")]
        public virtual Role Role { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastPasswordChangedDate")]
        public DateTime? LastPasswordChangedDate { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastActivityDate")]
        public DateTime? LastActivityDate { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LastLockoutDate")]
        public DateTime? LastLockoutDate { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "IsLockedOut")]
        public bool IsLockedOut { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "IsApproved")]
        public bool IsApproved { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "DeviceToken")]
        public string Comment { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "Password")]
        public string Password { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "FullName")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}