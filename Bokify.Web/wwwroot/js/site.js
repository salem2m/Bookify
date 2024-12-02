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
$(document).ready(function() {
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

});