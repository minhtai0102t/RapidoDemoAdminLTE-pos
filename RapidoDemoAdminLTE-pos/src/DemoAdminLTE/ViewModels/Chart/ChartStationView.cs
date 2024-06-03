using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoAdminLTE.ViewModels
{
    public class ChartStationView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ChartLabel { get; set; }

        public int LastedSampleTimeId { get; set; }

        public virtual ICollection<ChartSensorView> Sensors { get; set; }

    }
}