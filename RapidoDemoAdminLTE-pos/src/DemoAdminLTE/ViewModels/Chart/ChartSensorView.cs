using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoAdminLTE.ViewModels
{
    public class ChartSensorView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public string ColorBorder { get; set; }

        public string ColorFill { get; set; }

        public string Data { get; set; }

        public string TooltipLabel { get; set; }

        public string TooltipTitle { get; set; }

    }
}