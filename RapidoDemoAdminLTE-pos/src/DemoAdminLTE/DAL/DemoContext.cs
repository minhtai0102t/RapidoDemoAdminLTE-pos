using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DemoAdminLTE.Models;

namespace DemoAdminLTE.DAL
{
    public class DemoContext : DbContext
    {
        public DemoContext()
            : base("DemoContext")
        {
        }

        /* Rapido */
        public DbSet<Station> Stations { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorValue> SensorValues { get; set; }
        public DbSet<SampleTime> SampleTimes { get; set; }

        /* System */
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasMany(u => u.Permissions)
                .WithMany(r => r.Roles)
                .Map(m =>
                {
                    m.ToTable("RolePermissions");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("PermissionId");
                });

            modelBuilder.Entity<Station>()
                .HasMany(u => u.Sensors)
                .WithMany(r => r.Stations)
                .Map(m =>
                {
                    m.ToTable("StationSensors");
                    m.MapLeftKey("StationId");
                    m.MapRightKey("SensorId");
                });

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(config => config.HasColumnType("datetime2"));
        }
    }
}