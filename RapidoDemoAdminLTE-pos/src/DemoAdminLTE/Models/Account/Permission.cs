using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.Models
{
    public class Permission : BaseModel
    {
        [StringLength(32)]
        public string Group { get; set; }
        [StringLength(32)]
        public string Action { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public override string ToString()
        {
            return Group + "/" + Action;
        }

    }
}