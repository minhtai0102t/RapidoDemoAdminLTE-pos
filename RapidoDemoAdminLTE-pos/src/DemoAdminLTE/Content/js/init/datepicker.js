DatePicker = {
    init: function () {
        var lang = document.documentElement.lang;

        $(".datepicker").datepicker({
            autoclose: true,
            language: lang,
        });

    }
};
