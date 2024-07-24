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
using DemoAdminLTE.Resources.Views.StationViews;
using DemoAdminLTE.ViewModels;
using NLog;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;

namespace DemoAdminLTE.Controllers
{
    public class ValueController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IApiHelper apiHelper;

        // GET: Values

        public ValueController()
        {
            this.apiHelper = new ApiHelper(AppConfig.apiUrl);
        }

        [HasPermission("Value/List")]
        [HttpGet]
        public ActionResult Index(int? stationid)
        {
            Station station;
            if (!stationid.HasValue)
                //station = db.Stations.FirstOrDefault();
                station = apiHelper.Get<IEnumerable<Station>>("api/stations").FirstOrDefault();
            else
                //station = db.Stations.FirstOrDefault(o => o.Id == stationid);
                station = apiHelper.Get<IEnumerable<Station>>("api/stations").FirstOrDefault(o => o.Id == stationid);
            if (station == null)
                return RedirectToNotFound();
            if (station.Sensors.Count == 0)
            {
                Alerts.AddWarning(Messages.StationHasNoSensor);
                return RedirectToAction("Index", "Station");
            }

            ViewBag.StationId = station.Id;
            //ViewBag.DataTotal = db.SampleTimes.Where(o => o.StationId == station.Id).Count();
            ViewBag.Stations = new SelectList(db.Stations, "Id", "Name", station.Id);
            return View();
        }

        [HasPermission("Value/List")]
        [HttpGet]
        public PartialViewResult GridSearch(string search, int? stationid)
        {
            var queryable = db.SampleTimes.Where(o => o.StationId == stationid);

            if (string.IsNullOrEmpty(search))
            {
                // TODO
            }

            //var station = db.Stations.FirstOrDefault(o => o.Id == stationid);
            var station = apiHelper.Get<Station>("api/stations/" + stationid);
            if (station != null)
            {
                ViewBag.Sensors = station.Sensors.OrderBy(o => o.Id).ToArray();
            }

            return PartialView(queryable);
        }

        [HasPermission("Value/Create")]
        [HttpGet]
        public ActionResult Create(int stationid)
        {
            var station = apiHelper.Get<Station>("api/stations" + stationid);
            //Station station = db.Stations.FirstOrDefault(o => o.Id == stationid);
            if (station == null)
                return RedirectToBadRequest();
            var valueCreateViewModel = new StationValueCreateViewModel();
            valueCreateViewModel.StationId = stationid;
            valueCreateViewModel.Time = new DateTime?(DateTime.Now);
            valueCreateViewModel.Sensors = new List<SensorValueViewModel>();
            foreach (Sensor sensor in station.Sensors)
                valueCreateViewModel.Sensors.Add(new SensorValueViewModel()
                {
                    SensorId = sensor.Id,
                    SensorValue = 0.0,
                    SensorName = sensor.name
                });

            ViewBag.StationId = new SelectList(db.Stations, "Id", "Name", valueCreateViewModel.StationId);
            return View(valueCreateViewModel);
        }

