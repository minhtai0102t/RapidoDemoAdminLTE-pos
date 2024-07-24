using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using DemoAdminLTE.Models;

namespace DemoAdminLTE.Extensions
{
    public static class BootstrapExtensions
    {
        public static IHtmlString FormEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool disabled = false, bool @readonly = false, string htmlClass = "")
        {
            // label
            var label = htmlHelper.LabelFor(expression);

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control " + htmlClass);
            htmlAttributes.Add("placeholder", HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToString()));
            if (disabled)
                htmlAttributes.Add("disabled", "");
            if (@readonly)
                htmlAttributes.Add("readonly", "");
            var editor = htmlHelper.EditorFor(expression, new { htmlAttributes });

            if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(decimal))
            {
                editor = MvcHtmlString.Create(editor.ToString().Replace(",", "."));
            }

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group has-feedback\">",
                    label, editor, validator,
                    "<span class=\"form-control-feedback\"></span>",
                    "</div>"
                )
            );
            return result;
        }

        public static IHtmlString FormCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, bool disabled = false)
        {
            // label
            var displayname = htmlHelper.DisplayNameFor(expression);

            // checkbox
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control");
            if (disabled)
                htmlAttributes.Add("disabled", "");
            var checkbox = htmlHelper.CheckBoxFor(expression, htmlAttributes);

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group\">",
                    "<label>",
                    checkbox,
                    "&nbsp;",
                    displayname,
                    "</label>",
                    "</div>"
                )
            );
            return result;
        }


        public static IHtmlString FormDropDownList<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string name, bool disabled = false, string cssClass = "")
        {
            MvcHtmlString mvcHtmlString1 = htmlHelper.LabelFor<TModel, TValue>(expression);
            MvcHtmlString mvcHtmlString2 = htmlHelper.ValidationMessageFor<TModel, TValue>(expression, "", (object)new
            {
                @class = "text-danger"
            });
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("class", (object)("form-control " + cssClass));
            if (disabled)
                routeValueDictionary.Add(nameof(disabled), (object)"");
            MvcHtmlString mvcHtmlString3 = SelectExtensions.DropDownList(htmlHelper, name, (IEnumerable<SelectListItem>)null, (IDictionary<string, object>)routeValueDictionary);
            return (IHtmlString)MvcHtmlString.Create("<div class=\"form-group\">" + (object)mvcHtmlString1 + (object)mvcHtmlString3 + (object)mvcHtmlString2 + "</div>");
        }


        public static IHtmlString FormListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, bool disabled = false)
        {
            // label
            var label = htmlHelper.LabelFor(expression);

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control");
            if (disabled)
                htmlAttributes.Add("disabled", "");

            var editor = htmlHelper.ListBoxFor(expression, selectList, htmlAttributes);

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group\">",
                    label, editor, validator,
                    "</div>"
                )
            );
            return result;
        }

        public static MvcHtmlString FormTreeFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, MvcTree>> expression, int? hideDepth = null, bool readOnly = false)
        {
            // label
            var label = htmlHelper.LabelFor(expression);

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            var editor = htmlHelper.TreeFor(expression, hideDepth, readOnly);

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group\">",
                    label, editor, validator,
                    "</div>"
                )
            );
            return result;
        }

        public static IHtmlString FormHorizontalEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool disabled = false, bool @readonly = false)
        {
            // label
            var label = htmlHelper.LabelFor(expression, new { @class = "col-sm-3 control-label" });

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control");
            htmlAttributes.Add("placeholder", HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToString()));
            if (disabled)
                htmlAttributes.Add("disabled", "");
            if (@readonly)
                htmlAttributes.Add("readonly", "");
            var editor = htmlHelper.EditorFor(expression, new { htmlAttributes });

            if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(decimal))
            {
                editor = MvcHtmlString.Create(editor.ToString().Replace(",", "."));
            }

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group has-feedback\">",
                    label,
                    "<div class=\"col-sm-9\">",
                    editor, validator,
                    "<span class=\"form-control-feedback\"></span>",
                    "</div>",
                    "</div>"
                )
            );
            return result;
        }

        public static IHtmlString FormHorizontalTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool disabled = false, bool @readonly = false)
        {
            // label
            var label = htmlHelper.LabelFor(expression, new { @class = "col-sm-3 control-label" });

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control");
            htmlAttributes.Add("placeholder", HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToString()));
            if (disabled)
                htmlAttributes.Add("disabled", "");
            if (@readonly)
                htmlAttributes.Add("readonly", "");
            var editor = htmlHelper.TextAreaFor(expression, htmlAttributes);

            if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(decimal))
            {
                editor = MvcHtmlString.Create(editor.ToString().Replace(",", "."));
            }

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group has-feedback\">",
                    label,
                    "<div class=\"col-sm-9\">",
                    editor, validator,
                    "<span class=\"form-control-feedback\"></span>",
                    "</div>",
                    "</div>"
                )
            );
            return result;
        }

        public static IHtmlString FormHorizontalPasswordFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool disabled = false, bool @readonly = false)
        {
            // label
            var label = htmlHelper.LabelFor(expression, new { @class = "col-sm-3 control-label" });

            // validator
            var validator = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // editor
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes.Add("class", "form-control");
            htmlAttributes.Add("placeholder", HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToString()));
            if (disabled)
                htmlAttributes.Add("disabled", "");
            if (@readonly)
                htmlAttributes.Add("readonly", "");
            var editor = htmlHelper.PasswordFor(expression, htmlAttributes);

            if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(decimal))
            {
                editor = MvcHtmlString.Create(editor.ToString().Replace(",", "."));
            }

            var result = MvcHtmlString.Create(
                string.Concat(
                    "<div class=\"form-group has-feedback\">",
                    label,
                    "<div class=\"col-sm-9\">",
                    editor, validator,
                    "<span class=\"form-control-feedback\"></span>",
                    "</div>",
                    "</div>"
                )
            );
            return result;
        }

        public static IHtmlString GridValueHeaderHtml(this HtmlHelper htmlHelper, Sensor[] sensors)
        {
            string str = "";
            if (sensors != null)
            {
                foreach (Sensor sensor in sensors)
                    str = str + sensor.name + "</span></th><th><span class=\"mvc-grid-title\">";
            }
            return MvcHtmlString.Create(str ?? "");
        }
    }
}
