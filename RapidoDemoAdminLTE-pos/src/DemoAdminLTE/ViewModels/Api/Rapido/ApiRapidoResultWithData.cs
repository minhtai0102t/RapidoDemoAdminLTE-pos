namespace DemoAdminLTE.ViewModels
{
    public class ApiRapidoResultWithData : ApiRapidoResult
    {
        public object data { get; set; }

        public static ApiRapidoResultWithData ResultSuccess(string message, object data)
        {
            return new ApiRapidoResultWithData { code = CODE_OK, status = STATUS_SUCCESS, message = message, data = data };
        }
    }
}
