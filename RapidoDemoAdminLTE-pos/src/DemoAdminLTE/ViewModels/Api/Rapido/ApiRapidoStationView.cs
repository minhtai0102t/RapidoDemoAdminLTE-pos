using System.Collections.Generic;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoStationView
    {
        public int station_id { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public IList<ApiRapidoStationSensor> sensors { get; set; }

    }
}