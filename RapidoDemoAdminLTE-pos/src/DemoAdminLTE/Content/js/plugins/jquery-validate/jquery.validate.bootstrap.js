(function ($) {
    $.fn.removeClassStartingWith = function (filter) {
        $(this).removeClass(function (index, className) {
            return (className.match(new RegExp("\\S*" + filter + "\\S*", 'g')) || []).join(' ')
        });
        return this;
    };

    var defaultOptions = {
        errorClass: 'has-error',
        validClass: 'has-success',
        highlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
                .addClass(errorClass)
                .removeClass(validClass);
            $(element).closest(".form-group").find(".form-control-feedback")
                .removeClassStartingWith("ion-")
                .addClass("ion-close");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
                .removeClass(errorClass)
                .addClass(validClass);

            $(element).closest(".form-group").find(".form-control-feedback")
                .removeClassStartingWith("ion-")
                .addClass("ion-checkmark");
        }
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);