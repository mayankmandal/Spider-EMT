// Function to empty categories lists and hide categories section
function resetCategories() {
    availableCategoriesLst = [];
    selectedCategoriesLst = [];
    $('#availableCategories').empty();
    $('#selectedCategories').empty();
    $('#categoriesSection').css('display', 'none');
}

// Function to fetch categories based on selected page IDs
function fetchCategoriesForPages(pageIds) {
    $.ajax({
        url: '/api/navigation/GetPageToCategories',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(pageIds),
        success: function (data) {
            availableCategoriesLst = data;
            renderPagesOrCategories(availableCategoriesLst, $('#availableCategories'), false);
            $('#categoriesSection').css('display', 'flex'); // Show the categories section
        },
        error: function (xhr, status, error) {
            console.error('Error: ', error);
        }
    });
}

// Generic function to render either pages or categories
function renderPagesOrCategories(list, container, isPage) {
    container.empty(); // Clear the container first
    list.forEach(item => {
        const div = $('<div></div>')
            .text(isPage ? item.pageDescription : item.categoryName)
            .addClass(isPage ? 'page' : 'category')
            .on('click', function () {
                $(this).toggleClass('selected');
            });
        container.append(div);
    });
}

// Function to move selected items from available to selected list and update UI
function moveItemsAndUpdateUI(sourceList, targetList, sourceContainerId, targetContainerId, isPage) {
    let selectedItems = $(`#${sourceContainerId} .selected`);
    selectedItems.each(function () {
        let selectedItemText = $(this).text();
        // Find the corresponding object in the source list
        let foundItem = sourceList.find(item =>
            isPage ? item.pageDescription === selectedItemText : item.categoryName === selectedItemText
        );
        if (foundItem) {
            // Move the found item from source to target list
            sourceList.splice(sourceList.indexOf(foundItem), 1);
            targetList.push(foundItem);
        }
        $(this).removeClass('selected');
    });

    // Re-render both lists after the update
    renderPagesOrCategories(sourceList, $(`#${sourceContainerId}`), isPage);
    renderPagesOrCategories(targetList, $(`#${targetContainerId}`), isPage);
}

// Bind event listeners to buttons
$('#moveRightBtnPage').on('click', function () {
    moveItemsAndUpdateUI(availablePagesLst, selectedPagesLst, 'availablePages', 'selectedPages', true);
    resetCategories();
    var pageIds = selectedPagesLst.map(page => page.pageId);
    if (pageIds.length === 0) {
        resetCategories(); // Reset categories if no selected pages
    } else {
        fetchCategoriesForPages(pageIds); // Fetch categories for selected pages
    }
});

$('#moveLeftBtnPage').on('click', function () {
    moveItemsAndUpdateUI(selectedPagesLst, availablePagesLst, 'selectedPages', 'availablePages', true);
    resetCategories();
    var pageIds = selectedPagesLst.map(page => page.pageId);
    if (pageIds.length === 0) {
        resetCategories(); // Reset categories if no selected pages
    } else {
        fetchCategoriesForPages(pageIds); // Fetch categories for selected pages
    }
});

// Bind event listener to button for moving categories
$('#moveRightBtnCategory').on('click', function () {
    moveItemsAndUpdateUI(availableCategoriesLst, selectedCategoriesLst, 'availableCategories', 'selectedCategories', false);
});

$('#moveLeftBtnCategory').on('click', function () {
    moveItemsAndUpdateUI(selectedCategoriesLst, availableCategoriesLst, 'selectedCategories', 'availableCategories', false);
});

// Function to handle form submission
function prepareData() {
    var selectedPages = selectedPagesLst;
    var selectedCategories = selectedCategoriesLst;
    $('#selectedPagesJson').val(JSON.stringify(selectedPages));
    $('#selectedCategoriesJson').val(JSON.stringify(selectedCategories));
}

$('form').on('submit', function () {
    prepareData();
});

$(document).ready(function () {
    // Call the functions to render available pages and categories
    renderPagesOrCategories(availablePagesLst, $('#availablePages'), true);
    renderPagesOrCategories(selectedPagesLst, $('#selectedPages'), true);
    renderPagesOrCategories(availableCategoriesLst, $('#availableCategories'), false);
    renderPagesOrCategories(selectedCategoriesLst, $('#selectedCategories'), false);

    // Check if there are selected categories
    if (selectedCategoriesLst.length > 0) {
        $('#categoriesSection').css('display', 'flex'); // Show the categories section
    }
});

