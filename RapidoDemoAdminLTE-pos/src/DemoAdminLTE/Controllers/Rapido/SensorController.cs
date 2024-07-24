using System;
using System.Collections.Generic;
using System.Data;
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
            apiHelper = new ApiHelper(AppConfig.apiUrl);
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
                var result = apiHelper.Get<IEnumerable<Sensor>>("api/sensors");

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
            IEnumerable<Sensor> model = Enumerable.Empty<Sensor>();
            try
            {
                model = apiHelper.Get<IEnumerable<Sensor>>("api/sensors");

                if (model != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        model = model.Where(o => o.name.Contains(search));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return PartialView(model);
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

                try
                {
                    //Sensor existSensor = db.Sensors.FirstOrDefault(d => d.Name == sensor.Name);
                    //if (existSensor == null)
                    //{
                    //    db.Sensors.Add(sensor);
                    //    db.SaveChanges();
                    //    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new sensor '{0}'", sensor.Name));
                    //    return RedirectToAction("Index");
                    //}
                    var result = apiHelper.Post<string>("api/sensors", jsonContent: sensor);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
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

            var result = apiHelper.Get<Sensor>("api/sensors/" + id);
            if (result == null)
            {
                return RedirectToNotFound();
            }
            return View(result);
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
                var result = apiHelper.Put<bool>("/api/sensors/update", sensor);
                //Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit sensor '{0}'", sensor.Name));
                return RedirectToAction("Index");
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

            var result = apiHelper.Get<Sensor>("api/sensors/" + id);
            if (result == null)
            {
                return RedirectToNotFound();
            }
            return View(result);
            //Sensor sensor = db.Sensors.Find(id);
            //if (sensor == null)
            //{
            //    return RedirectToNotFound();
            //}
            //return View(sensor);
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

            var sensor = apiHelper.Get<Sensor>("api/sensors/" + id);
            if (sensor == null)
            {
                return RedirectToNotFound();
            }

            var result = apiHelper.Delete<string>("api/sensors/" + id);
            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete sensor '{0}'", sensor.name));

            return RedirectToAction("Index");

        }

        [HasPermission("Sensor/List")]
        [HttpGet]
        public FileContentResult Export()
        {

            var sensors = apiHelper.Get<IEnumerable<Sensor>>("api/sensors");

            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Sensor> grid = new Grid<Sensor>(sensors.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.name);
                grid.Columns.Add(model => model.upper_bound);
                grid.Columns.Add(model => model.lower_bound);
                grid.Columns.Add(model => model.differ_bound);
                grid.Columns.Add(model => model.unit_symbol);
                grid.Columns.Add(model => model.is_active);
                grid.Columns.Add(model => model.param);

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
