namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoSensorView
    {
        public int sensor_id { get; set; }

        public string name { get; set; }

        public double upper_bound { get; set; }

        public double lower_bound { get; set; }

        public double differ_bound { get; set; }

        public string unit_symbol { get; set; }

        public bool is_active { get; set; }

    }
}