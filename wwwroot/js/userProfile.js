// Sample data for available and selected pages
let selectedPages = [];

// Sample data for available and selected Categories
let selectedCategories = [];

// Function to display selected pages
function displaySelectedPages() {
    const selectedPagesDiv = $('#selectedPages');
    selectedPagesDiv.empty();
    selectedPages.forEach(page => {
        const pageDiv = $('<div></div>').text(page).addClass('page selected');
        pageDiv.on('click', function () {
            $(this).toggleClass('selected');
        });
        selectedPagesDiv.append(pageDiv);
    });
}

// Function to move page from available to selected
function selectPage(page) {
    const index = availablePages.indexOf(page);
    if (index !== -1) {
        availablePages.splice(index, 1);
        selectedPages.push(page);
        displaySelectedPages();

        $(`#selectedPages .page:contains(${page})`).addClass('selected-page');
    }
}

// Function to move page from selected to available
function deselectPage(page) {
    const index = selectedPages.indexOf(page);
    if (index !== -1) {
        selectedPages.splice(index, 1);
        availablePages.push(page);
        displaySelectedPages();

        $(`#selectedPages .page:contains(${page})`).removeClass('selected-page');
    }
}

// Event listener for moving page to the left
$('#moveLeftBtnPage').on('click', () => {
    $('.selected').each(function () {
        const pageToMove = $(this).text();
        deselectPage(pageToMove);
    });
    $('.page').removeClass('selected');
});

// Event listener for moving page to the right
$('#moveRightBtnPage').on('click', () => {
    $('.selected').each(function () {
        const pageToMove = $(this).text();
        selectPage(pageToMove);
    });
    $('.page').removeClass('selected');
});

// Function to display selected categories
function displaySelectedCategories() {
    const selectedCategoriesDiv = $('#selectedCategories');
    selectedCategoriesDiv.empty();
    selectedCategories.forEach(category => {
        const categoryDiv = $('<div></div>').text(category).addClass('category selected');
        categoryDiv.on('click', function () {
            $(this).toggleClass('selected');
        });
        selectedCategoriesDiv.append(categoryDiv);
    });
}

// Function to move category from available to selected
function selectCategory(category) {
    const index = availableCategories.indexOf(category);
    if (index !== -1) {
        availableCategories.splice(index, 1);
        selectedCategories.push(category);
        displaySelectedCategories();

        $(`#selectedCategories .category:contains(${category})`).addClass('selected-category');
    }
}

// Function to move category from selected to available
function deselectCategory(category) {
    const index = selectedCategories.indexOf(category);
    if (index !== -1) {
        selectedCategories.splice(index, 1);
        availableCategories.push(category);
        displaySelectedCategories();

        $(`#selectedCategories .category:contains(${category})`).removeClass('selected-category');
    }
}

// Event listener for moving category to the left
$('#moveLeftBtnCategory').on('click', () => {
    $('.selected').each(function () {
        const categoryToMove = $(this).text();
        deselectCategory(categoryToMove);
    });
    $('.category').removeClass('selected');
});

// Event listener for moving category to the right
$('#moveRightBtnCategory').on('click', () => {
    $('.selected').each(function () {
        const categoryToMove = $(this).text();
        selectCategory(categoryToMove);
    });
    $('.category').removeClass('selected');
});

$(document).ready(function () {
    
});
