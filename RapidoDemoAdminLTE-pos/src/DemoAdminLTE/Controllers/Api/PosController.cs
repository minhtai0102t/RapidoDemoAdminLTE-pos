using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.Models;
using DemoAdminLTE.ViewModels;
using NLog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Security;

namespace DemoAdminLTE.Controllers
{
    public class PosController : ApiController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly DemoContext db = new DemoContext();

        /*
         * GET      (SELECT)
         * POST     (CREATE)
         * PUT      (UPDATE)
         * DELETE   (DELETE)
         */

        // POST: api/pos/transaction?serial_number=123&product_id=1&quantity=10
        // content-type: application/json
        [HttpPost]
        [ActionName("transaction")]
        public ApiPosResult PostTransaction(ApiPosTransactionView data)
        {
            if (data == null)
                return ApiPosResult.ResultBadRequest("data is null");

            //var device = db.Devices.FirstOrDefault(o => o.SerialNumber == data.serial_number);
            //if (device == null)
            //    return ApiPosResult.ResultNotFound("device is not found");

            //var product = db.Products.Find(data.product_id);
            //if (product == null)
            //    return ApiPosResult.ResultNotFound("product is not found");

            //var transaction = new Transaction();

            //transaction.DeviceId = device.Id;
            //transaction.ProductId = data.product_id;

            ///* transaction infos */
            //transaction.Quantity = data.quantity;
            //transaction.UnitPrice = product.UnitPrice;
            //transaction.CurrencyUnit = product.CurrencyUnit;
            //transaction.UnitType = product.UnitType;

            ///* device infos backup at the time of transaction */
            //transaction.DeviceName = device.Name;
            //transaction.DeviceSerialNumber = device.SerialNumber;
            //transaction.DeviceTypeName = device.DeviceType.Name;
            //transaction.DeviceLocation = device.Location;
            //transaction.DeviceLatitude = device.Latitude;
            //transaction.DeviceLongitude = device.Longitude;
            //transaction.DeviceHardwareVersion = device.HardwareVersion;
            //transaction.DeviceSoftwareVersion = device.SoftwareVersion;

            ///* product infos backup at the time of transaction */
            //transaction.ProductName = product.Name;
            //transaction.ProductSKU = product.SKU;
            //transaction.CategoryName = product.Category.Name;

            //db.Transactions.Add(transaction);
            //db.SaveChanges();


            if (data.quantity <= 0)
            {
                return ApiPosResult.ResultBadRequest("quantity is not valid");
            }

            var device = db.Devices.FirstOrDefault(o => o.SerialNumber == data.serial_number);
            if (device == null)
            {
                return ApiPosResult.ResultNotFound("device is not found");
            }

            var product = db.Products.FirstOrDefault(o => o.Id == data.product_id);
            if (product == null)
            {
                return ApiPosResult.ResultNotFound("product is not found");
            }

            var deviceProduct = db.DeviceProducts.FirstOrDefault(o => o.DeviceId == device.Id && o.ProductId == product.Id);
            if (deviceProduct == null)
            {
                return ApiPosResult.ResultNotFound("the product is not set up in this device");
            }

            if (data.quantity > deviceProduct.Quantity)
            {
                return ApiPosResult.ResultBadRequest("Not enough inventory");
            }

            /* 
             * add new transaction 
             */

            var transaction = new Transaction();

            transaction.DeviceId = device.Id;
            transaction.ProductId = product.Id;
            transaction.Quantity = data.quantity;

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

            transaction.Note = "api";

            db.Transactions.Add(transaction);
            db.SaveChanges();
            //Log.ToDatabase(((CustomPrincipal)User).UserId, "Create", string.Format("Create new transaction '{0}'", transaction.Id));

            /* 
             * update stock 
             */

            deviceProduct.Quantity -= transaction.Quantity;
            deviceProduct.LastUpdate = DateTime.Now;

            db.Entry(deviceProduct).State = EntityState.Modified;
            db.SaveChanges();

            return ApiPosResult.ResultSuccess("transaction created successfully at " + transaction.CreationDate.ToString());
        }

        // POST: api/pos/login
        // Header:
        //      content-type: application/json
        // Body: 
        //        {
        //          "username": "",
        //          "password": ""
        //       }
        [HttpGet]
        [HttpPost]
        [ActionName("login")]
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

    }
}
