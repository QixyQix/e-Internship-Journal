(function ($) {
    "use strict"; // Start of use strict

    // Floating label headings for the contact form
    $(function () {
        $("body").on("input propertychange", ".floating-label-form-group", function (e) {
            $(this).toggleClass("floating-label-form-group-with-value", !!$(e.target).val());
        }).on("focus", ".floating-label-form-group", function () {
            $(this).addClass("floating-label-form-group-with-focus");
        }).on("blur", ".floating-label-form-group", function () {
            $(this).removeClass("floating-label-form-group-with-focus");
        });
        //$(document).ajaxSuccess(function (event, xhr, settings) {

        //    if (settings.url.includes("/API/UserBatches/GetStudentBatches/")) {
        //        $("body .floating-label-form-group").addclass("floating-label-form-group-with-value");
        //    }
        //});
        //$("body").on("input propertychange", ".floating-label-form-group", function (e) {
        //    $('body .floating-label-form-group').addClass("floating-label-form-group-with-value");
        //})

    });

})(jQuery); // End of use strict
