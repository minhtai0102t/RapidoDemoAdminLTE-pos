using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoAdminLTE.Controllers
{
    public class ActivityLogController : BaseController
    {
        private readonly DemoContext db = new DemoContext();

        [HasPermission("ActivityLog/List")]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.ActivityLogs.Count();
            return View();
        }

        [HttpGet]
        [HasPermission("ActivityLog/List")]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<ActivityLog> model = db.ActivityLogs;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o => 
                    o.Action.Contains(search)
                    || o.Context.Contains(search)
                    || o.Message.Contains(search)
                    || o.Username.Contains(search)
                );
            }

            return PartialView(model);
        }
    }
}