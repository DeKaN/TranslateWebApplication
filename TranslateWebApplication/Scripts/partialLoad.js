$(function () {
    $.ajaxSetup({
        beforeSend: function () {
            // show image here
            $("#busy").show();
        },
        complete: function () {
            // hide image here
            $("#busy").hide();
        }
    });
});

function rebind() {
    $('input[type=text]').change(function () {
        $('input[type=checkbox]', $(this).parents('tr')).prop('checked', !!$(this).val());
    });
}

function loadTable(lang) {
    $.ajax({
        type: 'POST',
        url: '/Home/GetTable',
        data: {
            lang: lang
        },
        success: function (data) {
            $('#translatesTable').html(data);
            rebind();
        }
    });
}