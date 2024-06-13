$(document).ready(function () {
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

        // Check if a file is selected
        if (file) {
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
                    fetchUserProfile();
                    toastr.success(response.message);
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