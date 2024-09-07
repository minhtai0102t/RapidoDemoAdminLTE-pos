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
    public class AccountSignUpReq
    {
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}