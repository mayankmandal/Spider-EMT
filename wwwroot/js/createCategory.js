$(document).ready(function () {
    function renderPageCheckboxes() {
        var container = $('#pageCheckboxContainer'); // container to append checkboxes

        // Clear the container before rendering new content
        container.empty();

        // If selectedPages is empty, render checkboxes disabled
        availablePagesLst.forEach(function (pageSite) {
            var checkboxDiv = $('<div>', { style: 'flex: 0 0 32%; display:flex; align-items:center; justify-content:flex-start; padding:5px; box-sizing:border-box; font-size:1rem;' });

            var checkbox = $('<input>', {
                type: 'checkbox',
                class: 'form-check-input',
                id: pageSite.pageId,
                name: 'SelectedPages',
                value: pageSite.pageId,
            });

            var label = $('<label>', {
                class: 'form-check-label',
                for: pageSite.pageId,
                text: pageSite.pageDescription,
                style: 'margin-left: 5px;',
            });

            checkboxDiv.append(checkbox);
            checkboxDiv.append(label);

            container.append(checkboxDiv);
        });
    }

    // Initial render of the checkboxes without selections
    renderPageCheckboxes();

    // Attach the prepare function to the form submission event
    $('form').submit(function (e) {
        e.preventDefault();

        // Initialize the list of selected pages
        selectedPagesLst = [];

        // Loop through all checkboxes and get their associated data
        $('input[name="SelectedPages"]').each(function () {
            var pageId = parseInt($(this).val());
            var isSelected = $(this).is(':checked'); // Check if the checkbox is selected
            if (isSelected) {
                // Find the corresponding data in availablePagesLst
                var selectedPage = availablePagesLst.find(page => page.pageId === pageId);
                if (selectedPage) {
                    // Add the selected page to selectedPageLst
                    selectedPagesLst.push({
                        PageId: selectedPage.pageId,
                        PageUrl: selectedPage.pageUrl,
                        PageDescription: selectedPage.pageDescription,
                        PageCatId: selectedPage.pageCatId,
                        MenuImgPath: selectedPage.menuImgPath,
                        isSelected: true
                    });
                }
            }
        });

        // Store the selected pages as JSON in a hidden field
        $('#SelectedPagesJson').val(JSON.stringify(selectedPagesLst));

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                toastr.error(response.message);
            }
        });
    });
});