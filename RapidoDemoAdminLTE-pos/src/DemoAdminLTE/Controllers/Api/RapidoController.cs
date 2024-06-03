using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using NLog;
using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.Models;
using DemoAdminLTE.Utils;
using DemoAdminLTE.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Security;

namespace DemoAdminLTE.Controllers
{
    public class RapidoController : ApiController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly DemoContext db = new DemoContext();
        private const string KEY_FILE = "env-tracking-firebase-adminsdk-rgbq9-c4e39873da.json";
        private const string TOPIC_ENV = "env_alerts";
        private const string ENV_MESSAGE_TEMP = "Tại trạm {0} có kết quả bất thường. Vui lòng kiểm tra để biết thêm chi tiết. {1}";
        private const string TOPIC_SMS = "sms_alerts";
        private const string SMS_MESSAGE_TEMP = "Tại trạm {0} có kết quả bất thường. {1}";

        // GET/POST: api/rapido/push?station_id=1&sensors[0].id=1&sensors[0].value=3&sensors[1].id=2&sensors[1].value=5
        // content-type: application/json
        [HttpGet]
        [HttpPost]
        [ActionName("push")]
        public async Task<ApiRapidoResult> PushAsync([FromUri] ApiRapidoStationPushView data)
        {
            if (data == null)
                return ApiRapidoResult.ResultBadRequest("data is null");
            var station = db.Stations.Find(data.station_id);
            if (station == null)
                return ApiRapidoResult.ResultNotFound("station_id is not found");
            if (data.Sensors == null || data.Sensors.Count == 0)
                return ApiRapidoResult.ResultBadRequest("sensor list is empty");
            if (!data.Sensors.Select(s => s.id).ToList().SequenceEqual(station.Sensors.Select(s => s.Id).ToList()))
                return ApiRapidoResult.ResultBadRequest("sensor list does not match database");
            var sampleTime = new SampleTime();
            sampleTime.StationId = data.station_id;
            db.SampleTimes.Add(sampleTime);
            db.SaveChanges();
            var sensorValueList = new List<SensorValue>();
            foreach (ApiRapidoStationPushSensor sensor in data.Sensors)
                sensorValueList.Add(new SensorValue()
                {
                    SensorId = sensor.id,
                    SampleTimeId = sampleTime.Id,
                    Value = sensor.value
                });
            db.SensorValues.AddRange(sensorValueList);
            db.SaveChanges();
            bool flag = false;
            string rMsg = "";
            foreach (var sensor1 in data.Sensors)
            {
                var sensorValue = sensor1;
                var sensor2 = db.Sensors.FirstOrDefault(o => o.Id == sensorValue.id);
                if (sensor2 != null)
                {
                    double? nullable = sensor2.UpperBound;
                    double num1 = nullable ?? 0.0;
                    nullable = sensor2.LowerBound;
                    double num2 = nullable ?? 0.0;
                    nullable = sensor2.DifferBound;
                    double num3 = nullable ?? 0.0;
                    double num4 = num2;
                    if (Math.Abs((num1 + num4) / 2.0 - sensorValue.value) * num3 > 20.0)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                if (InitGoogleCredential(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, KEY_FILE), out rMsg))
                {
                    try
                    {
                        string str1 = await SendNotification(TOPIC_ENV, "Rapido Thông báo", string.Format(ENV_MESSAGE_TEMP, station.Name, sampleTime.TimeStringDisplay), true);
                        if (station.IsSmsAlertEnable)
                        {
                            if (!string.IsNullOrEmpty(station.SmsAlertPhoneNumber))
                            {
                                string str2 = await SendNotification(TOPIC_SMS, station.SmsAlertPhoneNumber, string.Format(SMS_MESSAGE_TEMP, station.Name, sampleTime.TimeStringDisplay), true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rMsg += ex.Message;
                    }
                }
            }
            return ApiRapidoResult.ResultSuccess("OK " + sampleTime.Time.ToEpochTime().ToString() + ". " + rMsg);
        }


        // POST: api/rapido/user-register
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "username": "",
        //          "firstname": "",
        //          "lastname": "",
        //          "phone": "",
        //          "email": "",
        //          "password": ""
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("user-register")]
        public ApiRapidoResult UserRegister([FromUri] ApiRapidoRegistrationView data)
        {
            if (data == null)
            {
                return ApiRapidoResult.ResultBadRequest("data is null");
            }

            if (ModelState.IsValid)
            {
                // Email Verification  
                MembershipUser membershipUser = Membership.GetUser(data.Username);
                if (membershipUser != null)
                {
                    return ApiRapidoResult.ResultBadRequest("username is already exists");
                }

                string userNameByPhone = CustomMembership.GetUserNameByPhonenumber(data.Phone);
                if (!string.IsNullOrEmpty(userNameByPhone))
                {
                    return ApiRapidoResult.ResultBadRequest("phone number is already exists");
                }

                string userName = Membership.GetUserNameByEmail(data.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    return ApiRapidoResult.ResultBadRequest("email address is already exists");
                }

                //Save User Data   
                using (DemoContext dbContext = new DemoContext())
                {
                    var defaultRole = dbContext.Roles.FirstOrDefault(o => o.RoleName == "User");
                    var defaultRoleId = defaultRole != null ? defaultRole.Id : 2;

                    var user = new User()
                    {
                        Username = data.Username,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Phone = data.Phone,
                        Email = data.Email,
                        PasswordHash = Crypto.HashPassword(data.Password),
                        ActivationCode = Guid.NewGuid(),
                        IsApproved = true,
                        RoleId = defaultRoleId,
                        Comment = data.DeviceToken ?? ""
                    };

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    Log.ToDatabase(user.Id, "Registration", "New Registration from mobile application");


                    return ApiRapidoResult.ResultSuccess("OK. UserId=" + user.Id.ToString());
                }
            }

            string errorString = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage + " ";
                }
            }
            return ApiRapidoResult.ResultBadRequest("Error: " + errorString);

        }



