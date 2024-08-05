// Call the function to render the checkboxes when the page loads
$(document).ready(function () {
    // Function to render the checkboxes dynamically from availablePagesLst
    function renderPageCheckboxes(selectedPages) {
        var container = $('#pageCheckboxContainer'); // container to append checkboxes
        var updateBtn = $('button[type="submit"]'); // update button for submission
        updateBtn.prop('disabled', true);

        // Clear the container before rendering new content
        container.empty();

        // If selectedPages is not empty, render checkboxes with selections
        availablePagesLst.forEach(function (pageSite) {
            // Create a div for each checkbox
            var checkboxDiv = $('<div>', { style: 'flex: 0 0 32%; display:flex; align-items:center;justify-content:flex-start;padding:5px; box-sizing:border-box; font-size:1rem;' });

            // Check if the page should be selected
            var isChecked = selectedPages.some(function (page) {
                return page.pageId === pageSite.pageId; // Check if any object in the array has a matching pageId
            });

            // Create the checkbox inputl
            var checkbox = $('<input>', {
                type: 'checkbox',
                class: 'form-check-input',
                id: pageSite.pageId,
                name: 'SelectedPages',
                value: pageSite.pageId,
                checked: isChecked, // Check if the page should be selected
            });

            // Create the label for the checkbox
            var label = $('<label>', {
                class: 'form-check-label',
                for: pageSite.pageId,
                text: pageSite.pageDescription,
                style: 'margin-left: 5px;',
            });

            // Append the checkbox and label to the div
            checkboxDiv.append(checkbox);
            checkboxDiv.append(label);

            // Append the div to the container
            container.append(checkboxDiv);
        });

        // Enable the Update button
        updateBtn.prop('disabled', false);
    }

    // Ajax call to fetch selected pages for the chosen profile
    $('#CategoryIdDiv').change(function () {
        var selectedProfileId = $(this).val();
        
        $.ajax({
            url: '/api/Navigation/GetCategoryToPages/',
            headers: {
                Authorization: 'Bearer ' + tokenC
            },
            data: { categoryId: selectedProfileId },
            method: 'GET',
            success: function (response) {
                renderPageCheckboxes(response);
            },
            error: function (xhr, status, error) {
                console.error('Error fetching profile pages: ', error);
            }
        });
    });

    // Initial render of the checkboxes without selections
    renderPageCheckboxes([]);

    // Attach the prepare function to the form submission event
    $('#updateCategoryForm').submit(function (e) {
        e.preventDefault();

        // Initialize the list of selected pages
        selectedPagesLst = [];

        // Get the selected profile ID
        var selectedProfileId = $('#CategoryIdDiv').val();

        // Get the selected profile name based on the selected option
        var selectedProfileName = $('#CategoryIdDiv option:selected').text();

        // Set the hidden fields with ProfileId and ProfileName
        $('#SelectedProfileId').val(selectedProfileId);
        $('#SelectedProfileName').val(selectedProfileName);

        // Loop through all checkboxes and get their associated data
        $('input[name="SelectedPages"]').each(function () {
            var pageId = parseInt($(this).val());
            var isSelected = $(this).is(':checked'); // Check if the checkbox is selected
            if (isSelected) {
                // Find the corresponding data in availablePagesLst
                var selectedPage = availablePagesLst.find(page => page.pageId === pageId);
                if (selectedPage) {
                    // Add the selected page to selectedPageLst
                    selectedPagesLst.push({
                        PageId: selectedPage.pageId,
                        PageUrl: selectedPage.pageUrl,
                        PageDescription: selectedPage.pageDescription,
                        PageCatId: selectedPage.pageCatId,
                        MenuImgPath: selectedPage.menuImgPath,
                        isSelected: true
                    });
                }
            }
        });

        // Store the selected pages as JSON in a hidden field
        $('#SelectedPagesJson').val(JSON.stringify(selectedPagesLst));
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    // Remove 'valid' and 'is-valid' classes from all elements
                    $('.valid, .is-valid').removeClass('valid is-valid');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error response:", xhr);
                console.error("Status:", status);
                console.error("Error:", error);

                // Default error message if response.message is not defined
                var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : "An error occurred";
                toastr.error(errorMessage);
            }
        });
    });
});