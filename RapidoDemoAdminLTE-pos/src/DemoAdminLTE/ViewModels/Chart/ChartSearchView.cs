using DemoAdminLTE.Resources.Views.ChartViews;
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
  public class ChartSearchView
  {
    public int Id { get; set; }

    [Display(Name = "From", ResourceType = typeof (Titles))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
    public DateTime? TimeFrom { get; set; }

    [Display(Name = "To", ResourceType = typeof (Titles))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
    public DateTime? TimeTo { get; set; }
  }
}
