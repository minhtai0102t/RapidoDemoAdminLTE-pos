using System.Collections.Generic;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoStationPushView
    {
        public int station_id { get; set; }
        public IList<ApiRapidoStationPushSensor> Sensors { get; set; }
    }
}