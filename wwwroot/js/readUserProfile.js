$(document).ready(function () {
    // Attach click event handler to <a> tag inside <li> elements
    $("ul#categoriesAccordion li[id^='category-'] a.nav-link").click(function (e) {
        e.preventDefault(); // Prevent default behavior of anchor tag

        // Get the categoryCount from the ID of the parent <li> element
        var categoryCount = $(this).parent().attr('id').split('-')[1];

        // Check if the li has the specified classes
        if ($(this).parent().hasClass("menu-is-opening") && $(this).parent().hasClass("menu-open")) {
            // Remove the specified classes from the parent <li> element
            $(this).parent().removeClass("menu-is-opening menu-open");

            // Update child <ul> style to display: none
            $(this).siblings("ul.nav-treeview").css("display", "none");
        }
    });
});