using System.Web.Mvc;
using System.Web.Routing;

namespace DemoAdminLTE
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "LocalizedDefault",
                url: "{lg}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", lg = "vi", id = UrlParameter.Optional },
                constraints: new { lg = "vi|en" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
