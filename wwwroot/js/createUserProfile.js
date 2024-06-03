$(document).ready(function () {
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

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                toastr.error(response.message);
            }
        });
    });
});
