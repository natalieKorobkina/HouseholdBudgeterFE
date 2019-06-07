$(function () {
    $(".actionUpdate").on('click', function (e) {
        e.preventDefault();
        var result = confirm("Update balance?");
        if (result) {
            $(this).closest('form').submit();
        }
    });
});