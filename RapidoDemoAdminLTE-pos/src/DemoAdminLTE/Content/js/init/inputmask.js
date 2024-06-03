InputMask = {
    init: function () {
        //$("#datemask").inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });
        //$("#datemask2").inputmask("mm/dd/yyyy", { "placeholder": "mm/dd/yyyy" });
        //$("[data-mask]").inputmask();
        $(".phonenumber-mask").inputmask("0999-999-999");
        $(".email-mask").inputmask("*{1,20}[.*{1,20}][.*{1,20}][.*{1,20}]@*{1,20}[.*{2,6}][.*{1,2}]");
    }
};
