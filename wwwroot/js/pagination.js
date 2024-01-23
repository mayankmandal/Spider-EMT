// Function to render the pagination
function renderPagination(paginationContainerSelector, totalPages) {
    if (totalPages > 1) {
        var visiblePages = 5; // Number of visible pages
        var paginationHtml = '<ul class="pagination">';
        lastPage = totalPages; //Initialize Last Page value
        // Previous button
        paginationHtml += '<li class="page-item ' + (currentPage === 1 ? 'disabled' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + (currentPage - 1) + ')">&laquo;</a></li>';

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
        paginationHtml += '<li class="page-item ' + (currentPage === totalPages ? 'disabled' : '') + '"><a class="page-link" href="javascript:void(0);" onclick="changePage(' + (currentPage + 1) + ')">&raquo;</a></li>';

        paginationHtml += '</ul>';

        paginationContainerSelector.html(paginationHtml);

    } else {
        // Hide pagination if there is only one page or less
        paginationContainerSelector.empty();
    }
}

