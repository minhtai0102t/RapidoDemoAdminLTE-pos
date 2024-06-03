using System.ComponentModel.DataAnnotations;

namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoLoginResult
    {
        [Required]
        public int user_id { get; set; }

        [Required]
        public string access_key { get; set; }

        public ApiRapidoLoginResult()
        {

        }

        public ApiRapidoLoginResult(int user_id, string access_key)
        {
            this.user_id = user_id;
            this.access_key = access_key;
        }

    }
}
