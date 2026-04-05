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

function printHtmlContent(html, title) {
    var form = document.createElement('form');
    form.method = 'post';
    form.action = '/print/html';
    form.target = '_blank';

    var inputHtml = document.createElement('textarea');
    inputHtml.name = 'Html';
    inputHtml.style.display = 'none';
    inputHtml.value = html;
    form.appendChild(inputHtml);

    var inputTitle = document.createElement('input');
    inputTitle.type = 'hidden';
    inputTitle.name = 'Title';
    inputTitle.value = title || 'Imprimir';
    form.appendChild(inputTitle);

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}

function cloneTableForPrint(table) {
    var clone = table.cloneNode(true);

    // Find action column by header text or button presence.
    var actionIndex = -1;
    var headers = clone.querySelectorAll('thead th');
    headers.forEach(function (th, index) {
        var text = th.textContent.trim().toLowerCase();
        if (text === 'acciones' || text === 'acción' || text === 'accion') {
            actionIndex = index;
        }
    });

    if (actionIndex < 0) {
        var firstBodyRow = clone.querySelector('tbody tr');
        if (firstBodyRow) {
            var cells = firstBodyRow.children;
            for (var i = 0; i < cells.length; i++) {
                if (cells[i].querySelector('button, a.btn, .btn, form')) {
                    actionIndex = i;
                    break;
                }
            }
        }
    }

    if (actionIndex >= 0) {
        clone.querySelectorAll('thead tr, tbody tr, tfoot tr').forEach(function (row) {
            if (row.children[actionIndex]) {
                row.removeChild(row.children[actionIndex]);
            }
        });
    }

    clone.querySelectorAll('button, .btn, form').forEach(function (element) {
        if (element.tagName.toLowerCase() === 'form') {
            element.remove();
        } else {
            element.style.display = 'none';
        }
    });

    return clone;
}

function printTable(selector, title) {
    var table = document.querySelector(selector);
    if (!table) {
        swal('Error', 'No se encontró la tabla para imprimir.', 'error');
        return;
    }

    var printableTable = cloneTableForPrint(table);
    printHtmlContent(printableTable.outerHTML, title || 'Imprimir');
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
