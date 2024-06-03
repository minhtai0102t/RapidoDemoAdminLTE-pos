using DemoAdminLTE.Resources.Views.DeviceViews;
using DemoAdminLTE.Resources.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.Models
{
    public class Device : BaseModel
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

        [Display(ResourceType = typeof(Titles), Name = "SerialNumber")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string SerialNumber { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "MACAddress")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string MACAddress { get; set; } // MAC address

        [Display(ResourceType = typeof(Titles), Name = "DeviceTypeId")]
        [ForeignKey("DeviceType")]
        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Location")]
        [StringLength(256, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Location { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Latitude")]
        public double Latitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Longitude")]
        public double Longitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "HardwareVersion")]
        [RegularExpression(@"[0-9]{1,3}(\.[0-9]{1,3}){0,3}(-[a-zA-Z0-9]{1,32})?", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        public string HardwareVersion { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "SoftwareVersion")]
        [RegularExpression(@"[0-9]{1,3}(\.[0-9]{1,3}){0,3}(-[a-zA-Z0-9]{1,32})?", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        public string SoftwareVersion { get; set; } // same as firmware version

        [Display(ResourceType = typeof(Titles), Name = "Activated")]
        public bool Activated { get; set; } // is activated or not

        [Display(ResourceType = typeof(Titles), Name = "Online")]
        public bool Online { get; set; } // is online or not

        [Display(ResourceType = typeof(Titles), Name = "Password")]
        [StringLength(128, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "PinCode")]
        [StringLength(32, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string PinCode { get; set; } // pin code

        [Display(ResourceType = typeof(Titles), Name = "PinCodeGenerationTime")]
        public DateTime PinCodeGenerationTime { get; set; }

        public virtual ICollection<DeviceProduct> DeviceProducts { get; set; }

    }
}