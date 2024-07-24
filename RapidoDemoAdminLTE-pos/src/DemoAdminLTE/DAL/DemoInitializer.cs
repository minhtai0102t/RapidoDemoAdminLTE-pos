using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using DemoAdminLTE.Models;

namespace DemoAdminLTE.DAL
{
    public class DemoInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<DemoContext>
    {
        protected override void Seed(DemoContext context)
        {
            // permission
            var user_permissions = new List<Permission> {

                /* Rapido */
                new Permission { Group = "Home"       , Action = "Index"},
                new Permission { Group = "Station"    , Action = "List"},
                new Permission { Group = "Station"    , Action = "Create"},
                new Permission { Group = "Station"    , Action = "Edit"},
                new Permission { Group = "Station"    , Action = "Chart"},

                new Permission { Group = "Sensor"     , Action = "List"},
                new Permission { Group = "Sensor"     , Action = "Create"},
                new Permission { Group = "Sensor"     , Action = "Edit"},

                /* Pos */
                new Permission { Group = "Device"     , Action = "List"},
                new Permission { Group = "Device"     , Action = "Create"},
                new Permission { Group = "Device"     , Action = "Edit"},

                new Permission { Group = "DeviceType" , Action = "List"},
                new Permission { Group = "DeviceType" , Action = "Create"},
                new Permission { Group = "DeviceType" , Action = "Edit"},

                new Permission { Group = "Category"   , Action = "List"},
                new Permission { Group = "Category"   , Action = "Create"},
                new Permission { Group = "Category"   , Action = "Edit"},

                new Permission { Group = "Product"    , Action = "List"},
                new Permission { Group = "Product"    , Action = "Create"},
                new Permission { Group = "Product"    , Action = "Edit"},

                new Permission { Group = "Transaction", Action = "List"},
                new Permission { Group = "Transaction", Action = "Create"},
                new Permission { Group = "Transaction", Action = "Edit"},

            };
            var admin_permissions = new List<Permission> {
                new Permission { Group = "Station"    , Action = "Delete"},
                new Permission { Group = "Sensor"     , Action = "Delete"},

                new Permission { Group = "Device"     , Action = "Delete"},
                new Permission { Group = "DeviceType" , Action = "Delete"},
                new Permission { Group = "Category"   , Action = "Delete"},
                new Permission { Group = "Product"    , Action = "Delete"},
                new Permission { Group = "Transaction", Action = "Delete"},

                new Permission { Group = "User"       , Action = "List"},
                new Permission { Group = "User"       , Action = "Create"},
                new Permission { Group = "User"       , Action = "Edit"},
                new Permission { Group = "User"       , Action = "Delete"},

                new Permission { Group = "ActivityLog", Action = "List"},

                new Permission { Group = "Role"       , Action = "List"},
                new Permission { Group = "Role"       , Action = "Create"},
                new Permission { Group = "Role"       , Action = "Edit"},
                new Permission { Group = "Role"       , Action = "Delete"},
            };
            user_permissions.ForEach(obj => context.Permissions.Add(obj));
            admin_permissions.ForEach(obj => context.Permissions.Add(obj));
            context.SaveChanges();

            // roles
            Role roleAdmin = new Role { Id = 1, RoleName = "Admin", Permissions = user_permissions.Concat(admin_permissions).ToList() };
            Role roleUser = new Role { Id = 2, RoleName = "User", Permissions = user_permissions };
            var roles = new List<Role> { roleAdmin, roleUser };
            roles.ForEach(obj => context.Roles.Add(obj));
            context.SaveChanges();

            // users
            var users = new List<User>
            {
                new User { Id = 1, Email = "admin@admin.com", Phone = "0912345678", FirstName = "Super",  LastName = "Admin", Username = "admin", PasswordHash = Crypto.HashPassword("admin@1123"), IsApproved = true, ActivationCode = Guid.NewGuid(), Role = roleAdmin },
                new User { Id = 2, Email = "user@user.com",   Phone = "0888888888", FirstName = "Normal", LastName = "User",  Username = "user",  PasswordHash = Crypto.HashPassword("user@1234"),  IsApproved = true, ActivationCode = Guid.NewGuid(), Role = roleUser },
            };
            users.ForEach(obj => context.Users.Add(obj));
            context.SaveChanges();

            // stations
            var stations = new List<Station>
            {
                new Station { Name = "Xuân Đài", Address = "Tx. Sông Cầu, Phú Yên, Việt Nam", Latitude = 13.465017, Longitude = 109.2359723, IsActive = true },
                new Station { Name = "Vũng Lắm", Address = "Tx. Sông Cầu, Phú Yên, Việt Nam", Latitude = 13.400145, Longitude = 109.2363500, IsActive = true },
            };
            stations.ForEach(obj => context.Stations.Add(obj));
            context.SaveChanges();

            // sensors
            var sensors = new List<Sensor>
            {
                new Sensor { name = "Nhiệt độ", upper_bound = 50, lower_bound = 10, differ_bound = 5 , unit_symbol = "°C", param = "", is_active = true },
                new Sensor { name = "Độ ẩm"   , upper_bound = 90, lower_bound = 30, differ_bound = 10, unit_symbol = "%" , param = "", is_active = true },
                new Sensor { name = "Độ mặn"  , upper_bound = 40, lower_bound = 20, differ_bound = 5 , unit_symbol = "‰" , param = "", is_active = true },
                new Sensor { name = "Độ pH"   , upper_bound = 9 , lower_bound = 5 , differ_bound = 2 , unit_symbol = ""  , param = "", is_active = true },
            };
            sensors.ForEach(obj => context.Sensors.Add(obj));
            context.SaveChanges();

            // device type
            var deviceTypes = new List<DeviceType> {
                new DeviceType { Id = 1, Name = "Default", Status = true },
            };
            deviceTypes.ForEach(obj => context.DeviceTypes.Add(obj));
            context.SaveChanges();

            // device
            var devices = new List<Device>
            {
                new Device { Id = 1, Name = "Default", Status = true, DeviceTypeId = 1, Activated = true, HardwareVersion = "1.0.0", SoftwareVersion = "1.0.0", SerialNumber = "123456789" , Latitude = 0, Longitude = 0, Location = "Việt Nam", Online = true, PinCode = "1111", PinCodeGenerationTime = DateTime.Now, Password = "1234" , Description = "" },
            };
            devices.ForEach(obj => context.Devices.Add(obj));
            context.SaveChanges();

            // category
            var categories = new List<Category>
            {
                new Category { Id = 1, Name= "Default", Status = true },
            };
            categories.ForEach(obj => context.Categories.Add(obj));
            context.SaveChanges();

            // product
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Card", Status = true, Description = "", CategoryId = 1, UnitPrice = 2000, CurrencyUnit = "VND", SKU = "001", UnitType = "cái", LowQuantityThreshold = 10 },
            };
            products.ForEach(obj => context.Products.Add(obj));
            context.SaveChanges();


            // device-product
            var deviceProducts = new List<DeviceProduct>
            {
                new DeviceProduct { DeviceId = 1, ProductId = 1, Quantity = 1000 },
            };
            deviceProducts.ForEach(obj => context.DeviceProducts.Add(obj));
            context.SaveChanges();
        }
    }
}
