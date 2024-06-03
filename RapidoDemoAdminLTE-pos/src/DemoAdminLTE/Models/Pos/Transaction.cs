using DemoAdminLTE.Resources.Views.TransactionViews;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace DemoAdminLTE.Models
{
    public class Transaction : BaseModel
    {
        [Display(ResourceType = typeof(Titles), Name = "DeviceId")]
        [ForeignKey("Device")]
        public int DeviceId { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device")]
        public virtual Device Device { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "ProductId")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Product")]
        public virtual Product Product { get; set; }


        /* transaction infos */
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Titles), Name = "Quantity")]
        public long Quantity { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Note")]
        public string Note { get; set; }


        /* back up device information at the time the transaction is created */
        [Display(ResourceType = typeof(Titles), Name = "Device_Name")]
        public string Device_Name { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_SerialNumber")]
        public string Device_SerialNumber { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_MACAddress")]
        public string Device_MACAddress { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_TypeName")]
        public string Device_TypeName { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_Location")]
        public string Device_Location { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_Latitude")]
        public double Device_Latitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_Longitude")]
        public double Device_Longitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_HardwareVersion")]
        public string Device_HardwareVersion { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Device_SoftwareVersion")]
        public string Device_SoftwareVersion { get; set; }

        //public bool Status { get; set; }
        //public string Description { get; set; }
        //public bool Activated { get; set; } 
        //public bool Online { get; set; } 
        //public string Password { get; set; }
        //public string PinCode { get; set; } 
        //public DateTime PinCodeGenerationTime { get; set; }


        /* back up product information at the time the transaction is created */
        [Display(ResourceType = typeof(Titles), Name = "Product_Name")]
        public string Product_Name { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Product_SKU")]
        public string Product_SKU { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Product_CategoryName")]
        public string Product_CategoryName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Titles), Name = "Product_UnitPrice")]
        public float Product_UnitPrice { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Product_CurrencyUnit")]
        public string Product_CurrencyUnit { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Product_UnitType")]
        public string Product_UnitType { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Titles), Name = "Product_InventoryBeforeTransaction")]
        public long Product_InventoryBeforeTransaction { get; set; }

        //public bool Status { get; set; }
        //public string Description { get; set; }
        //public long LowQuantityThreshold { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Titles), Name = "Product_TotalPrice")]
        public float Product_TotalPrice
        {
            get
            {
                return Product_UnitPrice * Quantity;
            }
        }
    }
}