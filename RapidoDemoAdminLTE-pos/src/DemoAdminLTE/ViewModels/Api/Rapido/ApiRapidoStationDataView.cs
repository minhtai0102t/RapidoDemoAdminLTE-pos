using System;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoStationDataView : ApiRapidoLoginResult
    {
        public int station_id { get; set; }
        public int data_count { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}
