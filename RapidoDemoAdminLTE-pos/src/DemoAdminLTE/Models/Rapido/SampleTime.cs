using DemoAdminLTE.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DemoAdminLTE.Resources.Views.SampleTimeViews;

namespace DemoAdminLTE.Models
{
    public class SampleTime : BaseModel
    {
        public DateTime Time { get; set; }

        [ForeignKey("Station")]
        public int StationId { get; set; }
        public virtual Station Station { get; set; }

        public virtual ICollection<SensorValue> SensorValues { get; set; }

        public SampleTime()
        {
            Time = DateTime.Now;
        }

        [NotMapped]
        public long EpochTime
        {
            get
            {
                return Time.ToEpochTime();
            }

        }
        [NotMapped]
        public string TdHtmlRawValues => this.SensorValues.ToTdHtmlRawValues();

        [NotMapped]
        [Display(Name = "TimeStringDisplay", ResourceType = typeof(Titles))]
        public string TimeStringDisplay => this.Time.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
