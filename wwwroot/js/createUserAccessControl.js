$(document).ready(function () {
    // This function is called before form submission
    function prepareFormSubmission() {
        // Initialize the list of selected pages
        selectedPagesLst = [];

        // Get the selected profile ID
        var selectedProfileId = $('#ProfileId').val();

        // Get the selected profile name based on the selected option
        var selectedProfileName = $('#ProfileId option:selected').text();

        // Set the hidden fields with ProfileId and ProfileName
        $('#SelectedProfileId').val(selectedProfileId);
        $('#SelectedProfileName').val(selectedProfileName);

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
    }

    // Attach the prepare function to the form submission event
    $('form').submit(function (e) {
        prepareFormSubmission();
    });
});