document.addEventListener('DOMContentLoaded', function () {
    fetchProfiles();
    fetchPages();

    async function fetchProfiles() {
        try {
            const response = await fetch('/api/Navigation/GetCurrentProfiles');
            if (!response.ok) {
                throw new Error(`Http Error! Status : ${response.status}`);
            }
            const profiles = await response.json();
            populateSidebarforProfiles(profiles);
        }
        catch (error) {
            console.error('Fetch error: ', error);
        }
    }
    function populateSidebarforProfiles(profiles) {
        const list = document.getElementById('dynamicProfileName');
        list.innerHTML = ''; // Clear existing list items if any
        profiles.forEach(profile => {
            const listItem = document.createElement('li');
            listItem.className = 'nav-item';

            const link = document.createElement('a');
            link.href = profile.userId;
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
        });
    }

    async function fetchPages() {
        try {
            const response = await fetch('/api/Navigation/GetAllPages');
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
});