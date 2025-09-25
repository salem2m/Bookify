$(document).ready(function () {
    $('.page-link').on('click', function () {

        var btn = $(this);
        var bageNumber = btn.data('page-number');
        $('#pageNumber').val(bageNumber);
        $('#Filters').submit();
    });

    $('.js-date-range').daterangepicker({
        autoApply: true,
        changeYear: true,
        showDropdowns: true,
        minYear: 2024,
        maxDate: new Date(),
        autoUpdateInput: false,


    });
    $('.js-date-range').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });
});