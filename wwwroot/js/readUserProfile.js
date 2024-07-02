$(document).ready(function () {
    // Add click event listener to all nav items with the id starting with 'category-'
    $('[id^="category-"] > a').on('click', function (event) {
        event.preventDefault(); // Prevent default behavior of the link
        event.stopPropagation(); // Stop the event from bubbling up

        // Get the parent .nav-item of the clicked link
        var navItem = $(this).parent('.nav-item');

        // Toggle class
        if (navItem.hasClass('menu-is-opening') && navItem.hasClass('menu-open')) {
            navItem.removeClass('menu-is-opening menu-open');
        } else {
            navItem.addClass('menu-is-opening menu-open');
        }

        // Toggle the style of the corresponding ul element
        var ulElement = navItem.find('ul.nav-treeview').first();
        if (ulElement.is(':visible')) {
            ulElement.hide();
        } else {
            ulElement.css({
                'border-left': '1px ridge #DDDFBF',
                'display': 'block',
                'height': '259.2px',
                'padding-top': '0px',
                'margin-top': '0px',
                'padding-bottom': '0px',
                'margin-bottom': '0px'
            });
        }
    });
});