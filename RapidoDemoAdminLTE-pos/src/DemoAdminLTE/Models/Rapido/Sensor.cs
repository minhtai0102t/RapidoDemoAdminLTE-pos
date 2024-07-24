using System;
using System.Collections.Generic;

namespace DemoAdminLTE.Models
{
    public class Sensor : BaseModel
    {
        public int sensor_id { get; set; }
        public string name { get; set; }
        public double? upper_bound { get; set; }
        public double? lower_bound { get; set; }
        public double? differ_bound { get; set; }
        public string unit_symbol { get; set; }
        public bool is_active { get; set; }
        public string param { get; set; }
        public DateTime created_date { get; set; }
        public DateTime updated_date { get; set; }
        public virtual ICollection<Station> Stations { get; set; }
    }
}
