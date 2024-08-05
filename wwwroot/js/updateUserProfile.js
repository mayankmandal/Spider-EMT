var apiResultData = []; // Global variable to store the result set
function prepareFormSubmission() {
    // Get the selected profile ID
    var selectedProfileId = $('#profileSelect option:selected').attr('data-profileid');

    // Set the hidden fields with ProfileId and ProfileName
    $('#profileIdHidden').val(selectedProfileId);     // Set ProfileName to selected profile id
}
function handleResultItemClick(item, resultItem) {
    
    // Deselect all items
    $('.search-result-item').removeClass('selected');

    // Select the clicked item
    resultItem.addClass('selected');

    // Populate form with selected user data
    $('#ProfileUsersData_UserId').val(item.userId);
    $('#ProfileUsersData_IdNumber').val(item.idNumber);
    $('#ProfileUsersData_FullName').val(item.fullName);
    $('#ProfileUsersData_Email').val(item.email);
    $('#ProfileUsersData_MobileNo').val(item.mobileNo);
    $('#profileSelect').val(item.profileSiteData.profileName);
    $('#profileIdHidden').val(item.profileSiteData.profileId);
    $('#ProfileUsersData_Username').val(item.username);
    // Load user image
    $('#loadedProfilePicture').attr('src', item.userimgpath);
    // Check checkboxes based on item properties
    $('input[name="ProfileUsersData.IsActive"]').prop('checked', item.isActive === true);
    $('input[name="ProfileUsersData.IsActiveDirectoryUser"]').prop('checked', item.isActiveDirectoryUser === true);
    $('input[name="ProfileUsersData.ChangePassword"]').prop('checked', item.changePassword === true);
}

function handleResultOnError(item) {
    // Populate form with selected user data
    $('#ProfileUsersData_UserId').val(item.UserId);
    // Load user image
    $('#loadedProfilePicture').attr('src', item.Userimgpath);

    /*$('#ProfileUsersData_IdNumber').val(item.IdNumber);
    $('#ProfileUsersData_FullName').val(item.FullName);
    $('#ProfileUsersData_Email').val(item.Email);
    $('#ProfileUsersData_MobileNo').val(item.MobileNo);
    $('#profileSelect').val(item.profileSiteData.ProfileName);
    $('#profileIdHidden').val(item.profileSiteData.ProfileId);
    $('#ProfileUsersData_Username').val(item.Username);
    
    // Check checkboxes based on item properties
    $('input[name="ProfileUsersData.IsActive"]').prop('checked', item.IsActive === true);
    $('input[name="ProfileUsersData.IsActiveDirectoryUser"]').prop('checked', item.IsActiveDirectoryUser === true);
    $('input[name="ProfileUsersData.ChangePassword"]').prop('checked', item.ChangePassword === true);*/
}

$(document).ready(function () {
    // Check if ProfileUsersData.UserId has a value
    if (profileUsersDataJson) {
        handleResultOnError(profileUsersDataJson);
        $('#updateProfileSection').show();
    }
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
    $('#searchButton').on('click', function () {
        $('.validation-summary-errors').empty();
        $('.field-validation-error').empty();

        var searchCriteria = $('#searchCriteria').val();
        var searchInput = $('#searchInput').val();

        if (searchInput === null || searchInput === undefined || searchInput === "" || searchInput === 0) {
            $('#searchInput').addClass("is-invalid border-danger");
            $('#searchButtonIcon').addClass("border-danger");
            return;
        }
        $('#searchInput').removeClass("is-invalid border-danger");
        $('#searchButtonIcon').removeClass("border-danger");

        $.ajax({
            url: '/api/Navigation/SearchUserDetails',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + tokenC
            },
            data: {
                criteria: searchCriteria,
                input: searchInput
            },
            success: function (data) {
                apiResultData = data;
                var results = $('#searchResults');
                var resultsCount = $('#resultsCount');
                results.empty();
                if (data.length > 0) {
                    resultsCount.text(`${data.length} results present`);
                    results.show();
                    resultsCount.show();
                    data.forEach(function (item,index) {
                        var resultItem = $('<div></div>').text(item).addClass('search-result-item').css({
                            border: '1px solid #ccc',
                            padding: '10px',
                            margin: '5px',
                            flex: '0 1 45%', // Two-column layout
                            cursor: 'pointer'
                        });

                        var itemDetails = `
                        <p>User ID: ${item.userId}</p>
                        <p>Id Number: ${item.idNumber}</p>
                        <p>Full Name: ${item.fullName}</p>
                        <p>Email Address: ${item.email}</p>
                        <p>Mobile Number: ${item.mobileNo}</p>
                        <p>Profile Name: ${item.profileSiteData.profileName}</p>
                        `;
                        resultItem.html(itemDetails);

                        // Store the item index as data attribute
                        resultItem.data('itemIndex', index);

                        resultItem.on('click', function () {
                            handleResultItemClick(item, resultItem);
                        });

                        results.append(resultItem);
                    });
                    $('#selectButton').show();
                } else {
                    results.show();
                    resultsCount.show();
                    resultsCount.text(`0 results present`);
                    results.append($('<div class="p-2" style="border:1px solid #ccc; height:5rem; width: 45%;">No results found</div>'));
                    $('#selectButton').hide();
                }
            },
            error: function (xhr, status, error) {
                console.error('Error ' + error);
            }
        });
    });

    $('#clearButton').on('click', function () {
        // Clear all inputs fields and hide updateProfileSection
        $('#searchInput').val('');
        $('#searchResults').empty();
        $('#searchResults').hide();
        $('#resultsCount').text('');
        $('#resultsCount').hide();
        $('#updateProfileSection').hide();
        $('.validation-summary-errors').empty();
        $('.field-validation-error').empty();
        $('#selectButton').hide();
    })

    $('#selectButton').on('click', function () {
        // Find the selected result item
        var selectedResultItem = $('.search-result-item.selected');
        if (selectedResultItem.length > 0) {
            var itemIndex = selectedResultItem.data('itemIndex');
            var selectedItemData = apiResultData[itemIndex];
            handleResultItemClick(selectedItemData, selectedResultItem);
            $('#updateProfileSection').show();
            // Scroll to the updateProfileSection
            $('html, body').animate({
                scrollTop: $('#updateProfileSection').offset().top
            }, 'fast');
        }
    });
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

