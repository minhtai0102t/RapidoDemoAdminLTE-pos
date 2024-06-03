using DemoAdminLTE.Extensions;
using DemoAdminLTE.Resources.Views.RoleViews;
using DemoAdminLTE.Resources.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DemoAdminLTE.Models
{
    public class Role : BaseModel
    {

        [Display(ResourceType = typeof(Titles), Name = "RoleName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(128, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Permissions")]
        public virtual ICollection<Permission> Permissions { get; set; }

        public override string ToString()
        {
            return RoleName;
        }


        [NotMapped]
        private List<int> _selectedPermissions;

        [NotMapped]
        public List<int> SelectedPermissions
        {
            get
            {
                if (_selectedPermissions == null)
                {
                    if (Permissions != null)
                    {
                        _selectedPermissions = Permissions.Select(m => m.Id).ToList();
                    }
                    else
                    {
                        _selectedPermissions = new List<int>();
                    }
                }
                return _selectedPermissions;
            }
            set { _selectedPermissions = value; }
        }

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "Permissions")]
        public MvcTree TreePermissions { get; set; }

        public Role()
        {
            TreePermissions = new MvcTree();
        }


    }
}
