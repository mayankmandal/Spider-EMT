$(document).ready(function () {
    // Toggle categories view
    $('#categoryViewDiv').on('click', function (event) {
        event.preventDefault();
        $(this).toggleClass('menu-open').find('.nav-treeview').toggleClass('show');
    });

    // Toggle pages view
    $('#pageViewDiv').on('click', function (event) {
        event.preventDefault();
        $(this).toggleClass('menu-open');
    });
});