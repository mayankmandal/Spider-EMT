var apiResultData = []; // Global variable to store the result set

function handleResultItemClick(resultItem) {
    resultItem = $(resultItem);

    // Deselect all items
    $('.search-result-item').removeClass('selected');

    // Select the clicked item
    resultItem.addClass('selected');
}

function displaySelectedItemDetails(item, entityType) {
    var detailsForm = $('#detailsForm');
    detailsForm.empty();

    if (entityType === "User") {
        detailsForm.html(`
            <div class="mb-3 row">
                <div class="col">
                    <label for="ProfileUsersData_UserId" class="form-label">User Id</label>
                    <input type="text" id="ProfileUsersData_UserId" class="form-control" value="${item.userId}" readonly>
                    </div>
                <div class="col">
                    <label for="ProfileUsersData_IdNumber" class="form-label">Id Number</label>
                    <input type="text" id="ProfileUsersData_IdNumber" class="form-control" value="${item.idNumber}" readonly>
                </div>
            </div>
            <div class="mb-3">
                <label for="ProfileUsersData_FullName" class="form-label">Full Name</label>
                <input type="text" id="ProfileUsersData_FullName" class="form-control" value="${item.fullName}" readonly>
            </div>
            <div class="mb-3">
                <label for="ProfileUsersData_Email" class="form-label">Email</label>
                <input type="email" id="ProfileUsersData_Email" class="form-control" value="${item.email}" readonly>
            </div>
            <div class="mb-3">
                <label for="ProfileUsersData_MobileNo" class="form-label">Mobile Number</label>
                <input type="text" id="ProfileUsersData_MobileNo" class="form-control" value="${item.mobileNo}" readonly>
            </div>
            <div class="mb-3">
                <label for="profileSelect" class="form-label">Profile</label>
                <input type="text" id="profileSelect" class="form-control" value="${item.profileSiteData.profileName}" readonly>
            </div>
            <input type="hidden" id="profileIdHidden" value="${item.profileSiteData.profileId}" readonly>
        `);
    } else if (entityType === "Category") {
        detailsForm.html(`
        <div class="mb-3">
                <label for="categoryId" class="form-label">Category Id</label>
                <input type="text" id="categoryId" class="form-control" value="${item.pageCatId}" readonly>
        </div>
        <div class="mb-3">
            <label for="categoryName" class="form-label">Category Name</label>
            <input type="text" id="categoryName" class="form-control" value="${item.categoryName}" readonly>
        </div>
        `);
    } else if (entityType === "Profile") {
        detailsForm.html(`
        <div class="mb-3">
            <label for="profileId" class="form-label">Profile Id</label>
            <input type="text" id="profileId" class="form-control" value="${item.profileId}" readonly>
        </div>
        <div class="mb-3">
            <label for="profileName" class="form-label">Profile Name</label>
            <input type="text" id="profileName" class="form-control" value="${item.profileName}" readonly>
        </div>
        `);
    }
    $('#deleteEntitySection').show(); // Ensure the section is visible
}

$(document).ready(function () {
    $('#searchButton').on('click', function () {
        var searchCriteria = $('#searchCriteria').val();
        var apiUrl = '';
        if (searchCriteria === "Category") {
            apiUrl = '/api/Navigation/GetAllCategories';
        } else if (searchCriteria === "Profile") {
            apiUrl = '/api/Navigation/GetAllProfiles';
        } else if (searchCriteria === "User") {
            apiUrl = '/api/Navigation/GetAllUsers';
        } else {
            return;
        }

        $.ajax({
            url: apiUrl,
            type: 'GET',
            success: function (data) {
                apiResultData = data;
                var results = $('#searchResults');
                var resultsCount = $('#resultsCount');
                results.empty();
                if (data.length > 0) {
                    resultsCount.text(`${data.length} results present`).show();
                    results.show();
                    data.forEach(function (item, index) {
                        var resultItem = $('<div></div>').text(item).addClass('search-result-item').css({
                            border: '1px solid #ccc',
                            padding: '10px',
                            margin: '5px',
                            flex: '0 1 45%', // Two-column layout
                            cursor: 'pointer'
                        })
                            .data('itemIndex', index)
                            .on('click', function () {
                                handleResultItemClick(this);
                            });

                        var itemDetails = '';
                        if (searchCriteria === "User") {
                            itemDetails = `
                            <p>User ID: ${item.userId}</p>
                                <p>Id Number: ${item.idNumber}</p>
                                <p>Full Name: ${item.fullName}</p>
                                <p>Email Address: ${item.email}</p>
                                <p>Mobile Number: ${item.mobileNo}</p>
                                <p>Profile Name: ${item.profileSiteData.profileName}</p>
                                <p>User Name: ${item.username}</p>
                            `;
                        } else if (searchCriteria === "Category") {
                            itemDetails = `
                            <p>Category ID: ${item.pageCatId}</p>
                            <p>Category Name: ${item.categoryName}</p>
                            `;
                        } else if (searchCriteria === "Profile") {
                            itemDetails = `
                            <p>Profile Id: ${item.profileId}</p>
                            <p>Profile Name: ${item.profileName}</p>
                            `;
                        }

                        resultItem.html(itemDetails);
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
        // Clear all inputs fields and hide deleteEntitySection
        $('#searchResults').empty().hide();
        $('#resultsCount').text('').hide();
        $('#selectButton').hide();
        $('#deleteEntitySection').hide();
    });

    $('#selectButton').on('click', function () {
        // Find the selected result item
        var selectedResultItem = $('.search-result-item.selected');

        if (selectedResultItem.length > 0) {
            var itemIndex = selectedResultItem.data('itemIndex');
            var selectedItemData = apiResultData[itemIndex];

            var entityType = $('#searchCriteria').val();
            displaySelectedItemDetails(selectedItemData, entityType)

            // Set values for EntityType and EntityId
            $('#EntityType').val(entityType);

            if (entityType === "Category") {
                $('#EntityId').val(selectedItemData.pageCatId);
            } else if (entityType === "Profile") {
                $('#EntityId').val(selectedItemData.profileId);
            } else if (entityType === "User") {
                $('#EntityId').val(selectedItemData.userId);
            }

            // Scroll to the deleteEntitySection
            $('html, body').animate({
                scrollTop: $('#deleteEntitySection').offset().top
            }, 'fast');
        } else {
            alert("Please select an item from the list");
        }
    });

    $('form').submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#deleteEntitySection').hide();
                    // Trigger the search button click event
                    $('#searchButton').trigger('click');
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

