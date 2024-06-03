using DemoAdminLTE.Resources.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAdminLTE.Models
{
    public class DeviceProduct // : BaseModel
    {
        [ForeignKey("Device")]
        [Key, Column(Order = 0)]
        public virtual int DeviceId { get; set; }
        public virtual Device Device { get; set; }


        [ForeignKey("Product")]
        [Key, Column(Order = 1)]
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public long Quantity { get; set; }

        public DateTime LastUpdate { get; set; }
        public string Note { get; set; }


        /* BaseModel */
        [Display(ResourceType = typeof(BaseModelTitles), Name = "CreationDate")]
        public virtual DateTime CreationDate
        {
            get
            {
                if (!IsCreationDateSet)
                    CreationDate = DateTime.Now;

                return InternalCreationDate;
            }
            protected set
            {
                IsCreationDateSet = true;
                InternalCreationDate = value;
            }
        }
        private bool IsCreationDateSet { get; set; }
        private DateTime InternalCreationDate { get; set; }
    }
}