using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAdminLTE.Models
{
    public class SensorValue : BaseModel
    {
        public double Value { get; set; }

        [ForeignKey("Sensor")]
        public int SensorId { get; set; }
        public virtual Sensor Sensor { get; set; }

        [ForeignKey("SampleTime")]
        public int SampleTimeId { get; set; }
        public virtual SampleTime SampleTime { get; set; }
    }
}