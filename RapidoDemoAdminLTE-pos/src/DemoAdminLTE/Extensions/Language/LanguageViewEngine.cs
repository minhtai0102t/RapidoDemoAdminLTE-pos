using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DemoAdminLTE.Extensions
{
    public class LanguageViewEngine : RazorViewEngine
    {
        public LanguageViewEngine() : this(LanguageHelper.CurrentLanguageCode)
        {
        }

        public LanguageViewEngine(string lang)
        {
            SetCurrentCulture(lang);
        }

        public void SetCurrentCulture(string lang)
        {
            CurrentCulture = lang;
            ICollection<string> arViewLocationFormats =
                new string[] { "~/Views/{1}/" + lang + "/{0}.cshtml" };
            ICollection<string> arBaseViewLocationFormats = new string[] {
                @"~/Views/{1}/{0}.cshtml",
                @"~/Views/Shared/{0}.cshtml"};
            this.ViewLocationFormats = arViewLocationFormats.Concat(arBaseViewLocationFormats).ToArray();
        }

        public static string CurrentCulture { get; private set; } = LanguageHelper.CurrentLanguageCode;
    }
}