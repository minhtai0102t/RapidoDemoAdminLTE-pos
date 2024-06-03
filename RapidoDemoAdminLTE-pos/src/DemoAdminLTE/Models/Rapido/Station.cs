using DemoAdminLTE.Resources.Views.StationViews;
using DemoAdminLTE.Resources.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace DemoAdminLTE.Models
{
    public class Station : BaseModel
    {
        [Display(ResourceType = typeof(Titles), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(128, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Address")]
        [StringLength(256, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Latitude")]
        [Range(-90.0, 90.0, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Range")]
        [RegularExpression(@"[0-9]*.?[0-9]*", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        public double Latitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Longitude")]
        [Range(-180.0, 180.0, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Range")]
        [RegularExpression(@"[0-9]*.?[0-9]*", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        public double Longitude { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "IsSmsAlertEnable")]
        public bool IsSmsAlertEnable { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "SmsAlertPhoneNumber")]
        [RegularExpression(@"[+]?[0-9]*", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RegularExpression")]
        [DataType(DataType.PhoneNumber)]
        public string SmsAlertPhoneNumber { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Params")]
        [StringLength(256, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "StringLength")]
        public string Params { get; set; }

        [Display(ResourceType = typeof(Titles), Name = "Sensors")]
        public virtual ICollection<Sensor> Sensors { get; set; }

        public virtual ICollection<SampleTime> SampleTimes { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> AllSensors { get; set; }

        [NotMapped]
        private List<int> _selectedSensors;

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "SelectedSensors")]
        public List<int> SelectedSensors
        {
            get
            {
                if (_selectedSensors == null && Sensors != null)
                {
                    _selectedSensors = Sensors.Select(m => m.Id).ToList();
                }
                return _selectedSensors;
            }
            set { _selectedSensors = value; }
        }
    }
}