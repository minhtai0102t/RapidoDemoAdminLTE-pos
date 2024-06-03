using System.Collections.Generic;

namespace DemoAdminLTE.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; }
        public bool Status { get; set; } // is active or not
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}