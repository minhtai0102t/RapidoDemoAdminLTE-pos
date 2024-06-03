// Decompiled with JetBrains decompiler
// Type: DemoAdminLTE.ViewModels.SensorValueViewModel
// Assembly: DemoAdminLTE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 771896B4-5B09-4A8A-B2F6-33E1EA9470A2
// Assembly location: C:\Users\thanh\Downloads\webserver_backup\hst6.win.npnlab.com_103.28.39.47\rapido.npnlab.com\bin\DemoAdminLTE.dll

using DemoAdminLTE.Resources.Views.ValueViews;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class SensorValueViewModel
    {
        public int SensorId { get; set; }

        [Display(Name = "Value", ResourceType = typeof(Titles))]
        public double SensorValue { get; set; }

        public string SensorName { get; set; }
    }
}
