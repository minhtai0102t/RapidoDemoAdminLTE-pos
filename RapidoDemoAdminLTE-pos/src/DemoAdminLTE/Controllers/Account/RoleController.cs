using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DemoAdminLTE.Resources.Shared;
using DemoAdminLTE.Resources.Views.RoleViews;
using System.Collections.Generic;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.ViewModels;
using NLog;

namespace DemoAdminLTE.Controllers
{
    public class RoleController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // GET: Roles
        [HttpGet]
        [HasPermission("Role/List")]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.Roles.Count();
            return View();
        }

        [HttpGet]
        [HasPermission("Role/List")]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<Role> model = db.Roles;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o =>
                    o.RoleName.Contains(search)
                );
            }

            return PartialView(model);
        }

        // GET: Roles/Create
        [HasPermission("Role/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            Role role = new Role();

            role.TreePermissions = CreatePermissionTree(role.SelectedPermissions);

            return View(role);
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Role/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoleName,TreePermissions")] Role role)
        {
            if (role == null)
            {
                return RedirectToBadRequest();
            }

            if (ModelState.IsValid)
            {
                Role existRole = db.Roles.FirstOrDefault(d => d.RoleName == role.RoleName);
                if (existRole == null)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();

                    role.Permissions = new List<Permission>();
                    foreach (Permission permission in db.Permissions)
                    {
                        if (role.TreePermissions.SelectedIds.Contains(permission.Id))
                        {
                            role.Permissions.Add((permission));
                        }
                    }
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();

                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new role '{0}'", role.RoleName));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("RoleName", Messages.RoleNameExisted);
            }

            role.TreePermissions = CreatePermissionTree(role.TreePermissions.SelectedIds.ToList());
            return View(role);
        }

        // GET: Roles/Edit
        [HasPermission("Role/Edit")]
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

            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return RedirectToNotFound();
            }


            role.TreePermissions = CreatePermissionTree(role.SelectedPermissions);

            return View(role);
        }

        // POST: Roles/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Role/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoleName,TreePermissions")] Role role)
        {
            if (role == null)
            {
                return RedirectToBadRequest();
            }
            if (role.Id == 1)
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                Role existRole = db.Roles.FirstOrDefault(d => d.RoleName == role.RoleName && d.Id != role.Id);
                if (existRole == null)
                {
                    Role dbRole = db.Roles.FirstOrDefault(d => d.Id == role.Id);

                    if (TryUpdateModel(dbRole, new string[] { "RoleName" }))
                    {
                        foreach (Permission permission in db.Permissions)
                        {
                            if (!role.TreePermissions.SelectedIds.Contains(permission.Id))
                            {
                                dbRole.Permissions.Remove(permission);
                            }
                            else
                            {
                                dbRole.Permissions.Add((permission));
                            }
                        }

                        db.Entry(dbRole).State = EntityState.Modified;
                        db.SaveChanges();
                        Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit role '{0}'", role.RoleName));
                        return RedirectToAction("Index");
                    }
                }
                ModelState.AddModelError("RoleName", Messages.RoleNameExisted);
            }


            role.TreePermissions = CreatePermissionTree(role.TreePermissions.SelectedIds.ToList());
            return View(role);
        }

        // GET: Roles/Edit
        [HasPermission("Role/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            if (id == 1 || id == 2)
            {
                return RedirectToAccessDenied();
            }

            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return RedirectToNotFound();
            }

            role.TreePermissions = CreatePermissionTree(role.SelectedPermissions);
            return View(role);
        }

        // GET: Roles/Edit
        [HasPermission("Role/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            if (id == 1 || id == 2)
            {
                return RedirectToAccessDenied();
            }

            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return RedirectToNotFound();
            }

            db.Roles.Remove(role);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete role '{0}'", role.RoleName));
            return RedirectToAction("Index");
        }

        private MvcTree CreatePermissionTree(List<int> SelectedPermissions)
        {
            MvcTreeNode root = new MvcTreeNode(Strings.All);
            MvcTree expectedTree = new MvcTree();

            expectedTree.Nodes.Add(root);
            expectedTree.SelectedIds = new HashSet<int>(SelectedPermissions ?? new List<int>());


            IEnumerable<PermissionView> allPermissions = db
                .Permissions
                .Select(p => new PermissionView
                {
                    Id = p.Id,
                    Group = p.Group,
                    Action = p.Action,
                }); ;

            foreach (IGrouping<String, PermissionView> group in allPermissions.GroupBy(p => p.Group).OrderBy(p => p.Key ?? p.FirstOrDefault().Action))
            {
                MvcTreeNode groupNode = new MvcTreeNode(group.Key.GetPermissionResource());
                foreach (PermissionView permission in group.OrderBy(p => p.Action))
                {

                    if (groupNode.Title == null)
                        root.Children.Add(new MvcTreeNode(permission.Id, permission.Action.GetPermissionResource()));
                    else
                        groupNode.Children.Add(new MvcTreeNode(permission.Id, permission.Action.GetPermissionResource()));
                }

                if (groupNode.Title != null)
                    root.Children.Add(groupNode);
            }

            return expectedTree;
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
