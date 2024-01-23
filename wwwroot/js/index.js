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
    $('#applySearchFilter').removeClass('disabled');

    // Clear the existing data in tbody
    $('#bankTransactionSummaryTable tbody').empty();

    //Hide Pagination since no transactions
    $('#paginationContainer').empty();
}

// Function to handle sorting arrow indicators and highlight the sorted column
function updateSortIndicators(displayIndex) {
    $('#bankTransactionSummaryTable th').removeClass('sort-asc sort-desc sorted');

    var sortColumnIndex = displayIndex + 1; // Assuming display indices start from 0
    var sortDirection = isAscending ? 'asc' : 'desc'; // Set 'asc' or 'desc' based on the sorting direction

    // Update the sorting indicators based on the sorting direction
    if (sortColumnIndex) {
        $('#bankTransactionSummaryTable th:nth-child(' + sortColumnIndex + ')')
            .addClass('sort-' + sortDirection)
            .addClass('sorted');
    }
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

// Function to render the pagination
function renderPagination(totalPages) {
    if (totalPages > 1) {
        var visiblePages = 5; // Number of visible pages
        var paginationHtml = '<ul class="pagination">';
        lastPage = totalPages; //Initialize Last Page value
        // Previous button
        paginationHtml += '<li class="page-item ' + (currentPage === 1 ? 'disabled' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + (currentPage - 1) + ')">Prev</a></li>';

        // First page
        paginationHtml += '<li class="page-item ' + (currentPage === 1 ? 'active' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(1)">1</a></li>';

        // Pages in the middle
        var startPage = Math.max(2, currentPage - Math.floor(visiblePages / 2));
        var endPage = Math.min(totalPages - 1, startPage + visiblePages - 2);

        for (var i = startPage; i <= endPage; i++) {
            paginationHtml += '<li class="page-item ' + (i === currentPage ? 'active' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + i + ')">' + i + '</a></li>';
        }

        // Display input field for direct page navigation
        paginationHtml += '<li class="page-item">';
        paginationHtml += '<a class="page-link" id="directPageInput" onclick="openDirectPageInput()">...</a>';
        paginationHtml += '</li>';

        // Last page
        paginationHtml += '<li class="page-item ' + (currentPage === totalPages ? 'active' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + totalPages + ')">' + totalPages + '</a></li>';

        // Next button
        paginationHtml += '<li class="page-item ' + (currentPage === totalPages ? 'disabled' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + (currentPage + 1) + ')">Next</a></li>';

        paginationHtml += '</ul>';

        $('#paginationContainer').html(paginationHtml);

        // Initialize popover for small input
        $('#directPageInput').popover({
            content: '<button class="btn btn-primary" onclick="openBigInput()">Go</button>',
            html: true,
            trigger: 'manual',
            placement: 'bottom',
            container: 'body'
        });

    } else {
        // Hide pagination if there is only one page or less
        $('#paginationContainer').empty();
    }
}

// Function to handle rows per page change
function onRowsPerPageChange() {
    rowsPerPage = parseInt($('#rowsPerPage').val(), 10);
    currentPage = 1;
    renderTable(currentPage, sortedTransactions);
    // Check if there is data to display pagination
    if (sortedTransactions.length > rowsPerPage) {
        renderPagination(Math.ceil(sortedTransactions.length / rowsPerPage));
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
        renderPagination(Math.ceil(sortedTransactions.length / rowsPerPage));
        // Show pagination only if there is enough data to paginate
        $('#paginationContainer').show();
    } else {
        // Hide pagination if there is not enough data
        $('#paginationContainer').empty();
    }
}

// Function to get the value from an object based on index
function getValueByIndex(obj, index) {
    return Object.values(obj)[index];
}

//Function sorts the bank transactions based on the selected column and updates the table.
function sortColumn(displayIndex, transactionsdata) {
    // Convert display index to internal index
    var index = displayIndex + 1; // Assuming display indices start from 0

    // Update the sortedTransactions array based on sorting
    sortedTransactions = transactionsdata.slice(0);
    sortedTransactions.sort(function (a, b) {
        var aValue = getValueByIndex(a, index);
        var bValue = getValueByIndex(b, index);

        if (aValue < bValue) return isAscending ? -1 : 1;
        if (aValue > bValue) return isAscending ? 1 : -1;
        return 0;
    });
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
        $('#loadingOverlay').hide();
        clearTransactionsData(); // Clear data on error
        return;
    }
    isDateFilterApplied = true; // Set flag to indicate filter is applied
    sortedTransactions = data;
}

//Function to retrieves and displays terminal data through an AJAX request.
function showTerminalData(terminalId) {
    $.ajax({
        url: '/api/SiteSelection/GetTerminalDetails/' + terminalId,
        type: 'GET',
        success: function (result) {
            renderTerminalDetails(result);
        },
        error: function (message) {
            console.log(message);
        }
    });
}

//Function rendering the details of a terminal in a modal.
function renderTerminalDetails(terminalResult) {
    // Clear existing content in modal body
    $('#terminalDetailsFormColumn1').empty();
    $('#terminalDetailsFormColumn2').empty();
    $('#terminalDetailsFormColumn3').empty();
    $('#terminalDetailsFormColumn4').empty();

    // Define the columns
    var column1 = $('#terminalDetailsFormColumn1');
    var column2 = $('#terminalDetailsFormColumn2');
    var column3 = $('#terminalDetailsFormColumn3');
    var column4 = $('#terminalDetailsFormColumn4');

    // Create label and input elements for each property in Column 1
    var label11 = $('<label>', { 'for': 'TermId', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Term Id' });
    var input11 = $('<input>', { 'type': 'text', 'id': 'TermId', 'class': 'form-control mb-1 py-1', 'value': terminalResult.termId, 'disabled': 'disabled' });
    column1.append(label11).append('<br>');
    column2.append(input11);

    var label12 = $('<label>', { 'for': 'BankNameEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Bank Name' });
    var input12 = $('<input>', { 'type': 'text', 'id': 'BankNameEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.bankNameEn, 'disabled': 'disabled', 'style': 'color: #dc6f26' });
    column1.append(label12).append('<br>');
    column2.append(input12);

    var label13 = $('<label>', { 'for': 'CityEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'City Name English' });
    var input13 = $('<input>', { 'type': 'text', 'id': 'CityEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cityEn, 'disabled': 'disabled' });
    column1.append(label13).append('<br>');
    column2.append(input13);

    var label14 = $('<label>', { 'for': 'DistrictEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'District Name' });
    var input14 = $('<input>', { 'type': 'text', 'id': 'DistrictEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.districtEn, 'disabled': 'disabled' });
    column1.append(label14).append('<br>');
    column2.append(input14);

    var label15 = $('<label>', { 'for': 'Longitude', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Longitude' });
    var input15 = $('<input>', { 'type': 'text', 'id': 'Longitude', 'class': 'form-control mb-1 py-1', 'value': terminalResult.longitude, 'disabled': 'disabled' });
    column1.append(label15).append('<br>');
    column2.append(input15);

    var label16 = $('<label>', { 'for': 'AddressEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Address English' });
    var input16 = $('<input>', { 'type': 'text', 'id': 'AddressEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.addressEn, 'disabled': 'disabled' });
    column1.append(label16).append('<br>');
    column2.append(input16);

    var label17 = $('<label>', { 'for': 'AtmBrand', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Brand Name' });
    var input17 = $('<input>', { 'type': 'text', 'id': 'AtmBrand', 'class': 'form-control mb-1 py-1', 'value': terminalResult.atmBrand, 'disabled': 'disabled' });
    column1.append(label17).append('<br>');
    column2.append(input17);

    var label18 = $('<label>', { 'for': 'SiteTypeEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Site Type' });
    var input18 = $('<input>', { 'type': 'text', 'id': 'SiteTypeEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.siteTypeEn, 'disabled': 'disabled' });
    column1.append(label18).append('<br>');
    column2.append(input18);

    var label19 = $('<label>', { 'for': 'CashCenterNameEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Cash Center Name' });
    var input19 = $('<input>', { 'type': 'text', 'id': 'CashCenterNameEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cashCenterNameEn, 'disabled': 'disabled' });
    column1.append(label19).append('<br>');
    column2.append(input19);

    var label20 = $('<label>', { 'for': 'ForeignCurrWdl', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Foreign Currency Withdrawal' });
    var input20 = $('<input>', { 'type': 'text', 'id': 'ForeignCurrWdl', 'class': 'form-control mb-1 py-1', 'value': terminalResult.foreignCurrWdl, 'disabled': 'disabled' });
    column1.append(label20).append('<br>');
    column2.append(input20);

    var label01 = $('<label>', { 'for': 'Deposit', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Deposit' });
    var input01 = $('<input>', { 'type': 'text', 'id': 'Deposit', 'class': 'form-control mb-1 py-1', 'value': terminalResult.deposit, 'disabled': 'disabled' });
    column1.append(label01).append('<br>');
    column2.append(input01);

    var label02 = $('<label>', { 'for': 'ConnectionType', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Connection Type' });
    var input02 = $('<input>', { 'type': 'text', 'id': 'ConnectionType', 'class': 'form-control mb-1 py-1', 'value': terminalResult.connectionType, 'disabled': 'disabled' });
    column1.append(label02).append('<br>');
    column2.append(input02);


    // Create label and input elements for each property in Column 2
    var label21 = $('<label>', { 'for': 'BankNameAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Bank Name Arabic' });
    var input21 = $('<input>', { 'type': 'text', 'id': 'BankNameAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.bankNameAr, 'disabled': 'disabled', 'style': 'color: #dc6f26' });
    column3.append(label21).append('<br>');
    column4.append(input21);

    var label22 = $('<label>', { 'for': 'RegionEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Region Name' });
    var input22 = $('<input>', { 'type': 'text', 'id': 'RegionEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.regionEn, 'disabled': 'disabled' });
    column3.append(label22).append('<br>');
    column4.append(input22);

    var label23 = $('<label>', { 'for': 'CityAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'City Name Arabic' });
    var input23 = $('<input>', { 'type': 'text', 'id': 'CityAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cityAr, 'disabled': 'disabled' });
    column3.append(label23).append('<br>');
    column4.append(input23);

    var label24 = $('<label>', { 'for': 'StreetEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Street Name' });
    var input24 = $('<input>', { 'type': 'text', 'id': 'StreetEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.streetEn, 'disabled': 'disabled' });
    column3.append(label24).append('<br>');
    column4.append(input24);

    var label25 = $('<label>', { 'for': 'Latitude', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Lattitude' });
    var input25 = $('<input>', { 'type': 'text', 'id': 'Latitude', 'class': 'form-control mb-1 py-1', 'value': terminalResult.latitude, 'disabled': 'disabled' });
    column3.append(label25).append('<br>');
    column4.append(input25);

    var label26 = $('<label>', { 'for': 'AddressAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Address Arabic' });
    var input26 = $('<input>', { 'type': 'text', 'id': 'AddressAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.addressAr, 'disabled': 'disabled' });
    column3.append(label26).append('<br>');
    column4.append(input26);

    var label27 = $('<label>', { 'for': 'AtmType', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'ATM Type' });
    var input27 = $('<input>', { 'type': 'text', 'id': 'AtmType', 'class': 'form-control mb-1 py-1', 'value': terminalResult.atmType, 'disabled': 'disabled' });
    column3.append(label27).append('<br>');
    column4.append(input27);

    var label28 = $('<label>', { 'for': 'DistrictAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'District Arabic' });
    var input28 = $('<input>', { 'type': 'text', 'id': 'DistrictAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.districtAr, 'disabled': 'disabled' });
    column3.append(label28).append('<br>');
    column4.append(input28);

    var label29 = $('<label>', { 'for': 'CashCenterNameAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Cash Center Name Arabic' });
    var input29 = $('<input>', { 'type': 'text', 'id': 'CashCenterNameAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cashCenterNameAr, 'disabled': 'disabled' });
    column3.append(label29).append('<br>');
    column4.append(input29);

    var label30 = $('<label>', { 'for': 'SiteTypeAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Site Type Arabic' });
    var input30 = $('<input>', { 'type': 'text', 'id': 'SiteTypeAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.siteTypeAr, 'disabled': 'disabled' });
    column3.append(label30).append('<br>');
    column4.append(input30);

    var label31 = $('<label>', { 'for': 'StreetAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Street Arabic' });
    var input31 = $('<input>', { 'type': 'text', 'id': 'StreetAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.streetAr, 'disabled': 'disabled' });
    column3.append(label31).append('<br>');
    column4.append(input31);

    var label32 = $('<label>', { 'for': 'RegionAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Region Arabic' });
    var input32 = $('<input>', { 'type': 'text', 'id': 'RegionAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.regionAr, 'disabled': 'disabled' });
    column3.append(label32).append('<br>');
    column4.append(input32);

    // Show the modal
    $('#terminalDetailsModal').modal('show');
}

//Function for Terminal Details Columns to closes the modal and clears its content.
function closeForm() {
    // Clear the content in modal elements
    $('#terminalDetailsFormColumn1').empty();
    $('#terminalDetailsFormColumn2').empty();
    $('#terminalDetailsFormColumn3').empty();
    $('#terminalDetailsFormColumn4').empty();

    // Hide the modal
    $('#terminalDetailsModal').modal('hide');
}

// Function to export table data to CSV file
function exportDataToCsvCurrentPage() {
    var csvContent = "data:text/csv;charset=utf-8,";

    // Add headers to the CSV content
    var headers = [
        "Bank Name",
        "Terminal Id",
        "Region",
        "City",
        "Transaction Date",
        "Withdrawals",
        "Withdrawals Fee Amount",
        "Balance Inquiry",
        "Mini Statement",
        "Inquiry & Statement Fee Amount",
        "Total Txn On Us",
        "Total Payed Amount"
    ];
    csvContent += headers.join(",") + "\r\n";

    // Add data rows to the CSV content
    $("#bankTransactionSummaryTable tbody tr").each(function () {
        var rowData = $(this).find('td').map(function () {
            // Remove leading " - " from Bank Name column
            if (headers[this.cellIndex] === 'Bank Name') {
                return $(this).text().replace(/^ - /, '');
            }
            return $(this).text();
        }).get();
        csvContent += rowData.join(",") + "\r\n";
    });

    // Create a Blob and generate a download link
    var encodedUri = encodeURI(csvContent);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "transaction_data.csv");
    document.body.appendChild(link);

    // Trigger a click on the link to start the download
    link.click();

    // Remove the link from the DOM
    document.body.removeChild(link);
}

function exportDataToCsvAllPages() {
    var csvContent = "data:text/csv;charset=utf-8,";

    // Define headers for the CSV content
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

    // Add headers to the CSV content
    csvContent += Object.values(headers).join(",") + "\r\n";

    // Add data rows to the CSV content
    sortedTransactions.forEach(function (transaction) {
        var rowData = Object.keys(headers).map(function (header) {
            // Exclude 'bankLogoPath' from the data
            return transaction[header] || '';
        });
        csvContent += rowData.join(",") + "\r\n";
    });

    // Create a Blob and generate a download link
    var encodedUri = encodeURI(csvContent);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "all_transactions_data.csv");
    document.body.appendChild(link);

    // Trigger a click on the link to start the download
    link.click();

    // Remove the link from the DOM
    document.body.removeChild(link);
}

// Function to export table data to Excel file
function exportDataToExcelCurrentPage() {
    // Extract data from the table
    var headers = [];
    var dataRows = [];

    // Extract headers
    $("#bankTransactionSummaryTable th").each(function () {
        headers.push($(this).text());
    });

    // Extract data rows
    $("#bankTransactionSummaryTable tbody tr").each(function () {
        var rowData = $(this).find('td').map(function () {
            // Remove leading " - " from Bank Name column
            if (headers[this.cellIndex] === 'Bank Name') {
                return $(this).text().replace(/^ - /, '');
            }
            return $(this).text();
        }).get();
        dataRows.push(rowData);
    });

    // Create XML content for Excel
    var xmls = '<?xml version="1.0"?>\n';
    xmls += '<ss:Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">\n';
    xmls += '<ss:Worksheet ss:Name="Sheet1">\n';
    xmls += '<ss:Table>\n';

    // Add headers
    xmls += '<ss:Row>\n';
    headers.forEach(function (header) {
        xmls += '<ss:Cell><ss:Data ss:Type="String">' + header + '</ss:Data></ss:Cell>\n';
    });
    xmls += '</ss:Row>\n';

    // Add data rows
    dataRows.forEach(function (rowData) {
        xmls += '<ss:Row>\n';
        rowData.forEach(function (value) {
            xmls += '<ss:Cell><ss:Data ss:Type="String">' + value + '</ss:Data></ss:Cell>\n';
        });
        xmls += '</ss:Row>\n';
    });

    xmls += '</ss:Table>\n';
    xmls += '</ss:Worksheet>\n';
    xmls += '</ss:Workbook>';

    // Create a Blob and generate a download link
    var blob = new Blob([xmls], { type: 'application/vnd.ms-excel' });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = 'currentPageData.xls';

    // Trigger a click on the link to start the download
    document.body.appendChild(link);
    link.click();

    // Remove the link from the DOM
    document.body.removeChild(link);
}

function exportDataToExcelAllPages() {
    var data = sortedTransactions;

    var xmls = '<?xml version="1.0"?>\n';
    xmls += '<ss:Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">\n';
    xmls += '<ss:Worksheet ss:Name="Sheet1">\n';
    xmls += '<ss:Table>\n';

    // Define headers for the Excel content
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

    xmls += '<ss:Row>\n';
    Object.values(headers).forEach(function (header) {
        xmls += '<ss:Cell><ss:Data ss:Type="String">' + header + '</ss:Data></ss:Cell>\n';
    });
    xmls += '</ss:Row>\n';

    // Add data rows
    data.forEach(function (transaction) {
        xmls += '<ss:Row>\n';
        Object.keys(headers).forEach(function (key) {
            // Exclude 'bankLogoPath' from the data
            xmls += '<ss:Cell><ss:Data ss:Type="String">' + (key !== 'bankLogoPath' ? transaction[key] : '') + '</ss:Data></ss:Cell>\n';
        });
        xmls += '</ss:Row>\n';
    });

    xmls += '</ss:Table>\n';
    xmls += '</ss:Worksheet>\n';
    xmls += '</ss:Workbook>';

    // Create a Blob and generate a download link
    var blob = new Blob([xmls], { type: 'application/vnd.ms-excel' });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = 'all_transactions_data.xls';

    // Trigger a click on the link to start the download
    document.body.appendChild(link);
    link.click();

    // Remove the link from the DOM
    document.body.removeChild(link);
}

// Function to get header columns (replace this with your actual implementation)
function getHeaderColumnsFilterOn() {
    return [
        { value: 'bankNameEn', label: 'Bank Name' },
        { value: 'termId', label: 'Terminal Id' },
        { value: 'regionEn', label: 'Region' },
        { value: 'cityEn', label: 'City' },
        { value: 'txnDate', label: 'Transaction Date' },
        { value: 'totalCWCount', label: 'Withdrawals #' },
        { value: 'totalCWFeeAmount', label: 'Withdrawals Fee Amount' },
        { value: 'balanceInquiryCount', label: 'Balance Inquiry #' },
        { value: 'totalBICount', label: 'Mini Statement #' },
        { value: 'totalBI_MSFeeAmount', label: 'Inquiry & Statement Fee Amount' },
        { value: 'totalTxnOnUsCount', label: 'Total Txn On Us' },
        { value: 'totalPayedAmount', label: 'Total Payed Amount' }
    ];
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
    isApplyTopViewFilter = false; //Search Filter is applied

    // Populate column dropdown with header names
    var columnOptions = getHeaderColumnsFilterOn();
    var columnDropdown = $('#columnDropdown');
    columnDropdown.empty();
    $.each(columnOptions, function (index, column) {
        columnDropdown.append($('<option>', { value: column.value, text: column.label }));
    });
    $('#columnDropdown').val('totalCWCount');

    // Hide loading overlay after data is loaded
    $('#loadingOverlay').hide();
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
    sortColumn(sortedColumnIndex, sortedTransactions);

    // Update the rendering with sorted data
    renderTable(currentPage, sortedTransactions);

    // Highlight the sorted column with arrow
    updateSortIndicators(sortedColumnIndex);

    // Check if there is data to display pagination
    if (sortedTransactions.length > rowsPerPage) {
        renderPagination(Math.ceil(sortedTransactions.length / rowsPerPage));
        // Show pagination only if there is enough data to paginate
        $('#paginationContainer').show();
    } else {
        // Hide pagination if there is not enough data
        $('#paginationContainer').empty();
    }

    // Hide loading overlay after data is loaded
    $('#loadingOverlay').hide();
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
        $('#applySearchFilter').addClass('disabled');
        // Render the table with the filtered data
        renderTable(currentPage, sortedTransactions);

        // Check if there is data to display pagination
        if (sortedTransactions.length > rowsPerPage) {
            renderPagination(Math.ceil(sortedTransactions.length / rowsPerPage));
            // Show pagination only if there is enough data to paginate
            $('#paginationContainer').show();
        } else {
            // Hide pagination if there is not enough data
            $('#paginationContainer').empty();
        }
    }
});

// Event listener for the Search Clear button
$('#clearSearchFilter').click(function () {
    if (isSearchFilterApplied === true) {
        // Clear the search input field
        $('#filterInput').val('');
        isSearchFilterApplied = false;
        $('#applySearchFilter').removeClass('disabled');
        sortedTransactions = filteredTransactions;
        filteredTransactions = [];
        // Render the table with the original data
        renderTable(currentPage, sortedTransactions);
        // Check if there is data to display pagination
        if (sortedTransactions.length > rowsPerPage) {
            renderPagination(Math.ceil(sortedTransactions.length / rowsPerPage));
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
    sortColumn(index, sortedTransactions);

    // Update the rendering with sorted data
    renderTable(currentPage, sortedTransactions);

    // Highlight the sorted column with arrow
    updateSortIndicators(index);
});

// Event listener for the Download buttons
$('#downloadButton1').click(function () {
    // Call a function to handle data export in CSV Format for current page
    exportDataToCsvCurrentPage();
});

$('#downloadButton2').click(function () {
    // Call a function to handle data export in CSV Format for all pages
    exportDataToCsvAllPages();
});

$('#downloadButton3').click(function () {
    // Call a function to handle data export in Excel for current page
    exportDataToExcelCurrentPage();
});

$('#downloadButton4').click(function () {
    // Call a function to handle data export in Excel for all pages
    exportDataToExcelAllPages();
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
        var columnIndex = getHeaderColumnsFilterOn().findIndex(function (column) {
            return column.value === selectedColumn;
        })

        filteredTransactions = sortedTransactions;

        if (sortOrder === 'asc') {
            isAscending = true;
        } else {
            isAscending = false;
        }

        // Sort the column
        sortColumn(columnIndex, sortedTransactions);

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
        updateSortIndicators(columnIndex);
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