        // POST: api/rapido/user-login
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "username": "",
        //          "password": ""
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("user-login")]
        public ApiRapidoResult UserLogin([FromUri] ApiRapidoLoginView data)
        {
            if (data == null)
            {
                return ApiRapidoResult.ResultBadRequest("data is null");
            }

            if (ModelState.IsValid)
            {
                var isValidUser = Membership.ValidateUser(data.Username, data.Password);

                if (isValidUser)
                {
                    using (DemoContext dbContext = new DemoContext())
                    {
                        var dbUser = dbContext.Users.FirstOrDefault(u => u.Username == data.Username);
                        if (dbUser != null)
                        {
                            if (dbUser.IsApproved && !dbUser.IsLockedOut)
                            {
                                dbUser.LastLoginDate = DateTime.Now;
                                dbUser.LastActivityDate = DateTime.Now;
                                dbUser.ActivationCode = Guid.NewGuid();
                                dbUser.Comment = data.DeviceToken ?? "";
                                dbContext.Entry(dbUser).State = EntityState.Modified;
                                dbContext.SaveChanges();

                                Log.ToDatabase(dbUser.Id, "Login", "User login by mobile application");

                                return ApiRapidoResultWithData.ResultSuccess("OK. Login success.", new ApiRapidoLoginResult(dbUser.Id, dbUser.ActivationCode.ToString()));
                            }
                            else
                            {
                                return ApiRapidoResult.ResultBadRequest("user is not approved or is locked");
                            }
                        }
                    }

                }

                return ApiRapidoResult.ResultBadRequest("username or password is not correct");
            }

            string errorString = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage + " ";
                }
            }
            return ApiRapidoResult.ResultBadRequest("Error: " + errorString);

        }

        // POST: api/rapido/station-list
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "user_id"   : ,
        //          "access_key": ""
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("station-list")]
        public ApiRapidoResult StationList([FromUri] ApiRapidoLoginResult data)
        {
            if (data == null)
            {
                return ApiRapidoResult.ResultBadRequest("data is null");
            }

            if (ModelState.IsValid)
            {
                using (DemoContext dbContext = new DemoContext())
                {
                    try
                    {
                        Guid guid = new Guid(data.access_key);

                        var dbUser = dbContext.Users.FirstOrDefault(o => o.Id == data.user_id && o.ActivationCode == guid);

                        if (dbUser == null)
                            return ApiRapidoResult.ResultBadRequest("user is not logged in");

                        if (!dbUser.LastLoginDate.HasValue || dbUser.LastLoginDate.Value.AddDays(30) < DateTime.Now)
                            return ApiRapidoResult.ResultBadRequest("login expired");

                        if (!dbUser.IsApproved || dbUser.IsLockedOut)
                            return ApiRapidoResult.ResultBadRequest("user is not approved or locked");

                        IList<ApiRapidoStationView> stations = dbContext.Stations.Where(o => o.IsActive).Select(s => new ApiRapidoStationView
                        {
                            station_id = s.Id,
                            name = s.Name,
                            address = s.Address,
                            latitude = s.Latitude.ToString(),
                            longitude = s.Longitude.ToString(),
                            sensors = s.Sensors.Where(ss => ss.IsActive).Select(sss => new ApiRapidoStationSensor { sensor_id = sss.Id, name = sss.Name }).ToList()
                        }).ToList();

                        return ApiRapidoResultWithData.ResultSuccess("Get station list successfully", stations);

                    }
                    catch (Exception e)
                    {
                        return ApiRapidoResult.ResultBadRequest("Error: " + e.Message);
                    }
                }
            }

            string errorString = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage + " ";
                }
            }
            return ApiRapidoResult.ResultBadRequest("Error: " + errorString);

        }



        // POST: api/rapido/station-data
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "user_id"   : (int),
        //          "access_key": (string),
        //          "station_id": (int)
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("station-data")]
        public ApiRapidoResult StationData([FromUri] ApiRapidoStationDataView data)
        {
            if (data == null)
            {
                return ApiRapidoResult.ResultBadRequest("data is null");
            }

            if (ModelState.IsValid)
            {
                if (data.data_count <= 0)
                    data.data_count = 0;
                if (data.data_count > 10000)
                    data.data_count = 10000;
                using (DemoContext dbContext = new DemoContext())
                {
                    try
                    {
                        Guid guid = new Guid(data.access_key);

                        var dbUser = dbContext.Users.FirstOrDefault(o => o.Id == data.user_id && o.ActivationCode == guid);

                        if (dbUser == null)
                            return ApiRapidoResult.ResultBadRequest("user is not logged in");

                        if (!dbUser.LastLoginDate.HasValue || dbUser.LastLoginDate.Value.AddDays(30) < DateTime.Now)
                            return ApiRapidoResult.ResultBadRequest("login expired");

                        if (!dbUser.IsApproved || dbUser.IsLockedOut)
                            return ApiRapidoResult.ResultBadRequest("user is not approved or locked");


                        Station station = dbContext.Stations.FirstOrDefault(o => o.Id == data.station_id);

                        if (station == null)
                            return ApiRapidoResult.ResultBadRequest("station is not found");

                        if (!station.IsActive)
                            return ApiRapidoResult.ResultBadRequest("station is disabled");

                        if (station.Sensors.Count <= 0)
                            return ApiRapidoResult.ResultBadRequest("this station does have any sensor");


                        var sampleTimes = dbContext.SampleTimes.Where(o => o.StationId == data.station_id);
                        if (data.end_date.HasValue && data.end_date.HasValue)
                        {
                            sampleTimes = sampleTimes.Where(o => o.Time >= data.start_date.Value && o.Time <= data.end_date.Value);
                        }


                        IList<ApiRapidoStationDataTime> stationData = sampleTimes
                            .OrderByDescending(o => o.Time)
                            .Take(data.data_count)
                            .ToList()
                            .Select(s => new ApiRapidoStationDataTime
                            {
                                record_id = s.Id,
                                time = s.EpochTime,
                                sensor_value = s.SensorValues.Select(sv => new ApiRapidoStationDataValue
                                {
                                    sensor_id = sv.SensorId,
                                    sensor_name = sv.Sensor.Name,
                                    value = sv.Value
                                }).ToList()
                            }).ToList();

                        return ApiRapidoResultWithData.ResultSuccess("Get station data successfully", stationData);

                    }
                    catch (Exception e)
                    {
                        return ApiRapidoResult.ResultBadRequest("Error: " + e.Message);
                    }
                }
            }

            string errorString = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage + " ";
                }
            }
            return ApiRapidoResult.ResultBadRequest("Error: " + errorString);

        }



        // POST: api/rapido/sensor-list
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "user_id"   : ,
        //          "access_key": ""
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("sensor-list")]
        public ApiRapidoResult SensorList([FromUri] ApiRapidoLoginResult data)
        {
            if (data == null)
            {
                return ApiRapidoResult.ResultBadRequest("data is null");
            }

            if (ModelState.IsValid)
            {
                using (DemoContext dbContext = new DemoContext())
                {
                    try
                    {
                        Guid guid = new Guid(data.access_key);

                        var dbUser = dbContext.Users.FirstOrDefault(o => o.Id == data.user_id && o.ActivationCode == guid);

                        if (dbUser == null)
                            return ApiRapidoResult.ResultBadRequest("user is not logged in");

                        if (!dbUser.LastLoginDate.HasValue || dbUser.LastLoginDate.Value.AddDays(30) < DateTime.Now)
                            return ApiRapidoResult.ResultBadRequest("login expired");

                        if (!dbUser.IsApproved || dbUser.IsLockedOut)
                            return ApiRapidoResult.ResultBadRequest("user is not approved or locked");

                        IList<ApiRapidoSensorView> sensors = dbContext.Sensors.Select(s => new ApiRapidoSensorView
                        {
                            sensor_id = s.Id,
                            name = s.Name,
                            is_active = s.IsActive,
                            upper_bound = s.UpperBound ?? 0,
                            lower_bound = s.LowerBound ?? 0,
                            differ_bound = s.DifferBound ?? 0,
                            unit_symbol = s.UnitSymbol
                        }).ToList();

                        return ApiRapidoResultWithData.ResultSuccess("Get sensor list successfully", sensors);

                    }
                    catch (Exception e)
                    {
                        return ApiRapidoResult.ResultBadRequest("Error: " + e.Message);
                    }
                }
            }

            string errorString = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage + " ";
                }
            }
            return ApiRapidoResult.ResultBadRequest("Error: " + errorString);

        }


        private bool InitGoogleCredential(string keyFilePath, out string msg)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(keyFilePath)
                    });
                    msg = "FirebaseApp " + firebaseApp.Name + " created.";
                }
                else
                    msg = "FirebaseApp " + FirebaseApp.DefaultInstance.Name + " existed.";
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message + " " + keyFilePath;
                return false;
            }
        }


        private async Task<string> SendNotification(
          string dist,
          string title,
          string body,
          bool isBroadcast = false)
        {
            Notification notification = new Notification()
            {
                Title = title,
                Body = body
            };
            Message message;
            if (isBroadcast)
                message = new Message()
                {
                    Notification = notification,
                    Topic = dist
                };
            else
                message = new Message()
                {
                    Notification = notification,
                    Token = dist
                };
            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }


        [HttpGet]
        [HttpPost]
        [ActionName("test-push-notification")]
        public async Task<ApiRapidoResult> TestPushNotificationAsync(string devicetoken = "")
        {
            string keyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, KEY_FILE);
            bool isBroadcast = string.IsNullOrEmpty(devicetoken);
            string rMsg;
            if (!this.InitGoogleCredential(keyFilePath, out rMsg))
                return ApiRapidoResult.ResultBadRequest(rMsg);
            try
            {
                string str = await this.SendNotification(isBroadcast ? TOPIC_ENV : devicetoken, "Xin chào!", "Đây là tin nhắn thử nghiệm " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), isBroadcast);
                return ApiRapidoResult.ResultSuccess(rMsg);
            }
            catch (Exception ex)
            {
                return ApiRapidoResult.ResultBadRequest("Push notification error. " + rMsg + ". " + ex.Message);
            }
        }


        [HttpGet]
        [HttpPost]
        [ActionName("test-push-sms")]
        public async Task<ApiRapidoResult> TestPushSMSAsync(string phonenumber = "")
        {
            string keyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, KEY_FILE);
            if (string.IsNullOrEmpty(phonenumber))
                return ApiRapidoResult.ResultBadRequest("Missing 'phonenumber' parameter.");
            string rMsg;
            if (!this.InitGoogleCredential(keyFilePath, out rMsg))
                return ApiRapidoResult.ResultBadRequest(rMsg);
            try
            {
                string str = await this.SendNotification(TOPIC_SMS, phonenumber, "Đây là tin nhắn thử nghiệm SMS. " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), true);
                return ApiRapidoResult.ResultSuccess(rMsg);
            }
            catch (Exception ex)
            {
                return ApiRapidoResult.ResultBadRequest("Push notification error. " + rMsg + ". " + ex.Message);
            }
        }

    }
}
