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
        link.href = '/UserProfile';
        link.className = 'nav-link';

        const icon = document.createElement('i');
        icon.className = 'far fa-circle nav-icon';

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
            populateSidebarforPages(pages);
        }
        catch(error) {
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

            const icon = document.createElement('i');
            icon.className = 'far fa-circle nav-icon';

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
            const categories = await response.json();
            populateSidebarforCategories(categories);
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
            link.href = category.pageUrl;
            link.className = 'nav-link';

            const icon = document.createElement('i');
            icon.className = 'far fa-circle nav-icon';

            const paragraph = document.createElement('p');

            const text = document.createTextNode(category.categoryName);

            paragraph.appendChild(text);
            link.appendChild(icon);
            link.appendChild(paragraph);
            listItem.appendChild(link);
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