$(function () {
    $(".actionLeave").on('click', function (e) {
        e.preventDefault();
        var result = confirm("Are you sure you want to leave this household?");
        if (result) {
            $(this).closest('form').submit();
        }
    });
});