using System.Threading;
using System.Configuration;
using System.Web.Configuration;
using System.Globalization;

namespace DemoAdminLTE.Extensions
{
    public static class LanguageHelper
    {

        public static string CurrentLanguageCode
        {
            get
            {
                //return Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);
                return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }

        public static string DefaultLanguageCode
        {
            get
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
                GlobalizationSection section = (GlobalizationSection)config.GetSection("system.web/globalization");

                try
                {
                    CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(section.UICulture);
                    return cultureInfo.TwoLetterISOLanguageName;
                }
                catch
                {

                }
                return "en";
            }
        }

    }
}
