// Sample data for available and selected pages
let selectedPagesLst = [];

// Sample data for available and selected Categories
let selectedCategoriesLst = [];

// Function to render available pages
function renderAvailablePages(pages) {
    const availablePagesDiv = $('#availablePages');
    availablePagesDiv.empty();
    pages.forEach(page => {
        const pageDiv = $('<div></div>').text(page.pageDescription).addClass('page');
        pageDiv.on('click', function () {
            $(this).toggleClass('selected');
        });
        availablePagesDiv.append(pageDiv);
    });
}

// Function to render available categories
function renderAvailableCategories(categories) {
    const availableCategoriesDiv = $('#availableCategories');
    availableCategoriesDiv.empty();
    categories.forEach(category => {
        const categoryDiv = $('<div></div>').text(category.categoryName).addClass('category');
        categoryDiv.on('click', function () {
            $(this).toggleClass('selected');
        });
        availableCategoriesDiv.append(categoryDiv);
    });
}

// Function to move selected items from available to selected list
function moveSelectedItems(sourceList, targetList, sourceContainer, targetContainer) {
    const selectedItems = sourceContainer.find('.selected');
    selectedItems.each(function () {
        const selectedItem = $(this);
        const selectedItemText = selectedItem.text();
        selectedItem.removeClass('selected');
        targetContainer.append(selectedItem);
        targetList.push(selectedItemText);
        const index = sourceList.indexOf(selectedItemText);
        if (index > -1) {
            sourceList.splice(index, 1);
        }
    });
}

// Function to handle item selection and movement
function handleItemMovement(sourceList, targetList, sourceContainer, targetContainer) {
    const selectedItems = sourceContainer.find('.selected');
    selectedItems.each(function () {
        const selectedItem = $(this);
        const selectedItemText = selectedItem.text();
        selectedItem.removeClass('selected');
        targetContainer.append(selectedItem);
        targetList.push(selectedItemText);
        const index = sourceList.indexOf(selectedItemText);
        if (index > -1) {
            sourceList.splice(index, 1);
        }
    });
}

// Move selected pages to selected list
$('#moveRightBtnPage').on('click', function () {
    moveSelectedItems(availablePagesLst, selectedPagesLst, $('#availablePages'), $('#selectedPages'));
});

$('#moveLeftBtnPage').on('click', function () {
    moveSelectedItems(selectedPagesLst, availablePagesLst, $('#selectedPages'), $('#availablePages'));
});

// Move selected categories to selected list
$('#moveRightBtnCategory').on('click', function () {
    moveSelectedItems(availableCategoriesLst, selectedCategoriesLst, $('#availableCategories'), $('#selectedCategories'));
});

$('#moveLeftBtnCategory').on('click', function () {
    moveSelectedItems(selectedCategoriesLst, availableCategoriesLst, $('#selectedCategories'), $('#availableCategories'));
});

$(document).ready(function () {

    // Call the functions to render available pages and categories
    renderAvailablePages(availablePagesLst);
    renderAvailableCategories(availableCategoriesLst);
});
