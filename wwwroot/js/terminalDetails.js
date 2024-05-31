// Function to create a Leaflet map with a marker and popup
function createLeafletMap(terminalData) {
    terminalMap = L.map('mapContainer').setView([terminalData.latitude, terminalData.longitude], 18);
    terminalMap.zoomControl.setPosition('bottomright');
    // Create a layer group for markers
    terminalMarkerGroup = L.layerGroup().addTo(terminalMap);

    // Add a marker to the map
    var singleMarker = L.marker([terminalData.latitude, terminalData.longitude]);

    // Customize tooltip content with HTML
    var tooltipContent = `
        <div class="map-tooltip-content">
            <div class="map-bank-name">${terminalData.bankNameEn}</div>
            <div class="map-address-info">
                <div>${terminalData.cityAr}</div>
                <div>&nbsp;|&nbsp;</div>
                <div>${terminalData.streetAr}</div>
                <div>&nbsp;|&nbsp;</div>
                <div>${terminalData.districtAr}</div>
            </div>
        </div>`;

    // Set an offset to position the tooltip correctly
    var tooltipOffset = L.point(0, -28); // Adjust the offset as needed

    // Create a red transparent circle around the marker
    var circle = L.circle([terminalData.latitude, terminalData.longitude], {
        radius: 10, 
        color: '#e83815',
        fillOpacity: 0.4,
    });

    // Add the circle to the layer group
    circle.addTo(terminalMarkerGroup);

    singleMarker.bindTooltip(tooltipContent, { direction: 'top', permanent: true, opacity: 0.7, offset: tooltipOffset }).openTooltip();

    // Add the marker to the layer group
    singleMarker.addTo(terminalMarkerGroup);

    // Google Map Streets Layer
    var googleStreets = L.tileLayer('https://{s}.google.com/vt?lyrs=m&x={x}&y={y}&z={z}', {
        maxZoom: 22,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });

    // Google Map Hybrid Layer
    var googleHybrid = L.tileLayer('https://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 22,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });

    // Google Map Satellite Layer
    var googleSatellite = L.tileLayer('https://{s}.google.com/vt?lyrs=s&x={x}&y={y}&z={z}', {
        maxZoom: 22,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });

    // Google Map Terrain Layer
    var googleTerrain = L.tileLayer('https://{s}.google.com/vt?lyrs=p&x={x}&y={y}&z={z}', {
        maxZoom: 22,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });

    //Open Street Map Layer
    var openStreetMap = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 22,
    });

    // Layer Control
    var baseLayers = {
        "Google Street Map": googleStreets,
        "Google Satellite Map": googleSatellite,
        "Google Terrain Map": googleTerrain,
        "Google Hybrid Map": googleHybrid,
        "Open Street Map": openStreetMap,
    };

    var overLaysMarker = {
        "Marker": singleMarker
    };

    // Set Google Street Map as the default layer
    googleStreets.addTo(terminalMap);

    // Add layer control
    L.control.layers(baseLayers, overLaysMarker).addTo(terminalMap);
}

// Function to remove all previous markers
function clearMarkers() {
    // Clear previous markers if they exist
    if (terminalMarkerGroup) {
        terminalMarkerGroup.clearLayers();
    }

    // Remove the map instance
    if (terminalMap) {
        terminalMap.remove();
    }
}

//Function to retrieves and displays terminal data through an AJAX request.
function showTerminalData(terminalId) {
    $.ajax({
        url: '/api/SiteSelection/GetTerminalDetails/' + terminalId,
        type: 'GET',
        success: function (result) {
            renderTerminalDetails(result);
        },
        error: function (message) {
            console.log(message);
        }
    });
}

