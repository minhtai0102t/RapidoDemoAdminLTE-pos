using DemoAdminLTE.Resources.Views.SensorViews;
using DemoAdminLTE.Resources.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.Models
{
    public class Sensor : BaseModel
    {
        [Display(ResourceType = typeof(Titles), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(128, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "UpperBound")]
        public double? UpperBound { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "LowerBound")]
        public double? LowerBound { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "DifferBound")]
        public double? DifferBound { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "UnitSymbol")]
        public string UnitSymbol { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Params")]
        [StringLength(256, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Params { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Stations")]
        public virtual ICollection<Station> Stations { get; set; }

        public virtual ICollection<SensorValue> SensorValues { get; set; }

    }
}
