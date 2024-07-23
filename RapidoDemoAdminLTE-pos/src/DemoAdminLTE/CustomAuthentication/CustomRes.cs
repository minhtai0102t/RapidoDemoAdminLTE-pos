using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DemoAdminLTE
{
    public class PagingResponse<T>
    {
        [JsonIgnore]
        public CRUDStatusCodeRes StatusCode { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; }

        public int TotalRecord { get; set; }

        public int? CurrentPageIndex { get; set; }

        public int? PageSize { get; set; }

        public ICollection<T> Records { get; set; }
    }
    public class UserSearchRes
    {
        public int total_record { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public bool is_approved { get; set; }
        public DateTime created_date { get; set; }
        public string password { get; set; }
    }
    public class RoleRes
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
        public int[] permission_ids { get; set; }
    }
    public class UserRes
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int role_id { get; set; }
        public bool is_approved { get; set; }
    }
    public class UserUpdateReq
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool is_approved { get; set; }
        public int role_id { get; set; }
    }
}