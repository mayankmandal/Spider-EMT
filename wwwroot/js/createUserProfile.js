function prepareFormSubmission() {
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
}
