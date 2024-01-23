
// Function to export table data to Excel file
function exportDataToExcelCurrentPage(headers, tableTbodyTRSelector) {
    // Extract data from the table
    var dataRows = [];
    var headerValues = Object.values(headers);
    // Extract data rows
    tableTbodyTRSelector.each(function () {
        var rowData = $(this).find('td').map(function () {
            // Remove leading " - " from Bank Name column
            if (headerValues[this.cellIndex] === 'Bank Name') {
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
    headerValues.forEach(function (header) {
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

function exportDataToExcelAllPages(headers, eachTableRowData) {
    var xmls = '<?xml version="1.0"?>\n';
    xmls += '<ss:Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">\n';
    xmls += '<ss:Worksheet ss:Name="Sheet1">\n';
    xmls += '<ss:Table>\n';
    xmls += '<ss:Row>\n';
    Object.values(headers).forEach(function (header) {
        xmls += '<ss:Cell><ss:Data ss:Type="String">' + header + '</ss:Data></ss:Cell>\n';
    });
    xmls += '</ss:Row>\n';

    // Add data rows
    eachTableRowData.forEach(function (transaction) {
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
