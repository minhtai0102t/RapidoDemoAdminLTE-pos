using System;
using System.Net;
using System.Web;
using System.Xml;

namespace DemoAdminLTE.Extensions
{
    public class FiboSMS
    {
        //http://center.fibosms.com/service.asmx/SendMaskedSMS?clientNo=string&clientPass=string&senderName=string&phoneNumber=string&smsMessage=string&smsGUID=0&serviceType=0
        /*
         * http://center.fibosms.com/service.asmx/
         * SendMaskedSMS?
         * clientNo=string&
         * clientPass=string&
         * senderName=string&
         * phoneNumber=string&
         * smsMessage=string&
         * smsGUID=0&
         * serviceType=0
         */
        public string ApiUrl { get; set; }
        public string ApiName { get; set; }
        public string ClientNo { get; set; }
        public string ClientPass { get; set; }
        public string SenderName { get; set; }

        //public string PhoneNumber { get; set; }
        //public string SmsMessage { get; set; }
        public int SmsGUID { get; set; }

        public int ServiceType { get; set; }

        public FiboSMS()
        {
            ApiUrl = "http://center.fibosms.com/service.asmx/";
            SenderName = "LONGCODE";
            SmsGUID = 0;
            ServiceType = 0;
        }

        public FiboSMS(string clientNo, string clientPass) : this()
        {
            ClientNo = clientNo;
            ClientPass = clientPass;
        }

        public SmsResult sendSMS(string phoneNumber, string smsMessage, string messageTemplate)
        {
            ApiName = "SendMaskedSMS";

            string url = ApiUrl + ApiName + "?"
                        + "clientNo=" + HttpUtility.UrlEncode(ClientNo)
                        + "&clientPass=" + HttpUtility.UrlEncode(ClientPass)
                        + "&senderName=" + HttpUtility.UrlEncode(SenderName)
                        + "&phoneNumber=" + HttpUtility.UrlEncode(phoneNumber)
                        + "&smsMessage=" + HttpUtility.UrlEncode(messageTemplate + smsMessage)
                        + "&smsGUID=" + HttpUtility.UrlEncode(SmsGUID.ToString())
                        + "&serviceType=" + HttpUtility.UrlEncode(ServiceType.ToString());

            // Get response
            string response = "";
            using (WebClient wlc = new WebClient())
            {
                try
                {
                    response = wlc.DownloadString(url);
                }
                catch (WebException ex)
                {
                    throw new WebException("Error occurred while connecting to server. " + ex.Message, ex);
                }
            }

            try
            {
                // Parse result
                XmlDocument doc = new XmlDocument();
                response = HttpUtility.HtmlDecode(response);
                doc.LoadXml(response);

                XmlNode codeNode = doc.GetElementsByTagName("Code").Item(0);
                XmlNode messageNode = doc.GetElementsByTagName("Message").Item(0);
                XmlNode timeNode = doc.GetElementsByTagName("Time").Item(0);

                SmsResult result = new SmsResult();
                result.Code = (codeNode == null) ? 0 : int.Parse(codeNode.InnerText);
                result.Message = (messageNode == null) ? "" : messageNode.InnerText;
                result.Time = (timeNode == null) ? DateTime.Now.ToString() : timeNode.InnerText;
                result.Success = (result.Code == 200);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while parse result. " + ex.Message, ex);
            }
        }
    }
}