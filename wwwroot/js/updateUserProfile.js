﻿var apiResultData = []; // Global variable to store the result set

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
    if (item.userimgpath) {
        $('#loadedProfilePicture').attr('src', item.userimgpath);
    } else {
        // Handle case where user image path is empty or null
        $('#loadedProfilePicture').attr('src', '/images/icons/defaultUserImage.jpg'); // Set src to empty string or placeholder image path
    }
    // Check checkboxes based on item properties
    $('input[name="ProfileUsersData.IsActive"]').prop('checked', item.isActive === true);
    $('input[name="ProfileUsersData.IsActiveDirectoryUser"]').prop('checked', item.isActiveDirectoryUser === true);
    $('input[name="ProfileUsersData.ChangePassword"]').prop('checked', item.changePassword === true);
}

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
    $('#searchButton').on('click', function () {
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
                        <p>User Name: ${item.username}</p>
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

