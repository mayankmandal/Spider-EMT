$(document).ready(function () {
    $('#transactionFeeForm').on('submit', function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    // Remove 'valid' and 'is-valid' classes from all elements
                    $('.valid, .is-valid').removeClass('valid is-valid');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                toastr.error(response.message);
            }
        });
    });
})