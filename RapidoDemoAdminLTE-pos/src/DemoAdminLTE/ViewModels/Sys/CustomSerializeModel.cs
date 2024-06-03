using System;
using System.Collections.Generic;

namespace DemoAdminLTE.ViewModels
{
    public class CustomSerializeModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }

        public List<string> PermissionString { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
