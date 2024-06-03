using System.Web.Mvc;

namespace DemoAdminLTE.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Error");
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
