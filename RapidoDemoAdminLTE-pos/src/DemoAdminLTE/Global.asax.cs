using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DemoAdminLTE.App_Start;
using DemoAdminLTE.Controllers;
using NLog;
using System.Web.Security;
using System.Security.Cryptography;
using Newtonsoft.Json;
using DemoAdminLTE.ViewModels;
using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.Extensions;
using System.Web.Http;
using DemoAdminLTE.Utils;

namespace DemoAdminLTE
{
    public class MvcApplication : HttpApplication
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            Log.Info("Starting up...");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new LanguageViewEngine());

            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Messages";
            DefaultModelBinder.ResourceClassKey = "Messages";

            Log.Info("Routes and bundles registered");
            Log.Info("Started");
        }

        protected void Application_End()
        {
            Log.Info("Stopped");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Log.Error(exception, "Unhandled application exception");
            Server.ClearError();
            if (exception is CryptographicException)
            {
                FormsAuthentication.SignOut();
                return;
            }

            var httpContext = ((HttpApplication)sender).Context;
            httpContext.Response.Clear();
            httpContext.ClearError();

            if (new HttpRequestWrapper(httpContext.Request).IsAjaxRequest())
            {
                return;
            }

            ExecuteErrorController(httpContext, exception as HttpException);
        }

        private void ExecuteErrorController(HttpContext httpContext, HttpException exception)
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";

            if (exception != null && exception.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                routeData.Values["action"] = "NotFound";
            }
            else
            {
                routeData.Values["action"] = "InternalServerError";
            }

            using (Controller controller = new ErrorController())
            {
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {
                HttpCookie authCookie = Request.Cookies[CONST.COOKIE_AUTHENTICATION];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

                    CustomPrincipal principal = new CustomPrincipal(authTicket.Name);

                    principal.UserId = serializeModel.UserId;
                    principal.FirstName = serializeModel.FirstName;
                    principal.LastName = serializeModel.LastName;
                    principal.Role = serializeModel.RoleName;
                    principal.Phone = serializeModel.Phone;
                    principal.Email = serializeModel.Email;
                    principal.CreationDate = serializeModel.CreationDate;
                    principal.Permissions = serializeModel.PermissionString.ToArray();

                    HttpContext.Current.User = principal;
                }
            }
            catch (CryptographicException)
            {
                HttpCookie cookie = new HttpCookie(CONST.COOKIE_AUTHENTICATION, "");
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
                FormsAuthentication.SignOut();
            }
            catch
            {
                FormsAuthentication.SignOut();
            }
        }

    }
}
