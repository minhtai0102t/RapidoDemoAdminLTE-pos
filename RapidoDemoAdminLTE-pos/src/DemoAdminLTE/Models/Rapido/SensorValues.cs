using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAdminLTE.Models
{
    //public class SensorValues : BaseModel
    //{
    //    public double Value { get; set; }

    //    [ForeignKey(Sensor)]
    //    public int SensorId { get; set; }
    //    public virtual Sensor Sensor { get; set; }

    //    [ForeignKey(SampleTime)]
    //    public int SampleTimeId { get; set; }
    //    public virtual SampleTime SampleTime { get; set; }

    //}
    public class SensorValues
    {
        public int value_time_id { get; set; }
        public int station_id { get; set; }
        public string station_name { get; set; }
        public int time_epoch { get; set; }
        public DateTime time { get; set; }
        public List<SensorValue> sensor_values {get;set;}

    }
    public class SensorValue
    {
        public int sensor_value_id { get; set; }
        public int sensor_id { get; set; }
        public string sensor_name { get; set; }
        public int value { get; set; }
    }
}