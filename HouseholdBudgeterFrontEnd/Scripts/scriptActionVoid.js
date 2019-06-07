$(function () {
    $(".actionVoid").on('click', function (e) {
        e.preventDefault();
        var result = confirm("Void transaction?");
        if (result) {
            $(this).closest('form').submit();
        }
    });
});