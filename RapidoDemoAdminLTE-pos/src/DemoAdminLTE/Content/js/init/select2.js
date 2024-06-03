Select2 = {
    init: function () {
        var lang = document.documentElement.lang;

        $('select.apply_select2').select2({
            language: lang,
            //dropdownAutoWidth: true,
            //width: 'auto',
        });

    }
};
