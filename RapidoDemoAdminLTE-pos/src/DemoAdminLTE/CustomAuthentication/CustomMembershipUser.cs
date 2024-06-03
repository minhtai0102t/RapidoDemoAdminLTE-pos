using DemoAdminLTE.Models;
using System;
using System.Collections.Generic;
using System.Web.Security;

namespace DemoAdminLTE.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties  

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }

        public ICollection<Permission> Permissions { get; set; }
        public string Phone { get; set; }

        #endregion

        public CustomMembershipUser(User user) : base("CustomMembership", user.Username, user.Id, user.Email, user.ActivationCode.ToString(), user.Comment, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate ?? DateTime.Now, user.LastActivityDate ?? DateTime.Now, user.LastPasswordChangedDate ?? DateTime.Now, user.LastLockoutDate ?? DateTime.Now)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
            Phone = user.Phone;
            Permissions = Role.Permissions;
        }
    }
}
