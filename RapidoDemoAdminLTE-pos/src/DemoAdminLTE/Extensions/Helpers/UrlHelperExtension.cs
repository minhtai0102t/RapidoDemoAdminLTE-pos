// Decompiled with JetBrains decompiler
// Type: DemoAdminLTE.Extensions.Helpers.UrlHelperExtension
// Assembly: DemoAdminLTE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 771896B4-5B09-4A8A-B2F6-33E1EA9470A2
// Assembly location: C:\Users\thanh\Downloads\webserver_backup\hst6.win.npnlab.com_103.28.39.47\rapido.npnlab.com\bin\DemoAdminLTE.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DemoAdminLTE.Extensions.Helpers
{
    public static class UrlHelperExtension
    {
        public static string IsTreeActive(
          this UrlHelper url,
          List<KeyValuePair<string, string>> actions)
        {
            try
            {
                string controller = url.RequestContext.RouteData.Values["controller"].ToString();
                string action = url.RequestContext.RouteData.Values["action"].ToString();
                if (actions.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(a => a.Key.Equals(action, StringComparison.OrdinalIgnoreCase) && a.Value.Equals(controller, StringComparison.OrdinalIgnoreCase))))
                    return "active";
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static string IsLinkActiveController(this UrlHelper url, List<string> controllers)
        {
            try
            {
                string controller = url.RequestContext.RouteData.Values["controller"].ToString();
                if (controllers.Any<string>((Func<string, bool>)(a => a.Equals(controller, StringComparison.OrdinalIgnoreCase))))
                    return "active";
            }
            catch (Exception)
            {
            }
            return "";
        }
    }
}
