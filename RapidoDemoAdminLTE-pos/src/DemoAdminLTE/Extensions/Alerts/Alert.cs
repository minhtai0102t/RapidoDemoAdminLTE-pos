using System;

namespace DemoAdminLTE.Extensions.Alerts
{
    public class Alert
    {
        public string Message { get; set; }
        public AlertType Type { get; set; }
        public int Timeout { get; set; }
        public string Id { get; set; }
        public string Icon { get; set; }
    }
}
