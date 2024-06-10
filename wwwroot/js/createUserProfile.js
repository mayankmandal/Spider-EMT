$(document).ready(function () {
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
    $('form').submit(function (e) {
        e.preventDefault();

        // Get the selected profile ID
        var selectedProfileId = $('#profileSelect option:selected').attr('data-profileid');

        // Set the hidden fields with ProfileId and ProfileName
        $('#profileIdHidden').val(selectedProfileId);     // Set ProfileName to selected profile id

        // Get the selected user statuses
        var selectedUserStatusLst = [];
        $('input[name="UserStatusLst"]:checked').each(function () {
            selectedUserStatusLst.push($(this).val());
        });

        // Set the hidden field with selected user statuses
        $('#ProfileUsersData_UserStatus').val(selectedUserStatusLst); // Assuming a comma-separated string for UserStatus

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
