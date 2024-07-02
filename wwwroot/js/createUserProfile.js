$(document).ready(function () {

    $('#ProfileUsersData_Username, #ProfileUsersData_MobileNo, #ProfileUsersData_Email, #ProfileUsersData_IdNumber').on('blur', function () {
        var field = $(this).attr('data-field');
        var value = $(this).val();
        var validationSpan = $(this).attr('data-field') + '-validation';
        checkUniqueness(field, value, validationSpan);
    });

    $('#profile-img-file-input').change(function () {
        // Get the selected file
        var file = this.files[0];
        var validExtensions = ['jpg', 'jpeg', 'png', 'gif'];
        var fileName = file.name.toLowerCase();
        var fileExtension = fileName.split('.').pop();

        if (validExtensions.includes(fileExtension)) {
            // Create a file reader object
            var reader = new FileReader();

            // Set up the FileReader Onload event
            reader.onload = function (e) {
                // Set the SRC attribute of the image element to the data URL of the selected file
                $('#loadedProfilePicture').attr('src', e.target.result);
            };

            // Read the selected file as a data URL
            reader.readAsDataURL(file);
        }
    });
    $('form').submit(function (e) {
        e.preventDefault();

        // Get the selected profile ID
        var selectedProfileId = $('#profileSelect option:selected').attr('data-profileid');

        // Set the hidden fields with ProfileId and ProfileName
        $('#profileIdHidden').val(selectedProfileId);     // Set ProfileName to selected profile id

        // Create a FormData object
        var formData = new FormData(this);

        // Form submission handling
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                // Success handling
                if (response.success) {
                    toastr.success(response.message);
                    // Remove 'valid' and 'is-valid' classes from all elements
                    $('.valid, .is-valid').removeClass('valid is-valid');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                // Error Handling
                toastr.error("An error occurred: " + error);
            }
        });
    });
});
