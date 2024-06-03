// Decompiled with JetBrains decompiler
// Type: DemoAdminLTE.ViewModels.StationValueCreateViewModel
// Assembly: DemoAdminLTE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 771896B4-5B09-4A8A-B2F6-33E1EA9470A2
// Assembly location: C:\Users\thanh\Downloads\webserver_backup\hst6.win.npnlab.com_103.28.39.47\rapido.npnlab.com\bin\DemoAdminLTE.dll

using DemoAdminLTE.Resources.Views.ValueViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class StationValueCreateViewModel
    {
        [Display(Name = "StationName", ResourceType = typeof(Titles))]
        public int StationId { get; set; }

        [Display(Name = "Time", ResourceType = typeof(Titles))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? Time { get; set; }

        public IList<SensorValueViewModel> Sensors { get; set; }
    }
}
