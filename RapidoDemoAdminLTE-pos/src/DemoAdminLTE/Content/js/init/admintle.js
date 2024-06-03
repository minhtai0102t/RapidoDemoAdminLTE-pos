AdminTLE = {
    init: function () {
        // Fix sidebar white space at bottom of page on resize
        $(window).on("load", function () {
            setTimeout(function () {
                $("body").layout("fix");
                $("body").layout("fixSidebar");
            }, 250);

        });
    }
};
