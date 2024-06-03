using NonFactors.Mvc.Grid;
using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using OfficeOpenXml;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DemoAdminLTE.Resources.Views.UserViews;
using System.Web.Helpers;
using NLog;
using DemoAdminLTE.Extensions;

namespace DemoAdminLTE.Controllers
{
    public class UserController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // GET: Users
        [HasPermission("User/List")]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.Users.Count();
            return View();
        }

        [HasPermission("User/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<User> model = db.Users;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o =>
                    o.Username.Contains(search)
                       || o.FirstName.Contains(search)
                       || o.LastName.Contains(search)
                       || o.Phone.Contains(search)
                       || o.Email.Contains(search)
                       || o.Comment.Contains(search)
                );
            }

            return PartialView(model);
        }

        // GET: Users/Create
        [HasPermission("User/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", 2);
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
                    // username duplicate check
                    User existUser = db.Users.FirstOrDefault(d => d.Username == user.Username);
                    if (existUser != null) ModelState.AddModelError("Username", Messages.UsernameExisted);


                    // username email check
                    User existUser1 = db.Users.FirstOrDefault(d => d.Email == user.Email);
                    if (existUser1 != null) ModelState.AddModelError("Email", Messages.EmailExisted);

                    // username phone check
                    user.Phone = user.Phone.Replace("-", "").Replace(" ", "");
                    User existUser2 = db.Users.FirstOrDefault(d => d.Phone == user.Phone);
                    if (existUser2 != null) ModelState.AddModelError("Phone", Messages.PhoneExisted);

                    if (existUser == null && existUser1 == null && existUser2 == null)
                    {
                        user.PasswordHash = string.IsNullOrEmpty(user.Password) ? user.PasswordHash : Crypto.HashPassword(user.Password);
                        user.IsLockedOut = false;

                        db.Users.Add(user);
                        db.SaveChanges();
                        Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new user '{0}'", user.Username));
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.RoleId);
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

            User user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.RoleId);
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
                // username duplicate check
                User existUser = db.Users.FirstOrDefault(d => d.Username == user.Username && d.Id != user.Id);
                if (existUser != null) ModelState.AddModelError("Username", Messages.UsernameExisted);


                // username email check
                User existUser1 = db.Users.FirstOrDefault(d => d.Email == user.Email && d.Id != user.Id);
                if (existUser1 != null) ModelState.AddModelError("Email", Messages.EmailExisted);

                // username phone check
                user.Phone = user.Phone.Replace("-", "").Replace(" ", "");
                User existUser2 = db.Users.FirstOrDefault(d => d.Phone == user.Phone && d.Id != user.Id);
                if (existUser2 != null) ModelState.AddModelError("Phone", Messages.PhoneExisted);

                if (existUser == null && existUser1 == null && existUser2 == null)
                {
                    User dbUser = db.Users.FirstOrDefault(d => d.Id == user.Id);

                    dbUser.Username = user.Username;
                    dbUser.FirstName = user.FirstName;
                    dbUser.LastName = user.LastName;
                    dbUser.Phone = user.Phone;
                    dbUser.Email = user.Email;
                    dbUser.PasswordHash = string.IsNullOrEmpty(user.Password) ? dbUser.PasswordHash : Crypto.HashPassword(user.Password);
                    dbUser.IsApproved = user.IsApproved;
                    dbUser.Comment = user.Comment;
                    dbUser.RoleId = user.RoleId;

                    db.Entry(dbUser).State = EntityState.Modified;
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit user '{0}'", user.Username));
                    return RedirectToAction("Index");
                }
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "RoleName", user.RoleId);
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToNotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete user '{0}'", user.Username));
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
