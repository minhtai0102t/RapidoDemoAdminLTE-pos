using DemoAdminLTE.Resources.Views.ProductViews;
using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.Models
{
    public class Product : BaseModel
    {
        [Display(ResourceType = typeof(Titles), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(128, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Status")]
        public bool Status { get; set; } // is active or not

        [Display(ResourceType = typeof(Titles), Name = "Description")]
        [StringLength(1024, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "SKU")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string SKU { get; set; }


        [Display(ResourceType = typeof(Titles), Name = "CategoryId")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "UnitPrice")]
        public float UnitPrice { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "CurrencyUnit")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string CurrencyUnit { get; set; } // VNĐ, USD

        [Display(ResourceType = typeof(Titles), Name = "UnitType")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string UnitType { get; set; } // Cái, Hộp, Thùng

        [Display(ResourceType = typeof(Titles), Name = "LowQuantityThreshold")]
        public long LowQuantityThreshold { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "DeviceProducts")]
        public virtual ICollection<DeviceProduct> DeviceProducts { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "QuantityInStock")]
        public long QuantityInStock
        {
            get
            {
                var count = 0L;
                using (DemoContext dbContext = new DemoContext())
                {
                    var deviceProducts = dbContext.DeviceProducts.Where(o => o.ProductId == Id);
                    foreach (var product in deviceProducts)
                    {
                        count += product.Quantity;
                    }
                }
                return count;
            }
        }
    }
}