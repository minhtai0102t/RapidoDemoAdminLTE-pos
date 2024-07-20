using DemoAdminLTE.Utils;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.Extensions.Alerts;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoAdminLTE.Controllers
{
    public abstract class BaseController : Controller
    {
        public AlertsContainer Alerts { get; protected set; }

        protected BaseController()
        {
            Alerts = new AlertsContainer();
        }

        protected override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is JsonResult))
            {
                AlertsContainer current = TempData["Alerts"] as AlertsContainer;
                if (current == null)
                    TempData["Alerts"] = Alerts;
                else
                    current.Merge(Alerts);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string cultureOnCookie = GetCultureOnCookie(filterContext.HttpContext.Request);
            string cultureOnURL = filterContext.RouteData.Values.ContainsKey("lg")
                ? filterContext.RouteData.Values["lg"].ToString()
                : LanguageHelper.DefaultLanguageCode;
            string culture = (cultureOnCookie == string.Empty)
                ? cultureOnURL
                : cultureOnCookie;

            if (cultureOnURL != culture)
            {

                var routeValueDictionary =
                    new RouteValueDictionary(
                        new
                        {
                            lg = culture,
                            controller = filterContext.RouteData.Values["controller"],
                            action = filterContext.RouteData.Values["action"],
                            id = filterContext.RouteData.Values["id"],
                        });

                var queryString = filterContext.HttpContext.Request.QueryString;

                foreach (var key in queryString.AllKeys)
                {
                    routeValueDictionary.Add(key, queryString[key]);
                }

                filterContext.HttpContext.Response.RedirectToRoute("LocalizedDefault", routeValueDictionary);

                return;
            }

            SetCurrentCultureOnThread(culture);
            if (culture != LanguageViewEngine.CurrentCulture)
            {
                (ViewEngines.Engines[0] as LanguageViewEngine).SetCurrentCulture(culture);
            }
            base.OnActionExecuting(filterContext);
        }

        private static void SetCurrentCultureOnThread(string lg)
        {
            if (string.IsNullOrEmpty(lg))
                lg = LanguageHelper.DefaultLanguageCode;
            var cultureInfo = new CultureInfo(lg);
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }

        public static string GetCultureOnCookie(HttpRequestBase request)
        {
            var cookie = request.Cookies[CONST.COOKIE_LANGUAGE_NAME];
            string culture = string.Empty;
            if (cookie != null)
            {
                culture = cookie.Value;
            }
            return culture;
        }

        public virtual RedirectToRouteResult RedirectToDefault()
        {
            return RedirectToAction("Index", "Home");
        }

        public virtual RedirectToRouteResult RedirectToBadRequest()
        {
            return RedirectToAction("BadRequest", "Error");
        }

        public virtual RedirectToRouteResult RedirectToAccessDenied()
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        public virtual RedirectToRouteResult RedirectToNotFound()
        {
            return RedirectToAction("NotFound", "Error");
        }
    }
}
