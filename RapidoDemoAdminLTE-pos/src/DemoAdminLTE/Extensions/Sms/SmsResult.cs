namespace DemoAdminLTE.Extensions
{
    public class SmsResult
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }

        public SmsResult()
        {
            Success = false;
            Code = -1;
            Message = "";
            Time = "";
        }

        public static string GetErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 1: return "Thành công";
                case 2: return "Lỗi hệ thống";
                case 3: return "Sai user hoặc mật khẩu";
                case 4: return "IP không được phép";
                case 5: return "Chưa khai báo brandname/dịch vụ";
                case 6: return "Lặp nội dung";
                case 7: return "Thuê bao từ chối nhận tin";
                case 8: return "Không được phép gửi tin";
                case 9: return "Chưa khai báo template";
                case 10: return "Định dạng thuê bao không đúng";
                case 11: return "Có tham số không hợp lệ";
                case 12: return "Tài khoản không đúng";
                case 13: return "Gửi tin: lỗi kết nối";
                case 14: return "Tài khoản không đủ (tiền)";
                case 15: return "Tài khoản hết hạn";
                case 16: return "Hết hạn dịch vụ";
                case 17: return "Hết hạn mức gửi test";
                case 18: return "Hủy gửi tin (CSKH)";
                case 19: return "Hủy gửi tin (KD)";
                case 20: return "Gateway chưa hỗ trợ Unicode";
                case 21: return "Chưa set giá trả trước";
                case 22: return "Tài khoản chưa kích hoạt";
                case 23: return "Sai cú pháp tin nhắn";
                case 24: return "Thủ tục xử lý bị lỗi";
                case 25: return "Chưa khai báo partner cho user";
                case 26: return "Chưa khai báo GateOwner cho user";
                case 27: return "Gửi tin: gate trả mã lỗi";
                case 28: return "Hủy gửi tin";
                default: return "Lỗi không xác định";
            }
        }
    }
}