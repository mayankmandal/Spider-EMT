$(document).ready(function () {

    $('#SettingsData_SettingUsername, #SettingsData_SettingEmail').on('blur', function () {
        var field = $(this).attr('data-field');
        var value = $(this).val();
        var validationSpan = $(this).attr('data-field') + '-validation';
        checkUniqueness(field, value, validationSpan);
    });
    
    $('#togglePasswordVisibility').on('click', function () {
        let passwordInput = $('input[name="SettingsData_Password"]');
        let icon = $(this).find('i');
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        } else {
            passwordInput.attr('type', 'password');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        }
    });

    $('#toggleReTypePasswordVisibility').on('click', function () {
        let reTypePasswordInput = $('input[name="SettingsData_ReTypePassword"]');
        let icon = $(this).find('i');
        if (reTypePasswordInput.attr('type') === 'password') {
            reTypePasswordInput.attr('type', 'text');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        } else {
            reTypePasswordInput.attr('type', 'password');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        }
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
        } else {
            toastr.error('Invalid file type. Only image files (jpg, jpeg, png, gif) are allowed.');
            $(this).val(''); // Clear the input
        }
    });

    $('#updateSettingsForm').submit(function (e) {
        e.preventDefault();

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