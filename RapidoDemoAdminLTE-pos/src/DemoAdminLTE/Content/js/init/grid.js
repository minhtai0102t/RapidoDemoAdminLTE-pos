Grid = {
    init: function () {
        [].forEach.call(document.getElementsByClassName('mvc-grid'), function (element) {
            var lang = document.documentElement.lang;
            MvcGrid.prototype.lang = MvcGrid.prototype.lang[lang];
            new MvcGrid(element);
        });
    }
};