        [HasPermission("Value/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StationValueCreateViewModel model)
        {
            if (model == null)
                return RedirectToBadRequest();
            if (ModelState.IsValid)
            {
                //var station = db.Stations.Find(model.StationId);
                var station = apiHelper.Get<Station>("api/stations/" + model.StationId);
                if (station == null)
                {
                    ModelState.AddModelError("", "[1001] " + Messages.UnknownError);
                    return View(model);
                }
                if (model.Sensors == null || model.Sensors.Count == 0)
                {
                    ModelState.AddModelError("", "[1002] " + Messages.UnknownError);
                    return View(model);
                }
                if (!model.Sensors.Select(s => s.SensorId).ToList().SequenceEqual(station.Sensors.Select(s => s.Id).ToList()))
                {
                    ModelState.AddModelError("", "[1003] " + Messages.UnknownError);
                    return View(model);
                }

                var result = apiHelper.Post<bool>("api/sensorvalues/create", model);
                //SampleTime entity = new SampleTime();
                //DateTime? time = model.Time;
                //if (time.HasValue)
                //{
                //    SampleTime sampleTime = entity;
                //    time = model.Time;
                //    DateTime dateTime = time.Value;
                //    sampleTime.Time = dateTime;
                //}
                //entity.StationId = model.StationId;
                //db.SampleTimes.Add(entity);
                //db.SaveChanges();
                //var sensorValueList = new List<SensorValue>();
                //foreach (var sensor in model.Sensors)
                //    sensorValueList.Add(new SensorValue()
                //    {
                //        SensorId = sensor.SensorId,
                //        SampleTimeId = entity.Id,
                //        Value = sensor.SensorValue
                //    });
                //db.SensorValues.AddRange(sensorValueList);
                //db.SaveChanges();
                return RedirectToAction("Index", new { stationid = model.StationId });
            }
            ViewBag.StationId = new SelectList(db.Stations, "Id", "Name", model.StationId);
            return View(model);
        }

        // GET: Values/Edit
        [HasPermission("Value/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();

            SampleTime sampleTime = db.SampleTimes.Find(id);
            if (sampleTime == null)
                return RedirectToNotFound();
            var model = new StationValueEditViewModel();
            model.SampleTimeId = sampleTime.Id;
            model.StationId = sampleTime.StationId;
            model.Sensors = new List<SensorValueViewModel>();
            foreach (var sensorValue in sampleTime.SensorValues)
                model.Sensors.Add(new SensorValueViewModel()
                {
                    SensorId = sensorValue.SensorId,
                    SensorValue = sensorValue.Value,
                    SensorName = sensorValue.Sensor.name
                });

            ViewBag.StationId = new SelectList(db.Stations.Where(o => o.Id == model.StationId), "Id", "Name", model.StationId);
            ViewBag.SampleTimeId = new SelectList(db.SampleTimes.Where(o => o.Id == model.SampleTimeId), "Id", "TimeStringDisplay", model.SampleTimeId);
            return View(model);
        }


        // POST: Values/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Value/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StationValueEditViewModel model)
        {
            if (model == null)
                return RedirectToBadRequest();
            if (ModelState.IsValid)
            {
                foreach (var sensor1 in model.Sensors)
                {
                    var sensor = sensor1;
                    var entity = db.SensorValues.FirstOrDefault(d => d.SampleTimeId == model.SampleTimeId && d.SensorId == sensor.SensorId);
                    if (entity != null)
                    {
                        entity.Value = sensor.SensorValue;
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                Log.ToDatabase(((CustomPrincipal)User).UserId, "Edit", string.Format("Edit Data SampleTimeId='{0}'", model.SampleTimeId));
                return RedirectToAction("Index", new { stationid = model.StationId });
            }
            ViewBag.StationId = new SelectList(db.Stations.Where(o => o.Id == model.StationId), "Id", "Name", model.StationId);
            ViewBag.SampleTimeId = new SelectList(db.SampleTimes.Where(o => o.Id == model.SampleTimeId), "Id", "TimeStringDisplay", model.SampleTimeId);
            return View(model);
        }

        // GET: Values/Edit
        [HasPermission("Value/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();
            SampleTime sampleTime = db.SampleTimes.Find(id);
            if (sampleTime == null)
                return RedirectToNotFound();

            return View(sampleTime);
        }

        // POST: Values/Edit
        [HasPermission("Value/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();
            var entity = db.SampleTimes.Find(id);
            if (entity == null)
                return RedirectToNotFound();
            var stationId = entity.StationId;
            db.SensorValues.RemoveRange(entity.SensorValues);
            db.SaveChanges();
            db.SampleTimes.Remove(entity);
            db.SaveChanges();
            Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete Data SampleTimeId='{0}'", id));
            return RedirectToAction("Index", new { stationid = stationId });
        }


        [HasPermission("Station/List")]
        [HttpGet]
        public FileContentResult Export(int? stationid)
        {
            if (stationid == null)
                return null;

            Station station = db.Stations.FirstOrDefault(o => o.Id == stationid);
            if (station == null)
                return null;

            if (station.Sensors.Count == 0)
            {
                Alerts.AddWarning(Messages.StationHasNoSensor);
                return null;
            }

            var sensors = station.Sensors.Select(o => new
            {
                id = o.Id,
                name = o.name
            }).OrderBy(o => o.id).ToArray();
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Using EPPlus from nuget
            using (var package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<SampleTime> grid = new Grid<SampleTime>(db.SampleTimes.Where(o => o.StationId == stationid).OrderBy(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.Time);
                grid.Columns.Add(model => model.Station.Name);

                foreach (var s in sensors)
                {
                    grid.Columns.Add(model => model.SensorValues.FirstOrDefault(o => o.SensorId == s.id).Value).Titled(s.name);
                }

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                    column.IsEncoded = false;
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                foreach (IGridRow<SampleTime> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "SensorData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
