using DemoAdminLTE.DAL;
using DemoAdminLTE.Resources.Views.ActivityLogViews;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAdminLTE.Models
{
    public class ActivityLog : BaseModel
    {
        public int UserId { get; set; }

        [StringLength(32)]
        [Display(ResourceType = typeof(Titles), Name = "Context")]
        public string Context { get; set; }

        [StringLength(16)]
        [Display(ResourceType = typeof(Titles), Name = "Action")]
        public string Action { get; set; }

        [StringLength(128)]
        [Display(ResourceType = typeof(Titles), Name = "Message")]
        public string Message { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Titles), Name = "Username")]
        public string Username
        {
            get
            {
                using (DemoContext dbContext = new DemoContext())
                {
                    var user = dbContext.Users.Find(UserId);
                    if (user != null)
                    {
                        return user.Username;
                    }
                }
                return "";
            }
        }
    }
}
