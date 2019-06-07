$(function () {
    $(".actionDelete").on('click', function (e) {
        e.preventDefault();
        var result = confirm("Are you sure you want to delete this item?");
        if (result) {
            $(this).closest('form').submit();
        }
    });
});