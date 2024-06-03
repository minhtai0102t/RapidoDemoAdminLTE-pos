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
using DemoAdminLTE.Resources.Views.ProductViews;
using NLog;
using DemoAdminLTE.Extensions;

namespace DemoAdminLTE.Controllers
{
    public class ProductController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // GET: Product
        [HasPermission("Product/List")]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.Products.Count();
            return View();
        }

        [HasPermission("Product/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<Product> model = db.Products;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o => o.Name.Contains(search));
            }

            return PartialView(model);
        }

        // GET: Product/Create
        [HasPermission("Product/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Product/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Status,Description,SKU,CategoryId,UnitPrice,CurrencyUnit,UnitType,LowQuantityThreshold")] Product product)
        {
            if (product == null)
            {
                return RedirectToBadRequest();
            }
            if (ModelState.IsValid)
            {
                Product existProduct = db.Products.FirstOrDefault(d => d.SKU == product.SKU);
                if (existProduct == null)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new product '{0}'", product.SKU));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.SKUExisted);
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit
        [HasPermission("Product/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Product/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Status,Description,SKU,CategoryId,UnitPrice,CurrencyUnit,UnitType,LowQuantityThreshold")] Product product)
        {
            if (product == null)
            {
                return RedirectToBadRequest();
            }

            if (ModelState.IsValid)
            {
                Product existProduct = db.Products.FirstOrDefault(d => d.SKU == product.SKU && d.Id != product.Id);
                if (existProduct == null)
                {
                    Product dbProduct = db.Products.FirstOrDefault(d => d.Id == product.Id);

                    dbProduct.Name = product.Name;
                    dbProduct.Status = product.Status;
                    dbProduct.Description = product.Description;
                    dbProduct.SKU = product.SKU;
                    dbProduct.CategoryId = product.CategoryId;
                    dbProduct.UnitPrice = product.UnitPrice;
                    dbProduct.CurrencyUnit = product.CurrencyUnit;
                    dbProduct.UnitType = product.UnitType;
                    dbProduct.LowQuantityThreshold = product.LowQuantityThreshold;

                    db.Entry(dbProduct).State = EntityState.Modified;
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit product '{0}'", product.SKU));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.SKUExisted);
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete
        [HasPermission("Product/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit
        [HasPermission("Product/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToBadRequest();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete product '{0}'", product.Name));
            return RedirectToAction("Index");
        }

        [HasPermission("Product/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Product> grid = new Grid<Product>(db.Products.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.Name);
                grid.Columns.Add(model => model.Status);
                grid.Columns.Add(model => model.Description);

                grid.Columns.Add(model => model.SKU);
                grid.Columns.Add(model => model.CategoryId);
                grid.Columns.Add(model => model.UnitPrice);
                grid.Columns.Add(model => model.CurrencyUnit);
                grid.Columns.Add(model => model.UnitType);
                grid.Columns.Add(model => model.LowQuantityThreshold);

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<Product> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "ProductList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
