
// Initialize datepickers
$('.datepicker').datepicker({
    format: 'dd M yyyy',
    autoclose: true,
    todayHighlight: true,
});

function validateDateRange() {
    var startDateString = $('#startDate').val();
    var endDateString = $('#endDate').val();

    if (startDateString && endDateString) {
        var startDate = new Date(startDateString);
        var endDate = new Date(endDateString);

        // Check if startDate is greater than or equal to endDate
        if (startDate >= endDate) {
            $('#formValidationError').text('From Date must be less than To date');
            // Highlight the input fields
            $('#startDate').addClass('is-invalid');
            $('#endDate').addClass('is-invalid');
            return false;
        }

        var timeDiff = Math.abs(endDate.getTime() - startDate.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        if (diffDays > 90) {
            $('#formValidationError').text('Date range must be within 90 days');
            // Highlight the input fields
            $('#startDate').addClass('is-invalid');
            $('#endDate').addClass('is-invalid');
            return false;
        } else {
            $('#formValidationError').text('');
            $('#startDate').removeClass('is-invalid');
            $('#endDate').removeClass('is-invalid');
            return true;
        }
    }
    return true;
}

function fetchChartToDisplay() {

    if (!myViewModelList || myViewModelList.length < 1) {
        Highcharts.chart('chartContainer', {
            title: {
                text: 'No Data Available to Display',
                align: 'center',
                verticalAlign: 'middle'
            },
            credits: {
                enabled: false
            }
        });
        return;
    }

    // Prepare data for Highcharts
    switch (transactionType) {
        case "CW":
            transactionType = "Cash Withdrawal";
            break;
        case "MS":
            transactionType = "Mini Statement";
            break;
        case "BI":
            transactionType = "Business Inquiry";
            break;
        default:
            transactionType = "Unknown Transaction Type";
            break;
    }

    var columnColors = [
        '#1f78b4', '#33a02c', '#e31a1c', '#ff7f00', '#6a3d9a',
        '#b15928', '#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f',
        '#cab2d6', '#ffff99', '#1f78b4', '#33a02c', '#e31a1c',
        '#ff7f00', '#6a3d9a', '#b15928', '#a6cee3', '#b2df8a',
        '#fb9a99', '#fdbf6f', '#cab2d6', '#ffff99', '#1f78b4',
        '#33a02c', '#e31a1c', '#ff7f00', '#6a3d9a', '#b15928',
        '#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f', '#cab2d6',
        '#ffff99', '#1f78b4', '#33a02c', '#e31a1c', '#ff7f00',
        '#6a3d9a', '#b15928', '#a6cee3', '#b2df8a', '#fb9a99',
        '#fdbf6f', '#cab2d6', '#ffff99', '#1f78b4', '#33a02c',
        '#e31a1c', '#ff7f00', '#6a3d9a', '#b15928', '#a6cee3',
        '#b2df8a', '#fb9a99', '#fdbf6f', '#cab2d6', '#ffff99',
        '#1f78b4', '#33a02c', '#e31a1c', '#ff7f00', '#6a3d9a',
        '#b15928', '#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f',
        '#cab2d6', '#ffff99', '#1f78b4', '#33a02c', '#e31a1c',
        '#ff7f00', '#6a3d9a', '#b15928', '#a6cee3', '#b2df8a',
        '#fb9a99', '#fdbf6f', '#cab2d6', '#ffff99', '#1f78b4',
        '#33a02c', '#e31a1c', '#ff7f00', '#6a3d9a', '#b15928',
        '#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f', '#cab2d6',
        '#ffff99', '#1f78b4', '#33a02c', '#e31a1c', '#ff7f00',
        '#6a3d9a', '#b15928', '#a6cee3', '#b2df8a', '#fb9a99',
        '#fdbf6f', '#cab2d6', '#ffff99'
    ];

    var seriesData = myViewModelList.map(function (item, index) {
        return {
            name: item.bankNameEn,
            y: item.averageAmount,
            color: columnColors[index % columnColors.length]
        };
    });

    // Extract bank names from the seriesData for xAxis categories
    var bankCategoriesData = seriesData.map(dataPoint => dataPoint.name);

    // Create the Highcharts bar graph
    Highcharts.chart('chartContainer', {
        chart: {
            type: 'column',
            zoomType: 'xy',
            height: '800',
            borderWidth: 1,
            shadow: {
                color: 'rgba(0,0,0,0.2)',
                offsetX: 3,
                offsetX: 3,
                opacity: 0.2,
                width: 10
            }
        },
        title: {
            text: 'Total Collection By Bank',
        },
        subtitle: {
            text: 'Banks Collection showing total ' + transactionType + ' for each banks'
        },
        xAxis: {
            categories: bankCategoriesData,
            crosshair: true,
            accessibility: {
                description: 'Banks'
            },
            labels: {
                style: {
                    whiteSpace: 'normal'
                },
                useHTML: true
            },
            title: {
                text: 'Banks'
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Total Collections (in SAR)'
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                },
                
            },
        },
        series: [
            {
                name: 'Total Collections (in SAR)',
                data: seriesData
            }
        ]
    });
}
document.addEventListener('DOMContentLoaded', function () {
    // Check if myViewModelList is not defined or is null
    if (typeof myViewModelList === 'undefined' || myViewModelList === null || myViewModelList.length < 1) {
        return;
    }
    fetchChartToDisplay();
});

document.getElementById('resetButton').addEventListener('click', function () {
    document.getElementById('startDate').value = '';
    document.getElementById('endDate').value = '';
    myViewModelList = null;
    currentDate = null;
    Days90Before = null;
    transactionType = null;
    fetchChartToDisplay();
    $('#formValidationError').text('');
    $('#startDate').removeClass('is-invalid');
    $('#endDate').removeClass('is-invalid');
});

$(document).ready(function () {
    $('#startDate').datepicker('setDate', Days90Before);
    $('#endDate').datepicker('setDate', currentDate);

    // Added Event Listener  to validate date range on change
    $('#startDate, #endDate').on('changeDate', function () {
        validateDateRange();
    });
});

// Event Listener to Submit Form
$('form').submit(function (e) {
    if (!validateDateRange()) {
        e.preventDefault(); // Prevent form submission if validation fails
    }
})
