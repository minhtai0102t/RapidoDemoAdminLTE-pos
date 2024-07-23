using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.Helpers;
using DemoAdminLTE.Models;
using DemoAdminLTE.Resources.Views.UserViews;
using NLog;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DemoAdminLTE.Controllers
{
    public class UserController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IApiHelper apiHelper;
        public UserController()
        {
            apiHelper = new ApiHelper(AppConfig.apiUrl);
        }
        // GET: Users
        [HasPermission("User/List")]
        [HttpGet]
        public ActionResult Index()
        {
            //var req = new UserSearchReq
            //{
            //    keysearch = username
            //};
            //var users = apiHelper.Post<PagingResponse<UserSearchRes>>("api/Users/Search", jsonContent: req);
            ViewBag.DataTotal = db.Users.Count();
            return View();
        }

        [HasPermission("User/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search)
        {
            var req = new UserSearchReq
            {
                keysearch = search
            };
            var users = apiHelper.Post<PagingResponse<UserSearchRes>>("api/Users/Search", jsonContent: req);
            return PartialView(users);
        }

        // GET: Users/Create
        [HasPermission("User/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            var role = apiHelper.Get<RoleRes>($"api/roles/2");
            ViewBag.RoleId = role;
            //ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", 2);
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("User/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,FirstName,LastName,Phone,Email,Password,IsApproved,Comment,RoleId")] User user)
        {
            if (user == null)
            {
                return RedirectToBadRequest();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", Messages.PasswordRequired);
                }
                else if (string.IsNullOrEmpty(user.Phone))
                {
                    ModelState.AddModelError("Phone", Messages.PhoneRequired);
                }
                else if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Email", Messages.EmailRequired);
                }
                else
                {
                    //Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new user '{0}'", user.Username));
                    var createRes = apiHelper.Post<bool>("api/users/create", user);
                    return RedirectToAction("Index");
                }
            }
            var role = apiHelper.Get<RoleRes>($"api/roles/{user.RoleId}");
            ViewBag.RoleId = role;
            return View(user);
        }

        // GET: Users/Edit
        [HasPermission("User/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            if (id == 1)
            {
                return RedirectToAccessDenied();
            }

            var user = apiHelper.Get<UserRes>($"api/users/{id}");
            if (user == null)
            {
                return RedirectToNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.role_id);
            return View(user);
        }

        // POST: Users/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("User/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,FirstName,LastName,Phone,Email,Password,IsApproved,Comment,RoleId")] User user)
        {
            if (user == null)
            {
                return RedirectToBadRequest();
            }
            if (user.Id == 1)
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                var existUser = new UserUpdateReq();
                existUser.user_name = user.Username;
                existUser.first_name = user.FirstName;
                existUser.last_name = user.LastName;
                existUser.phone = user.Phone;
                existUser.email = user.Email;
                existUser.password = string.IsNullOrEmpty(user.Password) ? existUser.password : Crypto.HashPassword(user.Password);
                existUser.is_approved = user.IsApproved;
                //existUser.Comment = user.Comment;
                existUser.role_id = user.RoleId;

                //Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit user '{0}'", user.Username));
                var updateRes = apiHelper.Put<bool>("api/users/update", existUser);
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit
        [HasPermission("User/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            var user = apiHelper.Get<UserRes>($"api/users/{id}");
            if (user == null)
            {
                return RedirectToNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.role_id);
            return View(user);
        }

        // GET: Users/Edit
        [HasPermission("User/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            var deleteRes = apiHelper.Delete<bool>($"api/users/{id}");
            //Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete user '{0}'", user.Username));
            return RedirectToAction("Index");
        }

        [HasPermission("User/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<User> grid = new Grid<User>(db.Users.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id).Titled("#");
                grid.Columns.Add(model => model.Username).Titled(Titles.Username);
                grid.Columns.Add(model => model.FirstName).Titled(Titles.FirstName);
                grid.Columns.Add(model => model.LastName).Titled(Titles.LastName);
                grid.Columns.Add(model => model.Phone).Titled(Titles.Phone);
                grid.Columns.Add(model => model.Email).Titled(Titles.Email);
                grid.Columns.Add(model => model.PasswordHash).Titled(Titles.Password);
                //grid.Columns.Add(model => model.IsLockedOut).Titled(Titles.IsLockedOut);
                grid.Columns.Add(model => model.IsApproved).Titled(Titles.IsApproved);
                grid.Columns.Add(model => model.Comment).Titled(Titles.Comment);

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<User> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "UserList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                return File(package.GetAsByteArray(), "application/unknown", fileName);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
