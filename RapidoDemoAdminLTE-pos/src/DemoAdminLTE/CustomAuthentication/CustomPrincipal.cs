using System;
using System.Linq;
using System.Security.Principal;

namespace DemoAdminLTE.CustomAuthentication
{
    public class CustomPrincipal : IPrincipal
    {
        #region Identity Properties  

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string[] Permissions { get; set; }
        public string Phone { get; set; }
        public DateTime CreationDate { get; set; }
        #endregion

        public IIdentity Identity
        {
            get; private set;
        }

        public bool IsInRole(string role)
        {
            if (role.Contains(Role))
            {
                return true;
            }
            return false;
        }

        public bool HasPermission(string permission)
        {
            if (Permissions.Any(r => permission.Contains(r)))
            {
                return true;
            }
            return false;
        }

        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }
    }
}