//Function rendering the details of a terminal in a modal.
function renderTerminalDetails(terminalResult) {
    // Clear existing content in modal body
    $('#terminalDetailsFormColumn1, #terminalDetailsFormColumn2, #terminalDetailsFormColumn3, #terminalDetailsFormColumn4').empty();

    // Define the columns
    var column1 = $('#terminalDetailsFormColumn1');
    var column2 = $('#terminalDetailsFormColumn2');
    var column3 = $('#terminalDetailsFormColumn3');
    var column4 = $('#terminalDetailsFormColumn4');

    var details1 = [
        { label: 'Term Id', value: terminalResult.termId },
        { label: 'Bank Name', value: terminalResult.bankNameEn, style: 'color: #dc6f26'},
        { label: 'City Name English', value: terminalResult.cityEn },
        { label: 'District Name', value: terminalResult.districtEn },
        { label: 'Longitude', value: terminalResult.longitude },
        { label: 'Address English', value: terminalResult.addressEn },
        { label: 'Brand Name', value: terminalResult.atmBrand },
        { label: 'Site Type', value: terminalResult.siteTypeEn },
        { label: 'Cash Center Name', value: terminalResult.cashCenterNameEn },
        { label: 'Foreign Currency Withdrawal', value: terminalResult.foreignCurrWdl },
        { label: 'Deposit', value: terminalResult.deposit },
        { label: 'Connection Type', value: terminalResult.connectionType }
    ];

    var details2 = [
        { label: 'Bank Name Arabic', value: terminalResult.bankNameAr, style: 'color: #dc6f26' },
        { label: 'Region Name', value: terminalResult.regionEn },
        { label: 'City Name Arabic', value: terminalResult.cityAr },
        { label: 'Street Name', value: terminalResult.streetEn },
        { label: 'Latitude', value: terminalResult.latitude },
        { label: 'Address Arabic', value: terminalResult.addressAr },
        { label: 'ATM Type', value: terminalResult.atmType },
        { label: 'District Arabic', value: terminalResult.districtAr },
        { label: 'Cash Center Name Arabic', value: terminalResult.cashCenterNameAr },
        { label: 'Site Type Arabic', value: terminalResult.siteTypeAr },
        { label: 'Street Arabic', value: terminalResult.streetAr },
        { label: 'Region Arabic', value: terminalResult.regionAr },
    ];

    details1.forEach(detail => {
        column1.append($('<label>', { 'class': 'form-label py-0', 'style': 'margin-bottom: 12px;', text: detail.label }));
        column2.append($('<input>', { 'type': 'text', 'class': 'form-control mb-1 py-1', 'value': detail.value, 'disabled': 'disabled', 'style' : detail.style || '' }));
    });

    details2.forEach(detail => {
        column3.append($('<label>', { 'class': 'form-label py-0 ms-2', 'style': 'margin-bottom: 12px;', text: detail.label }));
        column4.append($('<input>', { 'type': 'text', 'class': 'form-control mb-1 py-1', 'value': detail.value, 'disabled': 'disabled', 'style': detail.style || '' }));
    });

    // Show the modal
    $('#terminalDetailsModal').modal('show');

    // Check if the map container already has a map instance
    var existingMap = L.DomUtil.get('mapContainer');

    // If a map already exists, remove it before initializing a new one
    if (existingMap) {
        existingMap._leaflet_id = null;
    }

    // Initialize the map
    createLeafletMap(terminalResult);

    // Listen for the Bootstrap modal shown event and call invalidateSize when the modal is fully shown
    $('#terminalDetailsModal').on('shown.bs.modal', function () {
        terminalMap.invalidateSize();
    });
}

//Function for Terminal Details Columns to closes the modal and clears its content.
function closeForm() {

    // To update the map with new coordinates
    clearMarkers();

    // Clear the content in modal elements
    $('#terminalDetailsFormColumn1').empty();
    $('#terminalDetailsFormColumn2').empty();
    $('#terminalDetailsFormColumn3').empty();
    $('#terminalDetailsFormColumn4').empty();

    // Hide the modal
    $('#terminalDetailsModal').modal('hide');
}