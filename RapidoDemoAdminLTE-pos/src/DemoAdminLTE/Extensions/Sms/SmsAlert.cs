using System.Net;

namespace DemoAdminLTE.Extensions
{
    public class SmsAlert
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public SmsAlert()
        {
            Username = "CL1809200001";
            Password = "9QWRCd8MtDNYeMvj";
        }

        public SmsAlert(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public SmsResult Send(string phoneNumber, string smsMessage, string messageTemplate = "NPNLab thong bao: ")
        {
            FiboSMS fiboSMS = new FiboSMS(Username, Password);
            SmsResult result;
            try
            {
                result = fiboSMS.sendSMS(phoneNumber, smsMessage, messageTemplate);
                return result;
            }
            catch (WebException ex)
            {
                result = new SmsResult();
                result.Message = ex.Message;
                return result;
            }
        }
    }
}