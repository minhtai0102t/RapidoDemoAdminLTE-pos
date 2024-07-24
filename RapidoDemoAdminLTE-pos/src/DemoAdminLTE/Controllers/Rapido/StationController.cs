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
using DemoAdminLTE.Resources.Views.StationViews;
using DemoAdminLTE.Utils;
using DemoAdminLTE.ViewModels;
using NLog;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;

namespace DemoAdminLTE.Controllers
{
    public class StationController : BaseController
    {
        private readonly DemoContext db = new DemoContext();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IApiHelper apiHelper;

        public StationController()
        {
            this.apiHelper = new ApiHelper(AppConfig.apiUrl);
        }
        // GET: Stations
        [HttpGet]
        [HasPermission("Station/List")]
        public ActionResult Index()
        {
            ViewBag.DataTotal = 0;
            try
            {
                var result = apiHelper.Get<IEnumerable<Station>>("api/station");
                ViewBag.DataTotal = result.Count();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return View();
        }

        [HttpGet]
        [HasPermission("Station/List")]
        public PartialViewResult GridSearch(string search)
        {
            var model = Enumerable.Empty<Station>();
            try
            {
                model = apiHelper.Get<IEnumerable<Station>>("api/station");

                if (model != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        model = model.Where(o =>
                            o.Name.Contains(search)
                               || o.Address.Contains(search)
                               || o.SmsAlertPhoneNumber.Contains(search)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return PartialView(model);
        }

        // JSON-GET: SensorValue/_lastedValueJson?sensorid=1
        [HasPermission("Station/Chart")]
        public JsonResult getLasted(int id, int tid)
        {
            var api = string.Format("api/station/{0}.", id);

            //Station station = db.Stations.Find(id);
            var result = apiHelper.Get<Station>(api);
            if (result == null)
                return Json("not found", JsonRequestBehavior.AllowGet);

            var sampleTime = result.SampleTimes.OrderBy(o => o.Time).FirstOrDefault(o => o.Id > tid);
            return sampleTime == null ? Json("null", JsonRequestBehavior.AllowGet) : Json(new
            {
                tid = sampleTime.Id,
                time = sampleTime.Time,
                values = sampleTime.SensorValues.Select(v => new
                {
                    sid = v.Sensor.Id,
                    value = v.Value,
                    tooltip_label = v.Value,
                    tooltip_title = v.SampleTime.Time.ToString("dd/MM/yyyy HH:mm:ss")
                })
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [HasPermission("Station/Chart")]
        public ActionResult Chart(int? id, long? from, long? to)
        {

            if (!id.HasValue)
                return RedirectToBadRequest();
            DateTime? timeFrom = new DateTime?();
            DateTime? timeTo = new DateTime?();
            timeFrom = !from.HasValue || from.Value <= 0L ? new DateTime?(DateTime.Now.AddDays(-7.0)) : new DateTime?(new DateTime(from.Value));
            timeTo = !to.HasValue || to.Value <= 0L ? new DateTime?(DateTime.Now) : new DateTime?(new DateTime(to.Value));

            //var station = db.Stations.Find(id);
            var station = apiHelper.Get<Station>("api/stations/" + id);

            if (station == null)
                return RedirectToNotFound();
            if (station.Sensors.Count == 0)
            {
                Alerts.AddWarning(Messages.StationHasNoSensor);
                return RedirectToAction("Index");
            }
            var source = new List<ChartSensorView>();
            var random = new Random();
            string str = "";
            foreach (var sampleTime in !timeFrom.HasValue || !timeTo.HasValue ? station.SampleTimes.OrderByDescending(x => x.Time) : station.SampleTimes.Where(o =>
            {
                DateTime? nullable1 = timeFrom;
                DateTime time1 = o.Time;
                if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() <= time1 ? 1 : 0) : 0) == 0)
                    return false;
                DateTime time2 = o.Time;
                DateTime? nullable2 = timeTo;
                return nullable2.HasValue && time2 <= nullable2.GetValueOrDefault();
            }).OrderByDescending(x => x.Time))
            {
                str = "\"" + sampleTime.Time.ToString("HH:mm") + "\"," + str;
                foreach (var sensorValue1 in sampleTime.SensorValues)
                {
                    var sensorValue = sensorValue1;
                    var chartSensorView1 = source.FirstOrDefault(x => x.Id == sensorValue.Sensor.Id);
                    if (chartSensorView1 == null)
                    {
                        var chartSensorView2 = new ChartSensorView()
                        {
                            Id = sensorValue.Sensor.Id,
                            Name = sensorValue.Sensor.name,
                            Color = random.Next(0, 200).ToString() + "," + random.Next(0, 200) + "," + random.Next(0, 200)
                        };
                        chartSensorView2.ColorFill = "'rgba(" + chartSensorView2.Color + ",0.7)'";
                        chartSensorView2.ColorBorder = "'rgba(" + chartSensorView2.Color + ",1.0)'";
                        chartSensorView2.Data = sensorValue.Value.ToStringEnUs();
                        chartSensorView2.TooltipLabel = "'" + sensorValue.Value.ToStringEnUs("#.00") + "'";
                        chartSensorView2.TooltipTitle = "'" + sensorValue.SampleTime.TimeStringDisplay + "'";
                        source.Add(chartSensorView2);
                    }
                    else
                    {
                        chartSensorView1.Data = sensorValue.Value.ToStringEnUs() + "," + chartSensorView1.Data;
                        chartSensorView1.TooltipLabel = "'" + sensorValue.Value.ToStringEnUs("#.00") + "'," + chartSensorView1.TooltipLabel;
                        chartSensorView1.TooltipTitle = "'" + sensorValue.SampleTime.TimeStringDisplay + "'," + chartSensorView1.TooltipTitle;
                    }
                }
            }
            var chartStationView = new ChartStationView();
            chartStationView.Id = station.Id;
            chartStationView.Name = station.Name;
            chartStationView.ChartLabel = str;
            chartStationView.LastedSampleTimeId = station.SampleTimes.Count > 0 ? station.SampleTimes.Max(x => x.Id) : 0;
            chartStationView.Sensors = source;

            ViewBag.ChartView = chartStationView;
            return View(new ChartSearchView()
            {
                Id = station.Id,
                TimeFrom = timeFrom,
                TimeTo = timeTo
            });
        }

        [HttpPost]
        [HasPermission("Station/Chart")]
        public ActionResult ChartSubmit([Bind(Include = "Id,TimeFrom,TimeTo")] ChartSearchView model)
        {
            if (model == null)
                return RedirectToBadRequest();
            long? local1;
            DateTime? nullable2 = model.TimeFrom;
            DateTime dateTime;
            long num1;
            if (!nullable2.HasValue)
            {
                num1 = 0L;
            }
            else
            {
                nullable2 = model.TimeFrom;
                dateTime = nullable2.Value;
                num1 = dateTime.Ticks;
            }
            local1 = new long?(num1);
            long? local2;
            nullable2 = model.TimeTo;
            long num2;
            if (!nullable2.HasValue)
            {
                num2 = 0L;
            }
            else
            {
                nullable2 = model.TimeTo;
                dateTime = nullable2.Value;
                num2 = dateTime.Ticks;
            }
            local2 = new long?(num2);
            return RedirectToAction("Chart", new
            {
                id = model.Id,
                from = local1,
                to = local2
            });
        }

        // GET: Stations/Create
        [HasPermission("Station/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            Station station = new Station();

            var results = apiHelper.Get<IEnumerable<Station>>("/api/stations");

            //station.AllSensors = db.Sensors.ToList().Select(o => new SelectListItem
            //{
            //    Text = o.name,
            //    Value = o.Id.ToString()
            //});

            station.AllSensors = results.ToList().Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.Id.ToString()
            });
            return View(station);
        }


        // POST: Stations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Station/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Address,Longitude,Latitude,IsActive,IsSmsAlertEnable,SmsAlertPhoneNumber,Params,SelectedSensors")] Station station)
        {
            if (station == null)
                return RedirectToBadRequest();
            if (ModelState.IsValid)
            {

                var results = apiHelper.Get<IEnumerable<Station>>("/api/stations");
                if (results.FirstOrDefault(d => d.Name == station.Name) == null)
                {
                    station.SmsAlertPhoneNumber = string.IsNullOrEmpty(station.SmsAlertPhoneNumber) ? "" : station.SmsAlertPhoneNumber.Replace("-", "").Replace(" ", "");
                    //db.Stations.Add(station);
                    //db.SaveChanges();
                    station.Sensors = new List<Sensor>();
                    var intSet = new HashSet<int>(station.SelectedSensors ?? new List<int>());
                    foreach (Sensor sensor in db.Sensors)
                    {
                        if (intSet.Contains(sensor.Id))
                            station.Sensors.Add(sensor);
                    }
                    //db.Entry(station).State = EntityState.Modified;
                    //db.SaveChanges();
                    var isCreated = apiHelper.Post<bool>("/api/stations/create", jsonContent: station);
                    Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new station '{0}'", station.Name));
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Name", Messages.NameExisted);
            }
            station.AllSensors = db.Sensors.ToList().Select(o => new SelectListItem()
            {
                Text = o.name,
                Value = o.Id.ToString()
            });
            return View(station);
        }

        // GET: Stations/Edit
        [HasPermission("Station/Edit")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();
            //var station = db.Stations.Find(id);

            var station = apiHelper.Get<Station>("/api/stations/" + id);
            var sensors = apiHelper.Get<IEnumerable<Sensor>>("/api/sensors/");

            if (station == null)
                return RedirectToNotFound();

            station.AllSensors = sensors.ToList().Select(o => new SelectListItem()
            {
                Text = o.name,
                Value = o.Id.ToString()
            });
            return View(station);
        }

        // POST: Stations/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HasPermission("Station/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Longitude,Latitude,IsActive,IsSmsAlertEnable,SmsAlertPhoneNumber,Params,SelectedSensors")] Station station)
        {
            if (station == null)
                return RedirectToBadRequest();
            if (ModelState.IsValid)
            {
                var stations = apiHelper.Get<IEnumerable<Station>>("/api/stations");
                if (stations.FirstOrDefault(d => d.Name == station.Name && d.Id != station.Id) == null)
                {
                    var station1 = stations.FirstOrDefault(d => d.Id == station.Id);
                    station.SmsAlertPhoneNumber = string.IsNullOrEmpty(station.SmsAlertPhoneNumber) ? "" : station.SmsAlertPhoneNumber.Replace("-", "").Replace(" ", "");
                    if (TryUpdateModel(station1, new string[8]
                    {
                        "Name",
                        "Address",
                        "Longitude",
                        "Latitude",
                        "IsActive",
                        "IsSmsAlertEnable",
                        "SmsAlertPhoneNumber",
                        "Params"
                    }))
                    {
                        var intSet = new HashSet<int>(station.SelectedSensors ?? new List<int>());
                        foreach (var sensor in db.Sensors)
                        {
                            if (!intSet.Contains(sensor.Id))
                                station1.Sensors.Remove(sensor);
                            else
                                station1.Sensors.Add(sensor);
                        }
                        var result = apiHelper.Put<bool>("/api/stations/update", station1);
                        //db.Entry(station1).State = EntityState.Modified;
                        //db.SaveChanges();
                        Log.ToDatabase(((CustomPrincipal)User).UserId, nameof(Edit), string.Format("Edit station '{0}'", station.Name));
                        return RedirectToAction("Index");
                    }
                }
                ModelState.AddModelError("Name", Messages.NameExisted);
            }
            var sensors = apiHelper.Get<IEnumerable<Sensor>>("/api/sensors");

            station.AllSensors = sensors.ToList().Select(o => new SelectListItem()
            {
                Text = o.name,
                Value = o.Id.ToString()
            });
            return View(station);
        }



        // GET: Stations/Edit
        [HasPermission("Station/Delete")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();
            var station = apiHelper.Get<Station>("api/stations/" + id);
            //var station = db.Stations.Find(id);
            if (station == null)
                return RedirectToNotFound();

            var sensors = apiHelper.Get<IEnumerable<Sensor>>("api/sensors");
            station.AllSensors = sensors.ToList().Select(o => new SelectListItem()
            {
                Text = o.name,
                Value = o.Id.ToString()
            });
            return View(station);
        }

        // GET: Stations/Edit
        [HasPermission("Station/Delete")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int? id)
        {
            if (!id.HasValue)
                return RedirectToBadRequest();
            var entity = apiHelper.Delete<bool>("/api/delete/" + id);
            //var entity = db.Stations.Find(id);
            if (entity == false)
                return RedirectToBadRequest();
            //db.Stations.Remove(entity);
            //db.SaveChanges();
            //Log.ToDatabase(((CustomPrincipal)User).UserId, "Delete", string.Format("Delete station '{0}'", entity.));
            return RedirectToAction("Index");
        }

        [HasPermission("Station/List")]
        [HttpGet]
        public FileContentResult Export()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                int row = 2;
                int col = 1;

                package.Workbook.Worksheets.Add("Data");

                IGrid<Station> grid = new Grid<Station>(db.Stations.OrderByDescending(o => o.Id));
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.Id);
                grid.Columns.Add(model => model.Name);
                grid.Columns.Add(model => model.Address);
                grid.Columns.Add(model => model.Longitude);
                grid.Columns.Add(model => model.Latitude);
                grid.Columns.Add(model => model.IsActive);
                grid.Columns.Add(model => model.IsSmsAlertEnable);
                grid.Columns.Add(model => model.SmsAlertPhoneNumber);
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

                foreach (IGridRow<Station> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);
                    row++;
                }

                string fileName = "StationList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
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
