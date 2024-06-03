using DemoAdminLTE.Resources;
using System.Resources;

namespace DemoAdminLTE.Extensions
{
    public static class ResourceExtension
    {
        public static string GetPermissionResource(this string name)
        {
            try
            {
                var res = new ResourceManager(typeof(Permissions));
                return res.GetString(name);
            }
            catch
            {
                return name;
            }
        }
    }
}
