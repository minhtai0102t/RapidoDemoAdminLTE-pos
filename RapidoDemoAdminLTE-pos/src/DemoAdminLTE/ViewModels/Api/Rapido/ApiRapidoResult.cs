namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoResult
    {
        public string status { get; set; }
        public int code { get; set; }
        public string message { get; set; }

        public static int CODE_OK = 200;

        public static int CODE_BAD_REQUEST = 400;
        public static int CODE_FORBIDDEN = 403;
        public static int CODE_NOT_FOUND = 404;

        public static string STATUS_SUCCESS = "success";
        public static string STATUS_FAILURE = "failure";

        public static ApiRapidoResult ResultNotFound(string message = "Not Found")
        {
            return new ApiRapidoResult { code = CODE_NOT_FOUND, status = STATUS_FAILURE, message = message };
        }

        public static ApiRapidoResult ResultForbidden(string message = "Forbidden")
        {
            return new ApiRapidoResult { code = CODE_FORBIDDEN, status = STATUS_FAILURE, message = message };
        }

        public static ApiRapidoResult ResultBadRequest(string message = "Bad Request")
        {
            return new ApiRapidoResult { code = CODE_BAD_REQUEST, status = STATUS_FAILURE, message = message };
        }

        public static ApiRapidoResult ResultSuccess(string message = "OK")
        {
            return new ApiRapidoResult { code = CODE_OK, status = STATUS_SUCCESS, message = message };
        }
    }
}