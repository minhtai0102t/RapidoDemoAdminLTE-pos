using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoAdminLTE
{
    public class UserSearchReq 
    {
        public string keysearch { get; set; }
        public int page_size { get; set; } = AppConfig.PageSizeDefaultValue;
        public int page_index { get; set; } = 0;
    }
    public class AccountSignInReq
    {
        public string user_name { get; set; }
        public string password { get; set; }
    }
}