var table;
var datatable;
var exportedCols = [];

function onModalBegain() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}
function showSuccessMessage(message = 'Saved successfully') {
    Swal.fire({
        icon: 'success',
        title: 'success',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-activ-lihgt-primary"
        }
    });
}
function showErrorMessage(message = 'Saved successfully') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-activ-lihgt-primary"
        }
    });
}
function onModalComplete() {
    $('body :submit').removeAttr('disabled');
}

//DataTables
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});

// Class definition
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {

        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            'pageLength': 10,
        });
    }


    // Export options
    var exportButtons = () => {

        const documentTitle = $('.js-datatables').data('document-title');

        var buttons = new $.fn.dataTable.Buttons(table, {

            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }

                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }
    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }
    // Public methods

    return {

        init: function () {

            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();

        }
    };

}();


$(document).ready(function () {
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    //Handle Change Status
    $('.js-toggle-status').on('click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "Are you sure that you nees to change this item status!",
            size: "mediam",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-xs btn-danger btn-hover-rise'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-xs btn-secondary btn-hover-rise'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),

                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },

                        success: function (LastUpdetedOn) {
                            var raw = btn.parents('tr')
                            var status = raw.find('.js-status')
                            var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger')
                            raw.find('.js-lastupdatedon').html(LastUpdetedOn);
                            raw.addClass('animate__animated animate__flash');
                            showSuccessMessage();
                        },

                        Error: function (LastUpdetedOn) {
                            showErrorMessage();
                        }
                    });
                }
            }
        });

    })

});