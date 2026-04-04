// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function loadGeneralConfig() {
    $.getJSON('/api/config/general')
        .done(function (data) {
            $('#configNombreAlcaldia').val(data.nombreAlcaldia || '');
            $('#configIva').val(data.ivaPercent || '');
            $('#configConsecutivoEntrada').val(data.consecutivoEntrada || '');
            $('#configConsecutivoSalida').val(data.consecutivoSalida || '');
        })
        .fail(function () {
            swal('Error', 'No se pudo cargar la configuración general.', 'error');
        });
}

function saveGeneralConfig() {
    var payload = {
        nombreAlcaldia: $('#configNombreAlcaldia').val(),
        ivaPercent: $('#configIva').val(),
        consecutivoEntrada: $('#configConsecutivoEntrada').val(),
        consecutivoSalida: $('#configConsecutivoSalida').val()
    };

    $.ajax({
        url: '/api/config/general',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
    })
    .done(function () {
        $('#configModal').modal('hide');
        swal('Guardado', 'Configuración general guardada correctamente.', 'success');
    })
    .fail(function () {
        swal('Error', 'Error al guardar la configuración general.', 'error');
    });
}

$(function () {
    $('#configModal').on('show.bs.modal', function () {
        loadGeneralConfig();
    });

    $('#configGuardar').on('click', function () {
        saveGeneralConfig();
    });

    $('.datatable').each(function () {
        if (!$.fn.DataTable.isDataTable(this)) {
            $(this).DataTable({
                language: {
                    url: '/lib/datatables/i18n/Spanish.json'
                },
                responsive: true,
                order: [],
                pageLength: 10,
                lengthMenu: [5, 10, 25, 50, 100]
            });
        }
    });
});
