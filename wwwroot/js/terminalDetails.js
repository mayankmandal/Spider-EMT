// Function to create a Leaflet map with a marker and popup
function createLeafletMap(terminalData) {
    terminalMap = L.map('mapContainer').setView([terminalData.latitude, terminalData.longitude], 18);
    terminalMap.zoomControl.setPosition('bottomright');
    // Create a layer group for markers
    terminalMarkerGroup = L.layerGroup().addTo(terminalMap);

    // Add a marker to the map
    var singleMarker = L.marker([terminalData.latitude, terminalData.longitude]);

    // Customize tooltip content with HTML
    var tooltipContent = `<div class="map-tooltip-content">
                         <div><b>${terminalData.addressAr}</b></div>
                         <div>${terminalData.districtAr}</div>
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

    // Layer Control
    var baseLayers = {
        "Google Street Map": googleStreets,
        "Google Satellite Map": googleSatellite,
        "Google Terrain Map": googleTerrain,
        "Google Hybrid Map": googleHybrid,
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
    $('#terminalDetailsFormColumn1').empty();
    $('#terminalDetailsFormColumn2').empty();
    $('#terminalDetailsFormColumn3').empty();
    $('#terminalDetailsFormColumn4').empty();

    // Define the columns
    var column1 = $('#terminalDetailsFormColumn1');
    var column2 = $('#terminalDetailsFormColumn2');
    var column3 = $('#terminalDetailsFormColumn3');
    var column4 = $('#terminalDetailsFormColumn4');

    // Create label and input elements for each property in Column 1
    var label11 = $('<label>', { 'for': 'TermId', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Term Id' });
    var input11 = $('<input>', { 'type': 'text', 'id': 'TermId', 'class': 'form-control mb-1 py-1', 'value': terminalResult.termId, 'disabled': 'disabled' });
    column1.append(label11).append('<br>');
    column2.append(input11);

    var label12 = $('<label>', { 'for': 'BankNameEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Bank Name' });
    var input12 = $('<input>', { 'type': 'text', 'id': 'BankNameEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.bankNameEn, 'disabled': 'disabled', 'style': 'color: #dc6f26' });
    column1.append(label12).append('<br>');
    column2.append(input12);

    var label13 = $('<label>', { 'for': 'CityEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'City Name English' });
    var input13 = $('<input>', { 'type': 'text', 'id': 'CityEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cityEn, 'disabled': 'disabled' });
    column1.append(label13).append('<br>');
    column2.append(input13);

    var label14 = $('<label>', { 'for': 'DistrictEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'District Name' });
    var input14 = $('<input>', { 'type': 'text', 'id': 'DistrictEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.districtEn, 'disabled': 'disabled' });
    column1.append(label14).append('<br>');
    column2.append(input14);

    var label15 = $('<label>', { 'for': 'Longitude', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Longitude' });
    var input15 = $('<input>', { 'type': 'text', 'id': 'Longitude', 'class': 'form-control mb-1 py-1', 'value': terminalResult.longitude, 'disabled': 'disabled' });
    column1.append(label15).append('<br>');
    column2.append(input15);

    var label16 = $('<label>', { 'for': 'AddressEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Address English' });
    var input16 = $('<input>', { 'type': 'text', 'id': 'AddressEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.addressEn, 'disabled': 'disabled' });
    column1.append(label16).append('<br>');
    column2.append(input16);

    var label17 = $('<label>', { 'for': 'AtmBrand', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Brand Name' });
    var input17 = $('<input>', { 'type': 'text', 'id': 'AtmBrand', 'class': 'form-control mb-1 py-1', 'value': terminalResult.atmBrand, 'disabled': 'disabled' });
    column1.append(label17).append('<br>');
    column2.append(input17);

    var label18 = $('<label>', { 'for': 'SiteTypeEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Site Type' });
    var input18 = $('<input>', { 'type': 'text', 'id': 'SiteTypeEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.siteTypeEn, 'disabled': 'disabled' });
    column1.append(label18).append('<br>');
    column2.append(input18);

    var label19 = $('<label>', { 'for': 'CashCenterNameEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Cash Center Name' });
    var input19 = $('<input>', { 'type': 'text', 'id': 'CashCenterNameEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cashCenterNameEn, 'disabled': 'disabled' });
    column1.append(label19).append('<br>');
    column2.append(input19);

    var label20 = $('<label>', { 'for': 'ForeignCurrWdl', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Foreign Currency Withdrawal' });
    var input20 = $('<input>', { 'type': 'text', 'id': 'ForeignCurrWdl', 'class': 'form-control mb-1 py-1', 'value': terminalResult.foreignCurrWdl, 'disabled': 'disabled' });
    column1.append(label20).append('<br>');
    column2.append(input20);

    var label01 = $('<label>', { 'for': 'Deposit', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Deposit' });
    var input01 = $('<input>', { 'type': 'text', 'id': 'Deposit', 'class': 'form-control mb-1 py-1', 'value': terminalResult.deposit, 'disabled': 'disabled' });
    column1.append(label01).append('<br>');
    column2.append(input01);

    var label02 = $('<label>', { 'for': 'ConnectionType', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Connection Type' });
    var input02 = $('<input>', { 'type': 'text', 'id': 'ConnectionType', 'class': 'form-control mb-1 py-1', 'value': terminalResult.connectionType, 'disabled': 'disabled' });
    column1.append(label02).append('<br>');
    column2.append(input02);


    // Create label and input elements for each property in Column 2
    var label21 = $('<label>', { 'for': 'BankNameAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Bank Name Arabic' });
    var input21 = $('<input>', { 'type': 'text', 'id': 'BankNameAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.bankNameAr, 'disabled': 'disabled', 'style': 'color: #dc6f26' });
    column3.append(label21).append('<br>');
    column4.append(input21);

    var label22 = $('<label>', { 'for': 'RegionEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Region Name' });
    var input22 = $('<input>', { 'type': 'text', 'id': 'RegionEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.regionEn, 'disabled': 'disabled' });
    column3.append(label22).append('<br>');
    column4.append(input22);

    var label23 = $('<label>', { 'for': 'CityAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'City Name Arabic' });
    var input23 = $('<input>', { 'type': 'text', 'id': 'CityAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cityAr, 'disabled': 'disabled' });
    column3.append(label23).append('<br>');
    column4.append(input23);

    var label24 = $('<label>', { 'for': 'StreetEn', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Street Name' });
    var input24 = $('<input>', { 'type': 'text', 'id': 'StreetEn', 'class': 'form-control mb-1 py-1', 'value': terminalResult.streetEn, 'disabled': 'disabled' });
    column3.append(label24).append('<br>');
    column4.append(input24);

    var label25 = $('<label>', { 'for': 'Latitude', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Lattitude' });
    var input25 = $('<input>', { 'type': 'text', 'id': 'Latitude', 'class': 'form-control mb-1 py-1', 'value': terminalResult.latitude, 'disabled': 'disabled' });
    column3.append(label25).append('<br>');
    column4.append(input25);

    var label26 = $('<label>', { 'for': 'AddressAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Address Arabic' });
    var input26 = $('<input>', { 'type': 'text', 'id': 'AddressAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.addressAr, 'disabled': 'disabled' });
    column3.append(label26).append('<br>');
    column4.append(input26);

    var label27 = $('<label>', { 'for': 'AtmType', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'ATM Type' });
    var input27 = $('<input>', { 'type': 'text', 'id': 'AtmType', 'class': 'form-control mb-1 py-1', 'value': terminalResult.atmType, 'disabled': 'disabled' });
    column3.append(label27).append('<br>');
    column4.append(input27);

    var label28 = $('<label>', { 'for': 'DistrictAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'District Arabic' });
    var input28 = $('<input>', { 'type': 'text', 'id': 'DistrictAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.districtAr, 'disabled': 'disabled' });
    column3.append(label28).append('<br>');
    column4.append(input28);

    var label29 = $('<label>', { 'for': 'CashCenterNameAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Cash Center Name Arabic' });
    var input29 = $('<input>', { 'type': 'text', 'id': 'CashCenterNameAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.cashCenterNameAr, 'disabled': 'disabled' });
    column3.append(label29).append('<br>');
    column4.append(input29);

    var label30 = $('<label>', { 'for': 'SiteTypeAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Site Type Arabic' });
    var input30 = $('<input>', { 'type': 'text', 'id': 'SiteTypeAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.siteTypeAr, 'disabled': 'disabled' });
    column3.append(label30).append('<br>');
    column4.append(input30);

    var label31 = $('<label>', { 'for': 'StreetAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Street Arabic' });
    var input31 = $('<input>', { 'type': 'text', 'id': 'StreetAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.streetAr, 'disabled': 'disabled' });
    column3.append(label31).append('<br>');
    column4.append(input31);

    var label32 = $('<label>', { 'for': 'RegionAr', 'class': 'form-label py-0', 'style': ' margin-bottom: 12px;', text: 'Region Arabic' });
    var input32 = $('<input>', { 'type': 'text', 'id': 'RegionAr', 'class': 'form-control mb-1 py-1', 'value': terminalResult.regionAr, 'disabled': 'disabled' });
    column3.append(label32).append('<br>');
    column4.append(input32);

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