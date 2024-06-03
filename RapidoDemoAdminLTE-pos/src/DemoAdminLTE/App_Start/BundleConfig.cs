using System.Web.Optimization;
using WebHelpers.Mvc5;

namespace DemoAdminLTE.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/Content/css/bootstrap.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/bootstrap-select.css")
                .Include("~/Content/css/select2.min.css")
                .Include("~/Content/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/css/bootstrap-datetimepicker.min.css")
                .Include("~/Content/css/bootstrap-jquery.validate.css")
                .Include("~/Content/css/font-awesome.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/ionicons.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/icheck/square/blue.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/mvc-grid.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/mvc-tree.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/chartjs/Chart.min.css")
                .Include("~/Content/css/alerts.css")
                .Include("~/Content/css/AdminLTE.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/AdminLTE-extension.css")
                .Include("~/Content/css/skins/skin-blue.min.css")
                .Include("~/Content/css/public.css", new CssRewriteUrlTransformAbsolute())
                );

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/jquery/jquery-3.3.1.js")
                 .Include("~/Content/js/plugins/bootstrap/bootstrap.js")
                 .Include("~/Content/js/plugins/fastclick/fastclick.js")
                 .Include("~/Content/js/plugins/slimscroll/jquery.slimscroll.js")
                 .Include("~/Content/js/plugins/bootstrap-select/bootstrap-select.js")
                 .Include("~/Content/js/plugins/select2/select2.min.js")
                 .Include("~/Content/js/plugins/select2/i18n/*.js")
                 .Include("~/Content/js/plugins/moment/moment-with-locales.min.js")
                 .Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                 .Include("~/Content/js/plugins/datepicker/cultures/*.js")
                 .Include("~/Content/js/plugins/datetimepicker/bootstrap-datetimepicker.min.js")
                 .Include("~/Content/js/plugins/icheck/icheck.js")
                 .Include("~/Content/js/plugins/MvcGrid/mvc-grid.js")
                 .Include("~/Content/js/plugins/MvcGrid/cultures/*.js")
                 .Include("~/Content/js/plugins/MvcTree/mvc-tree.js")
                 .Include("~/Content/js/plugins/chartjs/Chart.min.js")
                 .Include("~/Content/js/plugins/jquery-validate/jquery.validate.js")
                 .Include("~/Content/js/plugins/jquery-validate/jquery.validate.unobtrusive.js")
                 .Include("~/Content/js/plugins/jquery-validate/jquery.validate.bootstrap.js")
                 .Include("~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js")
                 .Include("~/Content/js/plugins/jquery-sparkline/jquery.sparkline.js")
                 .Include("~/Content/js/adminlte.js")
                 .Include("~/Content/js/LanguageJs.js")
                 .Include("~/Content/js/init/*.js")
                 .Include("~/Content/js/init.js")
                );

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
