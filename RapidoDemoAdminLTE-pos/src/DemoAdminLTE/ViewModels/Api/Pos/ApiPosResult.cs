namespace DemoAdminLTE.ViewModels
{
    public class ApiPosResult
    {
        public string status { get; set; }
        public int code { get; set; }
        public string message { get; set; }

        /*
         * Informational responses  (100 – 199)
         * Successful responses     (200 – 299)
         * Redirection messages     (300 – 399)
         * Client error responses   (400 – 499)
         * Server error responses   (500 – 599)
         */

        public static int HTTP_200_OK = 200;
        //public static int HTTP_201_CREATED = 201;
        //public static int HTTP_202_ACCEPTED = 202;
        //public static int HTTP_203_NON_AUTHORITATIVE_INFORMATION = 203;
        //public static int HTTP_204_NO_CONTENT = 204;
        //public static int HTTP_205_RESET_CONTENT = 205;
        //public static int HTTP_206_PARTIAL_CONTENT = 206;
        //public static int HTTP_207_MULTI_STATUS = 207;
        //public static int HTTP_208_ALREADY_REPORTED = 208;
        //public static int HTTP_226_IM_USED = 226;

        public static int HTTP_400_BAD_REQUEST = 400;
        public static int HTTP_401_UNAUTHORIZED = 401;
        public static int HTTP_402_PAYMENT_REQUIRED = 402;
        public static int HTTP_403_FORBIDDEN = 403;
        public static int HTTP_404_NOT_FOUND = 404;
        //public static int HTTP_405_METHOD_NOT_ALLOWED = 405;
        //public static int HTTP_406_NOT_ACCEPTABLE = 406;
        //public static int HTTP_407_PROXY_AUTHENTICATION_REQUIRED = 407;
        //public static int HTTP_408_REQUEST_TIMEOUT = 408;
        //public static int HTTP_409_CONFLICT = 409;
        //public static int HTTP_410_GONE = 410;
        //public static int HTTP_411_LENGTH_REQUIRED = 411;
        //public static int HTTP_412_PRECONDITION_FAILED = 412;
        //public static int HTTP_413_PAYLOAD_TOO_LARGE = 413;
        //public static int HTTP_414_URI_TOO_LONG = 414;
        //public static int HTTP_415_UNSUPPORTED_MEDIA_TYPE = 415;
        //public static int HTTP_416_RANGE_NOT_SATISFIABLE = 416;
        //public static int HTTP_417_EXPECTATION_FAILED = 417;
        //public static int HTTP_418_I_M_A_TEAPOT = 418;
        //public static int HTTP_421_MISDIRECTED_REQUEST = 421;
        //public static int HTTP_422_UNPROCESSABLE_CONTENT = 422;
        //public static int HTTP_423_LOCKED = 423;
        //public static int HTTP_424_FAILED_DEPENDENCY = 424;
        //public static int HTTP_425_TOO_EARLY = 425;
        //public static int HTTP_426_UPGRADE_REQUIRED = 426;
        //public static int HTTP_428_PRECONDITION_REQUIRED = 428;
        //public static int HTTP_429_TOO_MANY_REQUESTS = 429;
        //public static int HTTP_431_REQUEST_HEADER_FIELDS_TOO_LARGE = 431;
        //public static int HTTP_451_UNAVAILABLE_FOR_LEGAL_REASONS = 451;


        public static string STATUS_SUCCESS = "success";
        public static string STATUS_FAILURE = "failure";

        public static ApiPosResult ResultNotFound(string message = "Not Found")
        {
            return new ApiPosResult { code = HTTP_404_NOT_FOUND, status = STATUS_FAILURE, message = message };
        }

        public static ApiPosResult ResultForbidden(string message = "Forbidden")
        {
            return new ApiPosResult { code = HTTP_403_FORBIDDEN, status = STATUS_FAILURE, message = message };
        }

        public static ApiPosResult ResultBadRequest(string message = "Bad Request")
        {
            return new ApiPosResult { code = HTTP_400_BAD_REQUEST, status = STATUS_FAILURE, message = message };
        }

        public static ApiPosResult ResultSuccess(string message = "OK")
        {
            return new ApiPosResult { code = HTTP_200_OK, status = STATUS_SUCCESS, message = message };
        }
    }
}