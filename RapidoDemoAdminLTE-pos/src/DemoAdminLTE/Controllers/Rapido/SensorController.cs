using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.Helpers;
using DemoAdminLTE.Models;
using DemoAdminLTE.Resources.Views.SensorViews;
using NLog;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;

namespace DemoAdminLTE.Controllers
{
    public class SensorController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IApiHelper apiHelper;
        public SensorController()
        {
            apiHelper = new ApiHelper();
        }

        // GET: Sensors
        [HasPermission("Sensor/List")]
        [HttpGet]
        public ActionResult Index()
        {
            //ViewBag.DataTotal = db.Sensors.Count();
            //return View();
            ViewBag.DataTotal = 0;
            try
            {
                var result = apiHelper.Get<IEnumerable<Sensor>>("api/sensor");

                if (result != null)
                {
                    int count = result.Count();
                    ViewBag.DataTotal = count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return View();
        }

        [HttpGet]
        [HasPermission("Sensor/List")]
        public PartialViewResult GridSearch(string search)
        {


            //IQueryable<Sensor> model = db.Sensors;

            //if (!string.IsNullOrEmpty(search))
            //{
            //    model = model.Where(o => o.Name.Contains(search));
            //}

            //return PartialView(model);
            IEnumerable<Sensor> sensors = Enumerable.Empty<Sensor>();


            try
            {
                sensors = apiHelper.Get<IEnumerable<Sensor>>("api/sensor");

                if (sensors != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        sensors = sensors.Where(o => o.name.Contains(search, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"An error occurred: {ex.Message}");
            }


            // Trả về PartialView với dữ liệu đã lọc
            return PartialView("_SensorPartial", sensors);
        }

        // GET: Sensors/Create
        [HasPermission("Sensor/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sensors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Sensor/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,UpperBound,LowerBound,DifferBound,UnitName,UnitSymbol,IsActive,Params")] Sensor sensor)
        {
            if (sensor == null)
            {
                return RedirectToBadRequest();
            }
            if (ModelState.IsValid)
            {
                Sensor existSensor = db.Sensors.FirstOrDefault(d => d.Name == sensor.Name);
                if (existSensor == null)
                {
                    db.Sensors.Add(sensor);
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new sensor '{0}'", sensor.Name));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.NameExisted);
            }

            return View(sensor);
        }

        // GET: Sensors/Edit
        [HasPermission("Sensor/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Sensor sensor = db.Sensors.Find(id);
            if (sensor == null)
            {
                return RedirectToNotFound();
            }
            return View(sensor);
        }

        // POST: Sensors/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Sensor/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,UpperBound,LowerBound,DifferBound,UnitName,UnitSymbol,IsActive,Params")] Sensor sensor)
        {
            if (sensor == null)
            {
                return RedirectToBadRequest();
            }

            if (ModelState.IsValid)
            {
                Sensor existSensor = db.Sensors.FirstOrDefault(d => d.Name == sensor.Name && d.Id != sensor.Id);
                if (existSensor == null)
                {
                    Sensor dbSensor = db.Sensors.FirstOrDefault(d => d.Id == sensor.Id);

                    dbSensor.Name = sensor.Name;
                    dbSensor.UpperBound = sensor.UpperBound;
                    dbSensor.LowerBound = sensor.LowerBound;
                    dbSensor.DifferBound = sensor.DifferBound;
                    dbSensor.UnitSymbol = sensor.UnitSymbol;
                    dbSensor.IsActive = sensor.IsActive;
                    dbSensor.Params = sensor.Params;

                    db.Entry(dbSensor).State = EntityState.Modified;
                    db.SaveChanges();
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit sensor '{0}'", sensor.Name));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.NameExisted);
            }

            return View(sensor);
        }

        // GET: Sensors/Edit
        [HasPermission("Sensor/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Sensor sensor = db.Sensors.Find(id);
            if (sensor == null)
            {
                return RedirectToNotFound();
            }
            return View(sensor);
        }

        // GET: Sensors/Edit
        [HasPermission("Sensor/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToBadRequest();
            }
            Sensor sensor = db.Sensors.Find(id);
            if (sensor == null)
            {
                return RedirectToBadRequest();
            }

            db.Sensors.Remove(sensor);
            db.SaveChanges();

            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete sensor '{0}'", sensor.Name));
            return RedirectToAction("Index");
        }

        [HasPermission("Sensor/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Sensor> grid = new Grid<Sensor>(db.Sensors.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.Name);
                grid.Columns.Add(model => model.UpperBound);
                grid.Columns.Add(model => model.LowerBound);
                grid.Columns.Add(model => model.DifferBound);
                grid.Columns.Add(model => model.UnitSymbol);
                grid.Columns.Add(model => model.IsActive);
                grid.Columns.Add(model => model.Params);

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<Sensor> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "SensorList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
