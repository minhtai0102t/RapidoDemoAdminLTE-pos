DateTimePicker = {
    init: function () {
        var lang = document.documentElement.lang;

        $('input[type="datetime"]').datetimepicker({
            locale: lang,
            format: 'DD/MM/YYYY HH:mm:ss',
        });

    }
};
