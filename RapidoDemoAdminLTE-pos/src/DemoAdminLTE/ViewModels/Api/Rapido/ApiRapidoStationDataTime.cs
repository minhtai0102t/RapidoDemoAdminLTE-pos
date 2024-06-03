using System.Collections.Generic;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoStationDataTime
    {
        public int record_id { get; set; }
        public long time { get; set; }
        public IList<ApiRapidoStationDataValue> sensor_value { get; set; }
    }
}