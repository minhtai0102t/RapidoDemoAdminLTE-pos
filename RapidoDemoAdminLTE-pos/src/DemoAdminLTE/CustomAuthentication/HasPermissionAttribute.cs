using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoAdminLTE.CustomAuthentication
{
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _permission;

        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public HasPermissionAttribute(string group, string action)
        {
            _permission = group + "/" + action;
        }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Comment check hasPermission => pass
            if(CurrentUser!= null)
            {
                return;
            }
            //if (CurrentUser != null && CurrentUser.HasPermission(_permission))
            //{
            //    return;
            //}

            // Handle Unauthorized Request
            RedirectToRouteResult routeData;
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                routeData = new RedirectToRouteResult
                    (new RouteValueDictionary
                    (new
                    {
                        controller = "Account",
                        action = "Login",
                        returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                    }
                    ));
            }
            else
            {

                routeData = new RedirectToRouteResult
                (new RouteValueDictionary
                (new
                {
                    controller = "Error",
                    action = "AccessDenied"
                }
                ));
            }

            filterContext.Result = routeData;
        }
    }
}
