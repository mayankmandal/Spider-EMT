// Initialize default current page and rows per page and last page value
var rowsPerPage = 10;
var currentPage = 1;
var lastPage = -1;
var viewTypeCheck = 'default';

// Initialize sortedTransactions array
var sortedTransactions = [];
// Initialize filteredTransactions array
var filteredTransactions = [];

// Variables to track sorting state
var sortedColumnIndex = 5; // Index of currently sorted column
var isAscending = false; // Sorting order (true for ascending, false for descending)

// Initialize Default Date Search Filter
var isDateFilterApplied = false;

// Initialize Default Search Filter
var isSearchFilterApplied = false;

// Initialize Top View Filter
var isApplyTopViewFilter = false;

// Set default values for start and end dates
// start date - current date
var currentDate = new Date();

// End date: 90 days before the current date
var Days90Before = new Date();
Days90Before.setDate(currentDate.getDate() - 90);

// Add headers to the CSV/XLS content
var headers = {
    "bankNameEn": "Bank Name",
    "termId": "Terminal Id",
    "regionEn": "Region",
    "cityEn": "City",
    "txnDate": "Transaction Date",
    "totalCWCount": "Withdrawals",
    "totalCWFeeAmount": "Withdrawals Fee Amount",
    "totalBICount": "Balance Inquiry",
    "totalMSCount": "Mini Statement",
    "totalBI_MSFeeAmount": "Inquiry & Statement Fee Amount",
    "totalTxnOnUsCount": "Total Txn On Us",
    "totalPayedAmount": "Total Payed Amount"
};

var isDragging = false;
var startX, scrollLeft;

var terminalMap = null;
var terminalMarkerGroup = null;

