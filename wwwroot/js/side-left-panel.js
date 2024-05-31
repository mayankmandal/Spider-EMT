document.addEventListener('DOMContentLoaded', function () {
    fetchUserProfile();
    fetchProfiles();
    fetchPages();
    fetchCategories();

    async function fetchProfiles() {
        try {
            const response = await fetch('/api/Navigation/GetCurrentUserProfile');
            if (!response.ok) {
                throw new Error(`Http Error! Status : ${response.status}`);
            }
            const profile = await response.json();
            populateSidebarforProfiles(profile);
        }
        catch (error) {
            console.error('Fetch error: ', error);
        }
    }
    function populateSidebarforProfiles(profile) {
        const list = document.getElementById('dynamicProfileName');
        list.innerHTML = ''; // Clear existing list items if any

        const listItem = document.createElement('li');
        listItem.className = 'nav-item';

        const link = document.createElement('a');
        link.href = '/ReadUserProfile';
        link.className = 'nav-link';
        link.target = '_self';

        const icon = document.createElement('i');
        icon.className = 'far ion-android-contact nav-icon';

        const paragraph = document.createElement('p');

        const text = document.createTextNode(profile.profileName);

        paragraph.appendChild(text);
        link.appendChild(icon);
        link.appendChild(paragraph);
        listItem.appendChild(link);
        list.appendChild(listItem);
    }

    async function fetchPages() {
        try {
            const response = await fetch('/api/Navigation/GetCurrentUserPages');
            if (!response.ok) {
                throw new Error(`Http Error! Status : ${response.status}`);
            }
            const pages = await response.json();
            pages.sort((a, b) => a.pageDescription.localeCompare(b.pageDescription));
            populateSidebarforPages(pages);
        }
        catch (error) {
            console.error('Fetch error: ', error);
        }
    }
    function populateSidebarforPages(pages) {
        const list = document.getElementById('dynamicPagesList');
        list.innerHTML = ''; // Clear existing list items if any
        pages.forEach(page => {
            const listItem = document.createElement('li');
            listItem.className = 'nav-item';

            const link = document.createElement('a');
            link.href = page.pageUrl;
            link.className = 'nav-link';
            link.target = '_self';

            const icon = document.createElement('i');
            icon.className = 'far ion-android-open nav-icon';

            const paragraph = document.createElement('p');

            const text = document.createTextNode(page.pageDescription);

            paragraph.appendChild(text);
            link.appendChild(icon);
            link.appendChild(paragraph);
            listItem.appendChild(link);
            list.appendChild(listItem);
        });
    }

    async function fetchCategories() {
        try {
            const response = await fetch('/api/Navigation/GetCurrentUserCategories');
            if (!response.ok) {
                throw new Error(`Http Error! Status : ${response.status}`);
            }
            const structureData = await response.json();
            
            // Sort the pages within each category (if not already sorted by the API)
            structureData.forEach(category => {
                category.pages.sort((a, b) => a.pageDescription.localeCompare(b.pageDescription));
            });

            // Sort the categories (if not already sorted by the API)
            structureData.sort((a, b) => a.catagoryName.localeCompare(b.catagoryName));

            populateSidebarforCategories(structureData);
        }
        catch (error) {
            console.error('Fetch error: ', error);
        }
    }
    function populateSidebarforCategories(categories) {
        const list = document.getElementById('dynamicCategoriesList');
        list.innerHTML = ''; // Clear existing list items if any
        categories.forEach(category => {
            const listItem = document.createElement('li');
            listItem.className = 'nav-item';

            const link = document.createElement('a');
            link.href = '#'; // Link to a placeholder since it's a submenu
            link.className = 'nav-link';
            link.target = '_self';
            link.innerHTML = `
                <i class="far ion-merge nav-icon"></i>
                <p>${category.catagoryName}<i class="fas fa-angle-left right"></i></p>
            `;

            const submenu = document.createElement('ul');
            submenu.className = 'nav nav-treeview ml-3';
            submenu.style.borderLeft = '1px ridge #DDDFBF';

            // Populate pages under this category
            category.pages.forEach(page => {
                const pageItem = document.createElement('li');
                pageItem.className = 'nav-item';

                const pageLink = document.createElement('a');
                pageLink.target = '_self';
                pageLink.href = page.pageUrl;
                pageLink.className = 'nav-link';
                pageLink.innerHTML = `
                    <i class="far ion-android-open nav-icon"></i>
                    <p>${page.pageDescription}</p>
                `;

                pageItem.appendChild(pageLink);
                submenu.appendChild(pageItem);
            });

            listItem.appendChild(link);
            listItem.appendChild(submenu);
            list.appendChild(listItem);
        });
    }

    async function fetchUserProfile() {
        $.ajax({
            url: '/api/Navigation/GetCurrentUser',
            type: 'GET',
            dataType: 'json',
            success: function (profile) {
                populateSidebarforUserProfiles(profile);
            },
            error: function (xhr, status, error) {
                console.error('Fetch error:', status, error);
            }
        });
    }
    function populateSidebarforUserProfiles(currentUser) {
        // Update the username in the sidebar
        $('#userNameDiv a').text(currentUser.userName);

        // Update the user image in the sidebar
        $('#userImageDiv img').attr('src', currentUser.userImgPath);
        $('#userImageDiv img').attr('alt', `Image of ${currentUser.userName}`);
    }
});