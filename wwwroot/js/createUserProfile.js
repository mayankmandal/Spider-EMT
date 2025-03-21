﻿function prepareFormSubmission() {
    // Get the selected profile ID
    var selectedProfileId = $('#profileSelect option:selected').attr('data-profileid');

    // Set the hidden fields with ProfileId and ProfileName
    $('#profileIdHidden').val(selectedProfileId);     // Set ProfileName to selected profile id
}
$(document).ready(function () {
    $('#profile-img-file-input').change(function () {
        // Get the selected file
        var file = this.files[0];
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
});
