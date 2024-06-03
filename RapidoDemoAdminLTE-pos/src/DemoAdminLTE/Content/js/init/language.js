Language = {
    init: function () {
        $(document).ready(function () {
            var lang = LanguageJs.Cookies.getCookie("DemoAdminLTELanguage");

            $(".setLang").each(function () {
                if (lang != $(this).data("lang")) {
                    $(this).removeClass("hide");
                }

            });

            //$(".setLang[data-lang='" + lang + "']").addClass("hide");

            $(".setLang").on("click", function (event) {
                var lang = $(this).attr("data-lang");
                LanguageJs.Cookies.setCookie("DemoAdminLTELanguage", lang, 30);
                location.reload(true);
            });
        });
    }
};
