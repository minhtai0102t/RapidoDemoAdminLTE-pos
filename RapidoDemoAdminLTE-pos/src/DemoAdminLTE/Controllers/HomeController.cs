using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using System.Web.Mvc;

namespace DemoAdminLTE.Controllers
{
    public class HomeController : BaseController
    {
        //[HttpGet]
        //[HasPermission("Home/Index")]
        //public ActionResult Index()
        //{
        //    var model = db.Stations;
        //    return View(model);
        //}

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Station");
        }

    }
}
