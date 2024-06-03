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
using DemoAdminLTE.Resources.Views.TransactionViews;
using NLog;
using DemoAdminLTE.Extensions;
using System.Data.Common;

namespace DemoAdminLTE.Controllers
{
    public class TransactionController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // GET: Transaction
        [HasPermission("Transaction/List")]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.Transactions.Count();
            return View();
        }

        [HasPermission("Transaction/List")]
        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.DataTotal = db.Transactions.Count();
            return View();
        }

        [HasPermission("Transaction/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<Transaction> model = db.Transactions;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o => o.Product_Name.Contains(search) || o.Device_Name.Contains(search));
            }

            return PartialView(model);
        }

        // GET: Transaction/Create
        [HasPermission("Transaction/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Name");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Transaction/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeviceId,ProductId,Quantity")] Transaction transaction)
        {
            if (transaction == null)
            {
                return RedirectToBadRequest();
            }
            while (ModelState.IsValid)
            {
                if (transaction.Quantity <= 0)
                {
                    ModelState.AddModelError("Quantity", Messages.QuantityInvalid);
                    break;
                }

                var device = db.Devices.FirstOrDefault(o => o.Id == transaction.DeviceId);
                if (device == null)
                {
                    ModelState.AddModelError("DeviceId", Messages.DeviceNotFound);
                    break;
                }

                var product = db.Products.FirstOrDefault(o => o.Id == transaction.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("ProductId", Messages.ProductNotFound);
                    break;
                }

                var deviceProduct = db.DeviceProducts.FirstOrDefault(o => o.DeviceId == transaction.DeviceId && o.ProductId == transaction.ProductId);
                if (deviceProduct == null)
                {
                    ModelState.AddModelError("", Messages.ProductNotSetupInDevice);
                    break;
                }

                if (transaction.Quantity > deviceProduct.Quantity)
                {
                    ModelState.AddModelError("Quantity", Messages.QuantityNotAvailable);
                    break;
                }

                /* 
                 * add new transaction 
                 */

                transaction.Device_Name = device.Name;
                transaction.Device_SerialNumber = device.SerialNumber;
                transaction.Device_MACAddress = device.MACAddress;
                transaction.Device_TypeName = device.DeviceType.Name;
                transaction.Device_Location = device.Location;
                transaction.Device_Latitude = device.Latitude;
                transaction.Device_Longitude = device.Longitude;
                transaction.Device_HardwareVersion = device.HardwareVersion;
                transaction.Device_SoftwareVersion = device.SoftwareVersion;

                transaction.Product_Name = product.Name;
                transaction.Product_SKU = product.SKU;
                transaction.Product_CategoryName = product.Category.Name;
                transaction.Product_UnitPrice = product.UnitPrice;
                transaction.Product_CurrencyUnit = product.CurrencyUnit;
                transaction.Product_UnitType = product.UnitType;

                transaction.Product_InventoryBeforeTransaction = deviceProduct.Quantity;

                transaction.Note = "manual";

                db.Transactions.Add(transaction);
                db.SaveChanges();
                Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new transaction '{0}'", transaction.Id));

                /* 
                 * update stock 
                 */

                deviceProduct.Quantity -= transaction.Quantity;
                deviceProduct.LastUpdate = DateTime.Now;

                db.Entry(deviceProduct).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Name", transaction.DeviceId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", transaction.ProductId);
            return View(transaction);
        }

        // GET: Transaction/Details
        [HasPermission("Transaction/List")]
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Name", transaction.DeviceId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", transaction.ProductId);
            return View(transaction);
        }

        // GET: Transaction/Delete
        [HasPermission("Transaction/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.DeviceId = new SelectList(db.Devices, "Id", "Name", transaction.DeviceId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", transaction.ProductId);
            return View(transaction);
        }

        // GET: Transaction/Delete
        [HasPermission("Transaction/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToBadRequest();
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete transaction '{0}'", transaction.Id));
            return RedirectToAction("Index");
        }

        [HasPermission("Transaction/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Transaction> grid = new Grid<Transaction>(db.Transactions.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);


                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<Transaction> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "TransactionList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
