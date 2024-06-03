using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DemoAdminLTE.Controllers
{
    public class PatchController : BaseController
    {
        private readonly DemoContext db = new DemoContext();

        public ActionResult Index()
        {
            return RedirectToBadRequest();
        }

        public ActionResult ValuePermission()
        {

            // check existed
            var per = db.Permissions.FirstOrDefault(o => o.Group == "Value");
            if (per != null)
            {
                return RedirectToNotFound();
            }

            // permissions
            var permissions = new List<Permission> {
                new Permission { Group = "Value"       , Action = "List"},
                new Permission { Group = "Value"       , Action = "Edit"},
                new Permission { Group = "Value"       , Action = "Delete"},
                new Permission { Group = "Value"       , Action = "Create"},
            };
            permissions.ForEach(obj => db.Permissions.Add(obj));
            db.SaveChanges();

            // roles
            var role = db.Roles.FirstOrDefault(o => o.RoleName == "Admin");
            if (role != null)
            {
                permissions.ForEach(obj => role.Permissions.Add(obj));

                db.Entry(role).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToDefault();
            }


            return RedirectToBadRequest();
        }
    }
}