function validateDateRange() {
    var startDateString = $('#startDate').val();
    var endDateString = $('#endDate').val();

    if (startDateString && endDateString) {
        var startDate = new Date(startDateString);
        var endDate = new Date(endDateString);

        // Check if startDate is greater than or equal to endDate
        if (startDate >= endDate) {
            $('#dateCheckValidationError').text('From Date must be less than To date');
            // Highlight the input fields
            $('#startDate').addClass('is-invalid');
            $('#endDate').addClass('is-invalid');
            return false;
        }

        var timeDiff = Math.abs(endDate.getTime() - startDate.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        if (diffDays > 90) {
            $('#dateCheckValidationError').text('Date range must be within 90 days');
            // Highlight the input fields
            $('#startDate').addClass('is-invalid');
            $('#endDate').addClass('is-invalid');
            return false;
        } else {
            $('#dateCheckValidationError').text('');
            $('#startDate').removeClass('is-invalid');
            $('#endDate').removeClass('is-invalid');
            return true;
        }
    }
    return true;
}

// Function to clear transactions data
function clearTransactionsData() {

    sortedTransactions = [];
    currentPage = 1;
    lastPage = -1;

    // Clear the search input field
    $('#filterInput').val('');
    isSearchFilterApplied = false;
    isApplyTopViewFilter = false;
    $('#searchfilterValidationMsg').text('');
    $('#applySearchFilter').removeClass('border-danger');

    // Clear the existing data in tbody
    $('#bankTransactionSummaryTable tbody').empty();

    //Hide Pagination since no transactions
    $('#paginationContainer').empty();
}

//Function renders the bank transaction summary table based on the provided data.
function renderBankTransactionSummaryTable(data) {
    var tableBody = $('#bankTransactionSummaryTable tbody');

    // Clear existing content
    tableBody.empty();

    // Loop through the data and append rows to the table
    data.forEach(function (banktransaction) {
        var row = $('<tr>').css('text-align', 'right');

        // Add table cells with data
        row.append($('<td>').css('text-align', 'left').html('<img src="' + banktransaction.bankLogoPath + '" alt="' + banktransaction.bankNameEn + '" style="max-width: 70px; height:auto;" loading="lazy" /> - ' + banktransaction.bankNameEn));
        row.append($('<td>').css('text-align', 'left').html('<a href="javascript:void(0);" onclick="showTerminalData(\'' + banktransaction.termId + '\')">' + banktransaction.termId + '</a>'));
        row.append($('<td>').css('text-align', 'left').text(banktransaction.regionEn));
        row.append($('<td>').css('text-align', 'left').text(banktransaction.cityEn));
        var txnDate = banktransaction.txnDate;
        var dateObject = new Date(txnDate);
        var formattedDate = dateObject.toLocaleDateString('en-GB', { year: 'numeric', month: 'short', day: '2-digit' });
        var formattedTime = dateObject.toLocaleTimeString('en-GB');
        var formattedDateTime = formattedDate + ' ' + formattedTime;
        row.append($('<td>').css('text-align', 'left').text(formattedDateTime));
        row.append($('<td>').text(banktransaction.totalCWCount));
        row.append($('<td>').css({ 'color': 'green', 'font-weight': 'bold' }).text(banktransaction.totalCWFeeAmount));
        row.append($('<td>').text(banktransaction.totalBICount));
        row.append($('<td>').text(banktransaction.totalMSCount));
        row.append($('<td>').css({ 'color': 'green', 'font-weight': 'bold' }).text(banktransaction.totalBI_MSFeeAmount));
        row.append($('<td>').text(banktransaction.totalTxnOnUsCount));
        row.append($('<td>').css({ 'color': 'red', 'font-weight': 'bold' }).text(banktransaction.totalPayedAmount));

        // Append the row to the table body
        tableBody.append(row);
    });
}

//Function renders a specific page of the bank transaction summary table.
function renderTable(pageNumber, data) {
    var pageData = data;
    if (viewTypeCheck !== 'top') {
        var startIndex = (pageNumber - 1) * rowsPerPage;
        var endIndex = startIndex + rowsPerPage;
        pageData = data.slice(startIndex, endIndex);
    }
    renderBankTransactionSummaryTable(pageData);

    if (data.length > 0) {
        $('#downloadCointainer').show();
    }
    else {
        $('#downloadCointainer').hide();
    }
}

// Function to open the overlay
function openDirectPageInput() {
    $("#directPageInputOverlay").val('');
    $('.overlay-directPage').show();
    $('#directPageInputOverlay').focus();
}

// Function to close the overlay
function closeDirectPageInput() {
    $('#directPage-validationMessage').text('').removeClass('text-danger fw-bold');
    $('.overlay-directPage').hide();
}

// Function to handle direct page navigation from the overlay
function goToPageFromInput() {
    var inputPage = parseInt($('#directPageInputOverlay').val(), 10);
    if (!isNaN(inputPage) && inputPage > 0 && inputPage <= lastPage) {
        changePage(inputPage);
        // Hide the overlay after navigating
        closeDirectPageInput();
    } else {
        // Show validation message on the same view
        $('#directPage-validationMessage').text('Invalid page number. Please enter a valid page number.').addClass('text-danger fw-bold');
    }
}

// Function to handle rows per page change
function onRowsPerPageChange() {
    rowsPerPage = parseInt($('#rowsPerPage').val(), 10);
    currentPage = 1;
    renderTable(currentPage, sortedTransactions);
    // Check if there is data to display pagination
    if (sortedTransactions.length > rowsPerPage) {
        renderPagination($('#paginationContainer'),Math.ceil(sortedTransactions.length / rowsPerPage));
        // Show pagination only if there is enough data to paginate
        $('#paginationContainer').show();
    } else {
        // Hide pagination if there is not enough data
        $('#paginationContainer').empty();
    }
}

// Function to change the current page
function changePage(pageNumber) {
    currentPage = pageNumber;
    renderTable(currentPage, sortedTransactions);
    // Check if there is data to display pagination
    if (sortedTransactions.length > rowsPerPage) {
        renderPagination($('#paginationContainer'),Math.ceil(sortedTransactions.length / rowsPerPage));
        // Show pagination only if there is enough data to paginate
        $('#paginationContainer').show();
    } else {
        // Hide pagination if there is not enough data
        $('#paginationContainer').empty();
    }
}

// Function to fetch data from the API and initialize the page
function fetchData(selectedBankId, startDate, endDate) {
    try{
        var data = $.ajax({
            url: (selectedBankId === "All") ? '/api/SiteSelection/GetBankTransactionSummary' :
                '/api/SiteSelection/GetFilteredBankTransactionSummary',
            type: 'GET',
            async: false,
            data: {
                bankId: selectedBankId,
                startDate: startDate ? startDate.toISOString() : null,
                endDate: endDate ? endDate.toISOString() : null
            }
        }).responseJSON;
    } catch (error) {
        console.error('Error fetching data', error);
        // Hide loading overlay on error
        setTimeout(function () {
            $('#loadingOverlay').hide();
        }, 100);
        clearTransactionsData(); // Clear data on error
        return;
    }
    isDateFilterApplied = true; // Set flag to indicate filter is applied
    sortedTransactions = data;
}

// Function to switch to Top View
function switchToTopView() {
    // Hide elements for Default View

    $('#rowsPerPageContainer, #searchfiltertransaction').hide();
    $('#paginationContainer').empty();
    $('#bankTransactionSummaryTable tbody').empty();
    $('#bankTransactionSummaryTable th').removeClass('sort-asc sort-desc sorted');

    $('#rowCountInputValidationMsg').text('');
    $('#rowCountInput').removeClass('is-invalid');
    $('#applyTopViewFilterValidationMsg').text('');
    $('#btnApplyTopViewFilter').removeClass('border-danger');

    // Show elements for Top View
    $('#additionalTopFiltersContainer').show();

    if (isSearchFilterApplied === true) {
        sortedTransactions = filteredTransactions;
        filteredTransactions = [];
    }

    // Populate column dropdown with header names
    var columnDropdown = $('#columnDropdown');
    columnDropdown.empty();
    $.each(headers, function (key,value) {
        columnDropdown.append($('<option>', { value: key, text: value }));
    });
    $('#columnDropdown').val('totalCWCount');

    // Hide loading overlay after data is loaded
    setTimeout(function () {
        $('#loadingOverlay').hide();
    }, 100);
}

// Function to switch to Default View
function switchToDefaultView() {
    // Show elements for Default View
    $('#additionalTopFiltersContainer').hide();
    $('#rowsPerPageContainer, #searchfiltertransaction').show();
    if (isApplyTopViewFilter === true) {
        sortedTransactions = filteredTransactions;
        filteredTransactions = [];
    }
    isApplyTopViewFilter = false; //Search Filter is applied

    // Sort by "withdrawalsCount" in descending order by default
    sortedTransactions = sortColumn(sortedColumnIndex, sortedTransactions, isAscending);

    // Update the rendering with sorted data
    renderTable(currentPage, sortedTransactions);

    // Highlight the sorted column with arrow
    updateSortIndicators(sortedColumnIndex, $('#bankTransactionSummaryTable th'), isAscending);

    // Check if there is data to display pagination
    if (sortedTransactions.length > rowsPerPage) {
        renderPagination($('#paginationContainer'),Math.ceil(sortedTransactions.length / rowsPerPage));
        // Show pagination only if there is enough data to paginate
        $('#paginationContainer').show();
    } else {
        // Hide pagination if there is not enough data
        $('#paginationContainer').empty();
    }

    // Hide loading overlay after data is loaded
    setTimeout(function () {
        $('#loadingOverlay').hide();
    }, 100);
}

//All Event Listeners

// Initialize datepickers
$('.datepicker').datepicker({
    format: 'dd M yyyy',
    autoclose: true,
    todayHighlight: true
});

// Event listener for the Date Apply button
$('#applyDateFilterBtn').click(function () {

    $('#startDateValidationMsg').text('');
    $('#endDateValidationMsg').text('');
    $('#startDate').removeClass('is-invalid');
    $('#endDate').removeClass('is-invalid');

    // Get the selected start and end dates
    var startDate = $('#startDate').datepicker('getDate');
    var endDate = $('#endDate').datepicker('getDate');

    // Check if both start and end dates are selected
    if (!startDate || !endDate) {
        if (!startDate && endDate) {
            // Display validation message
            $('#startDateValidationMsg').text('From Date is required');
            $('#startDate').addClass('is-invalid');
            return; // Stop further processing
        }
        else if (startDate && !endDate) {
            // Display validation message
            $('#endDateValidationMsg').text('To Date is required');
            $('#endDate').addClass('is-invalid');

            return; // Stop further processing
        }
        else if (!startDate && !endDate) {
            // Display validation message
            $('#startDateValidationMsg').text('From Date is required');
            $('#endDateValidationMsg').text('To Date is required');
            $('#startDate').addClass('is-invalid');
            $('#endDate').addClass('is-invalid');
            return; // Stop further processing
        }
    }

    // Validate date range
    if (!validateDateRange()) {
        return; // Stop further processing if validation fails
    }
    // Show loading overlay
    $('#loadingOverlay').show();

    clearTransactionsData(); // Clear the existing data in the transactions and sortedTransactions array and initialize default current page last page value

    // Get the selected bank
    var selectedBank = $('#allBankName').val();

    // Fetch data with date filter
    fetchData(selectedBank, startDate, endDate);

    if (viewTypeCheck === 'default') {
        switchToDefaultView();
    } else if (viewTypeCheck === 'top') {
        switchToTopView();
    }
});

// Event listener for the Search Apply button
$('#applySearchFilter').click(function () {
    if (isSearchFilterApplied === false) {
        // Get the search keyword
        var keyword = $('#filterInput').val().toLowerCase();

        // Do nothing if the input is empty
        if (!keyword) {
            return;
        }
        filteredTransactions = sortedTransactions;
        // Filter the complete transactions based on the keyword
        sortedTransactions = sortedTransactions.filter(function (transaction) {
            var combinedData = Object.values(transaction).join(' ').toLowerCase();
            return combinedData.includes(keyword);
        });

        // Since single value search based, hence first page only
        currentPage = 1;
        isSearchFilterApplied = true; //Search Filter is applied
        // Render the table with the filtered data
        renderTable(currentPage, sortedTransactions);

        // Highlight the sorted column with arrow
        updateSortIndicators(sortedColumnIndex, $('#bankTransactionSummaryTable th'), isAscending);

        // Check if there is data to display pagination
        if (sortedTransactions.length > rowsPerPage) {
            renderPagination($('#paginationContainer'), Math.ceil(sortedTransactions.length / rowsPerPage));
            // Show pagination only if there is enough data to paginate
            $('#paginationContainer').show();
        } else {
            // Hide pagination if there is not enough data
            $('#paginationContainer').empty();
            $('#bankTransactionSummaryTable th').removeClass('sort-asc sort-desc sorted');
        }
    } else {
        $('#searchfilterValidationMsg').text('Clear the Existing applied Text Search');
        $('#applySearchFilter').addClass('border-danger');
    }
});

// Event listener for the Search Clear button
$('#clearSearchFilter').click(function () {
    if (isSearchFilterApplied === true) {
        // Clear the search input field
        $('#filterInput').val('');
        isSearchFilterApplied = false;
        $('#searchfilterValidationMsg').text('');
        $('#applySearchFilter').removeClass('border-danger');
        if (filteredTransactions.length > 0) {
            sortedTransactions = filteredTransactions;
            filteredTransactions = [];
        }
        

        // Render the table with the original data
        renderTable(currentPage, sortedTransactions);

        // Highlight the sorted column with arrow
        updateSortIndicators(sortedColumnIndex, $('#bankTransactionSummaryTable th'), isAscending);

        // Check if there is data to display pagination
        if (sortedTransactions.length > rowsPerPage) {
            renderPagination($('#paginationContainer'),Math.ceil(sortedTransactions.length / rowsPerPage));
            // Show pagination only if there is enough data to paginate
            $('#paginationContainer').show();
        } else {
            // Hide pagination if there is not enough data
            $('#paginationContainer').empty();
        }
    }
});

// Event listener for the Bank Name dropdown change
$('#allBankName').change(function () {

    var selectedBank = $(this).val();
    if (selectedBank !== 'None') {
        $('#allBankNameDateValidationMsg').text('');
        $('#tblStartDate').show();
        $('#tblEndDate').show();
        $('#tblApplyFilterBtn').show();
        $('#viewTypeContainer').show();
        $('#rowsPerPageContainer').show();
        $('#filterInput').val('');
        
        if (viewTypeCheck === 'default') {
            switchToDefaultView();
        } else if (viewTypeCheck === 'top') {
            switchToTopView();
        }
    } else {
        $('#allBankNameDateValidationMsg').text('Select All or Any Bank from the below');
        $('#dateCheckValidationError').text('');
        $('#tblStartDate').hide();
        $('#tblEndDate').hide();
        $('#tblApplyFilterBtn').hide();
        $('#viewTypeContainer').hide();
        $('#rowsPerPageContainer').hide();
        $('#searchfiltertransaction').hide();
        $('#downloadCointainer').hide();
        $('#additionalTopFiltersContainer').hide();
        $('#bankTransactionSummaryTable th').removeClass('sort-asc sort-desc sorted');
        clearTransactionsData(); // Clear the existing data in the transactions and sortedTransactions array and initialize default current page last page value
        isDateFilterApplied = false; // Reset the filter flag
        return;
    }
});

// Event listeners for column header clicks
$('#bankTransactionSummaryTable th').on('click', function () {
    // Get the index of the clicked header
    var index = $('#bankTransactionSummaryTable th').index(this);

    // Check if the same column is clicked again
    if (sortedColumnIndex === index) {
        // Toggle the sorting order
        isAscending = !isAscending;
    } else {
        // For a different column, default to descending order
        isAscending = false;
    }

    // Update the sorted column index
    sortedColumnIndex = index;

    // Sort the column
    sortedTransactions = sortColumn(index, sortedTransactions, isAscending);

    // Update the rendering with sorted data
    renderTable(currentPage, sortedTransactions);

    // Highlight the sorted column with arrow
    updateSortIndicators(index, $('#bankTransactionSummaryTable th'), isAscending);
});

// Event listener for the Download buttons
$('#downloadButton1').click(function () {
    // Call a function to handle data export in CSV Format for current page
    exportDataToCsvCurrentPage(headers, $("#bankTransactionSummaryTable tbody tr"));
});

$('#downloadButton2').click(function () {
    // Call a function to handle data export in CSV Format for all pages
    exportDataToCsvAllPages(headers, sortedTransactions);
});

$('#downloadButton3').click(function () {
    // Call a function to handle data export in Excel for current page
    exportDataToExcelCurrentPage(headers, $("#bankTransactionSummaryTable tbody tr"));
});

$('#downloadButton4').click(function () {
    // Call a function to handle data export in Excel for all pages
    exportDataToExcelAllPages(headers, sortedTransactions);
});

// Event listener for Bootstrap modal hidden event
$('#terminalDetailsModal').on('hidden.bs.modal', function () {
    // Call closeForm() to perform cleanup
    closeForm();
});

// Event Listner for radio buttons to switch between views
$('input[name="viewType"]').on('change', function () {
    viewTypeCheck = $(this).val();
    if (viewTypeCheck === 'default') {
        switchToDefaultView();
    } else if (viewTypeCheck === 'top') {
        switchToTopView();
    }
});

// Event Listener for Apply Button in Top View
$('#btnApplyTopViewFilter').on('click', function () {
    if (isApplyTopViewFilter === false) {
        $('#rowCountInputValidationMsg').text('');
        $('#rowCountInput').removeClass('is-invalid');

        var rowCount = $('#rowCountInput').val();
        var selectedColumn = $('#columnDropdown').val();
        var sortOrder = $('input[name="sortOrder"]:checked').val();

        if (rowCount > sortedTransactions.length) {
            // Display validation message
            $('#rowCountInputValidationMsg').text('Count should be less than and equal to ' + sortedTransactions.length);
            $('#rowCountInput').addClass('is-invalid');
            return; // Stop further processing
        }

        // Get the index of the Selected Column
        var columnIndex = Object.keys(headers).findIndex(function (key) {
            return key === selectedColumn;
        })

        filteredTransactions = sortedTransactions;

        if (sortOrder === 'asc') {
            isAscending = true;
        } else {
            isAscending = false;
        }

        // Sort the column
        sortedTransactions = sortColumn(columnIndex, sortedTransactions, isAscending);

        isApplyTopViewFilter = true; //Top View Filter is applied

        // Slice the data to get the first rowCount items
        sortedTransactions = sortedTransactions.slice(0, rowCount);

        // Update the rendering with sorted data
        renderBankTransactionSummaryTable(sortedTransactions);

        if (sortedTransactions.length > 0) {
            $('#downloadCointainer').show();
        }
        else {
            $('#downloadCointainer').hide();
        }

        // Update the sorted column index
        sortedColumnIndex = columnIndex;

        // Highlight the sorted column with arrow
        updateSortIndicators(columnIndex, $('#bankTransactionSummaryTable th'), isAscending);

    }
    else if (isApplyTopViewFilter === true) {
        $('#applyTopViewFilterValidationMsg').text('Clear the Existing applied Top View Filter');
        $('#btnApplyTopViewFilter').addClass('border-danger');
        return;
    }
});

// Event Listener for Clear Button in Top View
$('#btnClearTopViewFilter').on('click', function () {

    $('#rowCountInputValidationMsg').text('');
    $('#rowCountInput').removeClass('is-invalid');
    $('#applyTopViewFilterValidationMsg').text('');
    $('#btnApplyTopViewFilter').removeClass('border-danger');

    $('#rowCountInput').val('20');
    $('#columnDropdown').val('totalCWCount');
    $('input[name="sortOrder"][value="desc"]').prop('checked', true);

    if (isApplyTopViewFilter === true) {
        // Clear input fields and reset to default values
        
        isApplyTopViewFilter = false;
        sortedTransactions = filteredTransactions;
        filteredTransactions = [];
        $('#bankTransactionSummaryTable tbody').empty();
        $('#bankTransactionSummaryTable th').removeClass('sort-asc sort-desc sorted');
    }
});

// Enable horizontal scrolling when dragging inside the content area
$('#tableContainer')
.mousedown(function (e) {
    isDragging = true;
    startX = e.pageX - $('#tableContainer').offset().left;
    scrollLeft = $('#tableContainer').scrollLeft();
})
.mousemove(function (e) {
    if (!isDragging) return;
    var x = e.pageX - $('#tableContainer').offset().left;
    var walk = (x - startX) * 1.5; // Adjust the sensitivity as needed
    $('#tableContainer').scrollLeft(scrollLeft - walk);
})
.mouseup(function () {
    isDragging = false;
})
.mouseleave(function () {
    isDragging = false;
});

$(document).ready(function () {
    // Trigger initial data load on page load
    $('#startDate').datepicker('setDate', Days90Before);
    $('#endDate').datepicker('setDate', currentDate);
    // Added Event Listener  to validate date range on change
    $('#startDate, #endDate').on('changeDate', function () {
        validateDateRange();
    });

    // Show loading overlay
    $('#loadingOverlay').show();

    clearTransactionsData(); // Clear the existing data in the transactions and sortedTransactions array and initialize default current page last page value

    fetchData("All", Days90Before, currentDate);

    if (viewTypeCheck === 'default') {
        switchToDefaultView();
    } else if (viewTypeCheck === 'top') {
        switchToTopView();
    }
});
