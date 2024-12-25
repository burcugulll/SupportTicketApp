// wwwroot/js/sweetalert-helper.js
function confirmAction(title, text, confirmCallback, cancelCallback) {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, devam et',
        cancelButtonText: 'Hayır, iptal et',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            confirmCallback();
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            if (cancelCallback) cancelCallback();
        }
    });
}

