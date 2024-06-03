using System.Collections.Generic;

namespace DemoAdminLTE.Models
{
    public class DeviceType : BaseModel
    {
        public string Name { get; set; }
        public bool Status { get; set; } // is active or not
        public string Description { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

    }
}