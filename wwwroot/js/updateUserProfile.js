var apiResultData = []; // Global variable to store the result set
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
function mapUserStatus(status) {
    const statusMap = {
        'AC': 'IsActive',
        'AD': 'IsActiveDirectoryUser',
        'CH': 'ChangePassword'
    }

    // Split the status string by comma and map each individually
    const statusArray = status.split(',');
    const mappedStatusArray = statusArray.map(s => statusMap[s.trim()] || s.trim());

    // Join the mapped statuses back into a string 
    return mappedStatusArray.join(', ');
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
    $('#ProfileUsersData_UserStatus').val(item.userStatus);

    // Uncheck all the checkboxes first
    $('input[name="UserStatusLst"]').prop('checked', false);
    // Check the ones matching the user status
    item.userStatus.split(',').forEach(function (status) {
        $('input[value="' + status + '"]').prop('checked', true);
    });
}

$(document).ready(function () {
    $('#searchButton').on('click', function () {
        var searchCriteria = $('#searchCriteria').val();
        var searchInput = $('#searchInput').val();
        $.ajax({
            url: '/api/Navigation/SearchUserDetails',
            type: 'GET',
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

                        // Map user status
                        var mappedStatus = mapUserStatus(item.userStatus);

                        var itemDetails = `
                        <p>User ID: ${item.userId}</p>
                        <p>Id Number: ${item.idNumber}</p>
                        <p>Full Name: ${item.fullName}</p>
                        <p>Email Address: ${item.email}</p>
                        <p>Mobile Number: ${item.mobileNo}</p>
                        <p>Profile Name: ${item.profileSiteData.profileName}</p>
                        <p>User Status: ${mappedStatus}</p>
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
        // Clear all inputs fields and hide createProfileSection
        $('#searchInput').val('');
        $('#searchResults').empty();
        $('#searchResults').hide();
        $('#resultsCount').text('');
        $('#resultsCount').hide();
        $('#createProfileSection').hide();
        $('#selectButton').hide();
    })

    $('#selectButton').on('click', function () {
        // Find the selected result item
        var selectedResultItem = $('.search-result-item.selected');
        if (selectedResultItem.length > 0) {
            var itemIndex = selectedResultItem.data('itemIndex');
            var selectedItemData = apiResultData[itemIndex];
            handleResultItemClick(selectedItemData, selectedResultItem);
            $('#createProfileSection').show();
            // Scroll to the createProfileSection
            $('html, body').animate({
                scrollTop: $('#createProfileSection').offset().top
            }, 'fast');
        }
    });
});

