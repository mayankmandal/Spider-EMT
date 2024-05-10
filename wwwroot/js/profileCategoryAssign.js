$(document).ready(function () {
    // A set to track processed categories
    var processedCategories = new Set();

    // Designated area to display selected categories at the top
    var selectedCategoryDisplay = $('<div>').attr('id', 'selectedCategories').css({
        'display': 'flex',
        'flex-wrap': 'wrap',
        'padding': '5px',
        'margin-left':'10px'
    });

    // Find the parent element of CategoryIds to append selectedCategories beside it
    $('#CategoryIdsLabel').parent().append(selectedCategoryDisplay); // Add beside the CategoryIds label

    // Function to update the display for selected categories
    function updateSelectedCategoryDisplay() {
        selectedCategoryDisplay.empty();  // Clear previous display

        $('#CategoryIds option.selected-category').each(function () {
            var categoryId = $(this).val();
            var categoryName = $(this).text();

            // Create a badge for each selected category with a cross for removing
            var categoryBadge = $('<span>').addClass('badge bg-primary mx-1 my-1').text(categoryName + ' ');

            var removeIcon = $('<i>').addClass('icon ion-close-circled') // Cross for removing
                .click(function () {
                    // Remove class and trigger the change event
                    $('#CategoryIds option[value="' + categoryId + '"]').removeClass('selected-category');
                    $(`#category-section-${categoryId}`).remove();  // Remove corresponding section
                    processedCategories.delete(categoryId); // Update the set
                    updateSelectedCategoryDisplay();  // Refresh the display
                });

            categoryBadge.append(removeIcon); // Add cross to badge
            selectedCategoryDisplay.append(categoryBadge); // Add to the display
        });
    }


    // Function to render the checkboxes dynamically from availablePagesLst
    function renderPageCheckboxes(categoryId, categoryName) {
        if (processedCategories.has(categoryId)) {
            return; // Avoid re-processing a category that is already rendered
        }

        processedCategories.add(categoryId);

        $.ajax({
            url: '/api/Navigation/GetCategoryToPages/',
            data: { categoryId: categoryId },
            method: 'GET',
            success: function (pages) {
                var categorySection = $('<div>').attr('id', `category-section-${categoryId}`).addClass('form-control pt-4 pb-2 mb-2').css({
                    'height': 'auto',
                    'background-color': 'rgba(244,240,236,0.8)',
                    'width': 'auto',
                    'font-size': '1rem'
                });

                var header = $('<h3>').text('Pages under ' + categoryName).css({
                    'color': 'rgba(23,162,184,0.8)',
                    'text-align': 'center',
                    'font-size': '1.2rem'
                });

                var pageCheckboxContainer = $('<div>').addClass('form-check').css({
                    'display': 'grid',
                    'grid-template-columns': 'repeat(3, 1fr)', // 3 equal columns
                    'column-gap': '10px',
                    'row-gap': '10px',
                    'justify-content': 'flex-start'
                });

                pages.forEach(page => {
                    var checkboxDiv = $('<div>', {
                        style: 'display: flex; align-items: center; justify-content: flex-start; padding: 5px;'
                        // ,'flex': '1 0 auto' // Ensure proper alignment
                    });

                    var checkbox = $('<input>', {
                        type: 'checkbox',
                        class: 'form-check-input',
                        id: 'page-${page.pageId}',
                        value: page.pageId,
                        disabled: true // Disabled since it's for display only
                    });

                    var label = $('<label>', {
                        'for': 'page-${page.pageId}',
                        'class': 'form-check-label',
                        text: page.pageDescription,
                        style: 'margin-left: 5px; color:rgba(0, 0, 0,0.9);'
                    });

                    checkboxDiv.append(checkbox);
                    checkboxDiv.append(label);

                    pageCheckboxContainer.append(checkboxDiv);
                });

                categorySection.append(header);
                categorySection.append(pageCheckboxContainer);

                $('#dynamicCategorySections').append(categorySection);
            },
            error: function (xhr, status, error) {
                console.error('Error fetching pages for category: ', error);
            }
        });
    }

    // Update when a category is selected
    $('#CategoryIds').change(function () {
        updateSelectedCategoryDisplay(); // Refresh the display
    });

    // Event handler for click to toggle the 'selected-category' class
    $('#CategoryIds').click(function (e) {
        var $target = $(e.target);
        var categoryId = $target.val();
        var isSelected = $target.hasClass('selected-category'); // Check if already selected

        if (isSelected) {
            $target.removeClass('selected-category'); // Remove the class if deselected
            $(`#category-section-${categoryId}`).remove(); // Remove the section
            processedCategories.delete($target.val()); // Update the processed set
        } else {
            $target.addClass('selected-category'); // Add the class for selected
        }

        if (!isSelected) { // If selected
            console.log(`Category with ID ${categoryId} selected`);
            renderPageCheckboxes(categoryId, $target.text()); // Render the checkboxes
        }

        if (isSelected) {
            console.log(`Category with ID ${categoryId} deselected`);
            $(`#category-section-${categoryId}`).remove(); // Remove the corresponding section
            processedCategories.delete(categoryId); // Update the set
        }
        updateSelectedCategoryDisplay(); // Refresh the display
    });

    $('#categoryAssignForm').submit(function (e) {
        if ($('#ProfileId').val() === "") {
            e.preventDefault();
            toastr.error("Please select a Profile");
            return;
        }

        var selectedCategoryCount = $('#CategoryIds option.selected-category').length;

        if (selectedCategoryCount === 0) {
            e.preventDefault();
            toastr.error("Please select at least one Category");
            return;
        }

        selectedCategoryLst = [];
        $('#CategoryIds option.selected-category').each(function () {
            var categoryId = $(this).val();
            var matchingCategory = availableCategoryLst.find(c => c.pageCatId === parseInt(categoryId));

            if (matchingCategory) {
                selectedCategoryLst.push(matchingCategory); // Push Matching Category
            }
        });
        $('#SelectedCategoriesJson').val(JSON.stringify(selectedCategoryLst));

        // Get the selected profile ID
        var selectedProfileId = $('#ProfileId').val();

        // Get the selected profile name based on the selected option
        var selectedProfileName = $('#ProfileId option:selected').text();

        // Set the hidden fields with ProfileId and ProfileName
        $('#SelectedProfileId').val(selectedProfileId);
        $('#SelectedProfileName').val(selectedProfileName);
    });

    $('#ClearBtn').click(function () {
        $('#dynamicCategorySections').empty(); // Clear previous sections
        $('#CategoryIds option').removeClass('selected-category'); // Remove the selected class
        processedCategories.clear(); // Clear the set
        updateSelectedCategoryDisplay(); // Refresh the display
    });
});

