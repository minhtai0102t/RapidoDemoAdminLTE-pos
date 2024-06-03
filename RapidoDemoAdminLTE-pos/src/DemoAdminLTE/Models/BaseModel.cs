using DemoAdminLTE.Resources.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.Models
{
    public abstract class BaseModel
    {
        [Key]
        [Display(ResourceType = typeof(BaseModelTitles), Name = "Id")]
        public virtual int Id { get; set; }

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
