// Function to export table data to CSV file
function exportDataToCsvCurrentPage(headers, tableTbodyTRSelector) {
    var csvContent = "data:text/csv;charset=utf-8,";
    var headerValues = Object.values(headers);
    csvContent += headerValues.join(",") + "\r\n";

    // Add data rows to the CSV content
    tableTbodyTRSelector.each(function () {
        var rowData = $(this).find('td').map(function () {
            // Remove leading " - " from Bank Name column
            if (headerValues[this.cellIndex] === 'Bank Name') {
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

function exportDataToCsvAllPages(headers, eachTableRowData) {
    var csvContent = "data:text/csv;charset=utf-8,";

    // Add headers to the CSV content
    csvContent += Object.values(headers).join(",") + "\r\n";

    // Add data rows to the CSV content
    eachTableRowData.forEach(function (eachTableRow) {
        var rowData = Object.keys(headers).map(function (header) {
            // Exclude 'bankLogoPath' from the data
            return eachTableRow[header] || '';
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
