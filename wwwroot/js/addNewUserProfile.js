
// Function to handle form submission
function prepareRawData() {
    var selectedProfileUsers = [];
    var selectedProfiles = [];

    // Get the list of selected profile users and profiles
    var selectedProfileUsersValues = $('#profileUsers .selected').map(function () {
        return parseInt($(this).val(), 10); // Convert to integer
    }).get();

    var selectedProfilesValues = $('#profiles .selected').map(function () {
        return parseInt($(this).val(), 10); // Convert to integer
    }).get();

    // Filter the allProfileUsersLst to get the objects corresponding to the selected values
    allProfileUsersLst.forEach(function (user) {
        if (selectedProfileUsersValues.includes(user.userId)) {
            selectedProfileUsers.push(user);
        }
    });

    // Filter the allProfilesLst to get the objects corresponding to the selected values
    allProfilesLst.forEach(function (profile) {
        if (selectedProfilesValues.includes(profile.profileId)) {
            selectedProfiles.push(profile);
        }
    });

    // Set the hidden inputs with the JSON of the filtered objects
    $('#SelectedProfileUsersJson').val(JSON.stringify(selectedProfileUsers));
    $('#SelectedProfilesJson').val(JSON.stringify(selectedProfiles));
}

$('form').on('submit', function () {
    prepareRawData();
});

$(document).ready(function () {
    // Function to toggle selection on click
    $('#profileUsers').on('click', 'option', function () {
        $(this).toggleClass('selected');
        if ($(this).hasClass('selected')) {
            $(this).css('background-color', '#17a2b8'); // Highlight color
        } else {
            $(this).css('background-color', ''); // Remove highlight
        }
    });


    $('#profiles').on('click', 'option', function () {
        $(this).toggleClass('selected');
        if ($(this).hasClass('selected')) {
            $(this).css('background-color', '#17a2b8'); // Highlight color
        } else {
            $(this).css('background-color', ''); // Remove highlight
        }
    });
});