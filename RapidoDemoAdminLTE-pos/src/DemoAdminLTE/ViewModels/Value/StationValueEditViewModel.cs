// Decompiled with JetBrains decompiler
// Type: DemoAdminLTE.ViewModels.StationValueEditViewModel
// Assembly: DemoAdminLTE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 771896B4-5B09-4A8A-B2F6-33E1EA9470A2
// Assembly location: C:\Users\thanh\Downloads\webserver_backup\hst6.win.npnlab.com_103.28.39.47\rapido.npnlab.com\bin\DemoAdminLTE.dll

using DemoAdminLTE.Resources.Views.ValueViews;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class StationValueEditViewModel
    {
        [Display(Name = "SampleTime", ResourceType = typeof(Titles))]
        public int SampleTimeId { get; set; }

        [Display(Name = "StationName", ResourceType = typeof(Titles))]
        public int StationId { get; set; }

        public IList<SensorValueViewModel> Sensors { get; set; }
    }
}
