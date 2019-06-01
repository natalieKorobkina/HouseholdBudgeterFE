$(function () {
    $(".actionJoin").on('click', function (e) {
        e.preventDefault();
        var result = confirm("Join this household?");
        if (result) {
            $(this).closest('form').submit();
        }
    });
});