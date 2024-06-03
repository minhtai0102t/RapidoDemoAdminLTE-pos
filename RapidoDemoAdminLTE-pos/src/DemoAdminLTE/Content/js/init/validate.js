ValidateExtras = {
    init: function () {

        $.validator.methods.date = function (value, element) {
            return this.optional(element) || moment(value, "DD/MM/YYYY HH:mm:ss", true).isValid();
        }

        $.validator.methods.number = function (value, element) {
            //return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)?(?:,\d+)?$/.test(value);
            return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
        }

        $.validator.methods.range = function (value, element, param) {
            var globalizedValue = value.replace(",", ".");
            return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
        }

    }
};
