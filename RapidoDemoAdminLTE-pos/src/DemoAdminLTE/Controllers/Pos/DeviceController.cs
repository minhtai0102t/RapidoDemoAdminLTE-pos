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
using DemoAdminLTE.Resources.Views.DeviceViews;
using NLog;
using DemoAdminLTE.Extensions;
using static Humanizer.On;

namespace DemoAdminLTE.Controllers
{
    public class DeviceController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // GET: Device
        [HasPermission("Device/List")]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DataTotal = db.Devices.Count();
            return View();
        }

        [HasPermission("Device/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search)
        {
            IQueryable<Device> model = db.Devices;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o => o.Name.Contains(search));
            }

            return PartialView(model);
        }

        // GET: Device/Create
        [HasPermission("Device/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Name");
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Device/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Status,Description,SerialNumber,DeviceTypeId,Location,Latitude,Longitude,HardwareVersion,SoftwareVersion,Activated,Password,MACAddress")] Device device)
        {
            if (device == null)
            {
                return RedirectToBadRequest();
            }
            if (ModelState.IsValid)
            {
                Device existDevice = db.Devices.FirstOrDefault(d => d.SerialNumber == device.SerialNumber);
                if (existDevice == null)
                {
                    db.Devices.Add(device);
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new device '{0}'", device.SerialNumber));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.SerialNumberExisted);
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.DeviceTypeId);
            return View(device);
        }

        // GET: Device/Edit
        [HasPermission("Device/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.DeviceTypeId);
            return View(device);
        }

        // POST: Device/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Device/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Status,Description,SerialNumber,DeviceTypeId,Location,Latitude,Longitude,HardwareVersion,SoftwareVersion,Activated,Password,MACAddress")] Device device)
        {
            if (device == null)
            {
                return RedirectToBadRequest();
            }

            if (ModelState.IsValid)
            {
                Device existDevice = db.Devices.FirstOrDefault(d => d.SerialNumber == device.SerialNumber && d.Id != device.Id);
                if (existDevice == null)
                {
                    Device dbDevice = db.Devices.FirstOrDefault(d => d.Id == device.Id);

                    dbDevice.Name = device.Name;
                    dbDevice.Status = device.Status;
                    dbDevice.Description = device.Description;
                    dbDevice.SerialNumber = device.SerialNumber;
                    dbDevice.DeviceTypeId = device.DeviceTypeId;
                    dbDevice.Location = device.Location;
                    dbDevice.Latitude = device.Latitude;
                    dbDevice.Longitude = device.Longitude;
                    dbDevice.HardwareVersion = device.HardwareVersion;
                    dbDevice.SoftwareVersion = device.SoftwareVersion;
                    dbDevice.Activated = device.Activated;
                    dbDevice.Password = device.Password;
                    dbDevice.MACAddress = device.MACAddress;

                    db.Entry(dbDevice).State = EntityState.Modified;
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit device '{0}'", device.SerialNumber));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.SerialNumberExisted);
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.DeviceTypeId);
            return View(device);
        }

        // GET: Device/Delete
        [HasPermission("Device/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Name", device.DeviceTypeId);
            return View(device);
        }

        // GET: Device/Delete
        [HasPermission("Device/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return RedirectToBadRequest();
            }

            db.Devices.Remove(device);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete device '{0}'", device.Name));
            return RedirectToAction("Index");
        }

        // GET: Device/Product
        [HasPermission("Device/List")]
        [HttpGet]
        public ActionResult Product(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return RedirectToNotFound();
            }

            ViewBag.Device = device;
            ViewBag.DataTotal = db.DeviceProducts.Where(o => o.DeviceId == id).Count();

            return View();
        }

        [HasPermission("Device/List")]
        [HttpGet]
        public PartialViewResult ProductGridSearch(int id, string search)
        {
            IQueryable<DeviceProduct> model = db.DeviceProducts.Where(o => o.DeviceId == id);

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(o => o.Device.Name.Contains(search) || o.Product.Name.Contains(search));
            }

            return PartialView(model);
        }

        [HasPermission("Device/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Device> grid = new Grid<Device>(db.Devices.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.Name);
                grid.Columns.Add(model => model.Status);
                grid.Columns.Add(model => model.Description);
                grid.Columns.Add(model => model.SerialNumber);
                grid.Columns.Add(model => model.DeviceTypeId);
                grid.Columns.Add(model => model.Location);
                grid.Columns.Add(model => model.Latitude);
                grid.Columns.Add(model => model.Longitude);
                grid.Columns.Add(model => model.HardwareVersion);
                grid.Columns.Add(model => model.SoftwareVersion);
                grid.Columns.Add(model => model.Activated);
                grid.Columns.Add(model => model.Password);
                grid.Columns.Add(model => model.MACAddress);

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<Device> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "DeviceList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
