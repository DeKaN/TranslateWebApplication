$(function () {
    $('input[type=text]').change(function () {
        $('input[type=checkbox]', $(this).parents('tr')).prop('checked', !!$(this).val());
    });
});