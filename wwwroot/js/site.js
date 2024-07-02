/**
 * getCurrentFormattedDateTime - Returns the current date and time in a formatted string.
 * @returns {string} Formatted date and time string (e.g., "Jan 24 2024T12.30.45").
 */
function getCurrentFormattedDateTime() {
    var currentDate = new Date();
    var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var month = monthNames[currentDate.getMonth()];
    var day = String(currentDate.getDate()).padStart(2, '0');
    var year = currentDate.getFullYear();
    var hours = String(currentDate.getHours()).padStart(2, '0');
    var minutes = String(currentDate.getMinutes()).padStart(2, '0');
    var seconds = String(currentDate.getSeconds()).padStart(2, '0');

    var formattedDateTime = `${month} ${day} ${year}T${hours}.${minutes}.${seconds}`;

    return formattedDateTime;
}

function checkUniqueness(field, value, validationSpan) {
    $.ajax({
        url: '/api/Navigation/CheckUniqueness',
        type: 'GET',
        data: { field: field, value: value },
        success: function (response) {
            if (response.isUnique) {
                $(`#${field}`).removeClass('is-invalid').addClass('is-valid');
                $(`#${validationSpan}`).text(`${field} is available`).removeClass('text-danger').addClass('text-success');
            } else {
                $(`#${field}`).removeClass('is-valid').addClass('is-invalid');
                $(`#${validationSpan}`).text(`${field} is already taken. Please provide another ${field}`).removeClass('text-success').addClass('text-danger');
            }
        },
        error: function () {
            $(`#${validationSpan}`).text(`An error occurred while checking the ${field}`);
        }
    });
}

$(document).ready(function () {
    var currentYear = new Date().getFullYear();
    $('#copyright-year').text(currentYear);
    toastr.options = {
        "positionClass": "toast-bottom-right", // Set the position to right bottom
    };
});