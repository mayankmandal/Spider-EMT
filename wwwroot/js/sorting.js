// Function to get the value from an object based on index
function getValueByIndex(obj, index) {
    return Object.values(obj)[index];
}

//Function to sorts data in order based on the selected column and return the sorted data.
function sortColumn(displayIndex, tobeSorteddata, isAscendingOrder) {
    // Convert display index to internal index
    var index = displayIndex + 1; // Assuming display indices start from 0

    // Update the tobeSorteddata array based on sorting
    var sorteddata = tobeSorteddata.slice(0);
    sorteddata.sort(function (a, b) {
        var aValue = getValueByIndex(a, index);
        var bValue = getValueByIndex(b, index);

        if (aValue < bValue) return isAscendingOrder ? -1 : 1;
        if (aValue > bValue) return isAscendingOrder ? 1 : -1;
        return 0;
    });
    return sorteddata;
}

// Function to handle sorting arrow indicators and highlight the sorted column
function updateSortIndicators(displayIndex, tableTHSelector, isAscendingOrder) {
    tableTHSelector.removeClass('sort-asc sort-desc sorted');

    var sortColumnIndex = displayIndex + 1; // Assuming display indices start from 0
    var sortDirection = isAscending ? 'asc' : 'desc'; // Set 'asc' or 'desc' based on the sorting direction

    // Update the sorting indicators based on the sorting direction
    if (sortColumnIndex) {
        tableTHSelector.eq(sortColumnIndex - 1)
            .addClass('sort-' + sortDirection)
            .addClass('sorted');
    }
}