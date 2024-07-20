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
using System.IdentityModel.Tokens;
using System.Linq;
using System.IdentityModel.Claims;

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
                    string token = authCookie.Value;
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                    if (jsonToken != null)
                    {
                        var claims = jsonToken.Claims;
                        var principal = new CustomPrincipal(claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value)
                        {
                            UserId = int.Parse(claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value),
                            FirstName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value,
                            LastName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value,
                            Role = claims.FirstOrDefault(c => c.Type == "role")?.Value,
                            Phone = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone")?.Value,
                            Email = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value,
                            CreationDate = DateTime.UtcNow, // This might be different, depends on your token's claims
                            Permissions = new string[] { } // Add your logic to parse permissions, if they are in the claims
                        };                        //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        //var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

                        //CustomPrincipal principal = new CustomPrincipal(serializeModel.FirstName);

                        //principal.UserId = serializeModel.UserId;
                        //principal.FirstName = serializeModel.FirstName;
                        //principal.LastName = serializeModel.LastName;
                        //principal.Role = serializeModel.RoleName;
                        //principal.Phone = serializeModel.Phone;
                        //principal.Email = serializeModel.Email;
                        //principal.CreationDate = serializeModel.CreationDate;
                        //principal.Permissions = serializeModel.PermissionString.ToArray();

                        HttpContext.Current.User = principal;
                    }
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
