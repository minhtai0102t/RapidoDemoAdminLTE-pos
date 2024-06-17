using System.Configuration;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DemoAdminLTE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = "Get", id = RouteParameter.Optional }
            );
            // Config value
            AppConfig.apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            AppConfig.PageSizeDefaultValue = int.Parse(ConfigurationManager.AppSettings["PageSizeDefaultValue"]);
        }
    }
}
