var map = null;
var infoboxOptions = {width:100, 
    height: 50, 
    title: "", 
    description: "", 
    showPointer: false,     
    offset: new Microsoft.Maps.Point(0, 0),
    visible: false
};

var center = new Microsoft.Maps.Location(49.233333, 28.483333);
var standardPushpinOptions = { icon: '../icons/small-blue-pushpin.png'};
var standardPushpinIcon = '../icons/small-blue-pushpin.png';
var newPushpinOptions = { icon: '../icons/small-red-new-pushpin.png' };
var selectedPushpinOptions = { icon: '../icons/small-orange-pushpin.png'};
var mapOptions = null;
var newOutletSelection = false;
var mapClickHandler = null;
var newPushpin = null;
var newOutletLatitude;
var newOutletLongitude;

var myLatitude;
var myLongitude;

function GetMap() {
    // Initialize the map
    map = new Microsoft.Maps.Map(document.getElementById('bingMap'), {
        credentials: "AoYrBEi7XfpIxwGJVUWNnHqbnTwLSSLfOJsMibyQy9aSKR7gGiwdxbFrGj41uSeA",
        center: center,
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        zoom: 14
    });
    mapOptions = map.getOptions();    
}

function CenterMap(location) {
    mapOptions.center = location;
    map.setView(mapOptions);
}

function ToggleMapClick() {
    newOutletSelection = !newOutletSelection;
    if (newOutletSelection) {
        mapClickHandler = Microsoft.Maps.Events.addHandler(map, 'click', MapClick);        
    }
    else {
        UnsibscribeFromMapClick();
    }
}

function UnsibscribeFromMapClick() {
    newOutletSelection = false;    
    Microsoft.Maps.Events.removeHandler(mapClickHandler);
    map.entities.remove(newPushpin);
    newPushpin = null;
    ClearPushpinDisplayInfo();
}

function ClearPushpinDisplayInfo() {    
    $("#NewOutletName").val("");
    $("#NewOutletRating").val("");
    $("#NewOutletDescription").val("");
    $("#NewOutletURL").val("");
}

function MapClick(e) {
    if (e.targetType != "map") { return; }            

    var point = new Microsoft.Maps.Point(e.getX(), e.getY());
    var loc = e.target.tryPixelToLocation(point);    

    map.entities.remove(newPushpin);
    newPushpin = AddPushpinWoHandler(loc.latitude, loc.longitude, newPushpinOptions);
    newOutletLatitude = loc.latitude;
    newOutletLongitude = loc.longitude;
}

function AddPushpinWoHandler(latitude, longitude, pushpinOptions, outletEntity) {
    var pinLocation = new Microsoft.Maps.Location(latitude, longitude);
    var pushpin = new Microsoft.Maps.Pushpin(
        pinLocation,
        pushpinOptions);

    if (typeof outletEntity != "undefined") {
        var descriptionText = outletEntity.Description != null && outletEntity.Description.length > 0 ? outletEntity.Description : "";

        //elipsis overflow inplementation
        if (descriptionText.length > 80) {
            descriptionText = descriptionText.substring(0, 80) + "...";
        }
        var description =  "</br>" + descriptionText;
        var image = outletEntity.ImageUrl != null && outletEntity.ImageUrl.length > 0 ? "<br><img class=\x22infoboxIcon\x22 src=\x22" + outletEntity.ImageUrl + "\x22></img>" : "";
        var rating = outletEntity.Votes > 0 ? "<br>" + dictionary.rating[langId] + outletEntity.OutletRating + "/5 (" + outletEntity.Votes + dictionary.rated[langId] : "";
        var infoboxHtml = "<div class=\x22infobox\x22><b>" + outletEntity.EntityName + "</b>" + image + description + rating + "<br><br>" + dictionary.clickPin[langId] + "<div>";        

        pushpin._infobox = new Microsoft.Maps.Infobox(pinLocation,{htmlContent: infoboxHtml});

        Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', ShowInfobox);
        Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', HideInfobox);
    }

    map.entities.push(pushpin);
    return pushpin;
}

function ShowInfobox(pushpin) {        
    map.entities.push(pushpin.target._infobox);    
}

function HideInfobox(pushpin) {    
    map.entities.remove(pushpin.target._infobox);
}

function AddNewPushpin(clickHanlder) {    
    Microsoft.Maps.Events.addHandler(newPushpin, 'click', clickHanlder);    
}

function ClearMap() {    
    map.entities.clear();    
}

function PaintSelectedPushpin(pushpinLatitude, pushpinLongitude) {
    var pins = map.entities;
    var options;
    var pushpin;
    var selectedPushpin;
    var length = pins.getLength();    
    for (var i = 0; i < length; i++) {
        pushpin = pins.get(i);
        if (typeof pushpin._location == "undefined") { continue; }
        if (pushpin._location.latitude == pushpinLatitude
                && pushpin._location.longitude == pushpinLongitude)
        {   
            selectedPushpin = pushpin;
            continue;
        }
        options = pushpin.getIcon() != newPushpinOptions.icon ?
                                    standardPushpinOptions:
                                    newPushpinOptions;
        
        pushpin.setOptions(options);        
    }

    //paint selected pushpin in the last turn for it to be on top.
    map.entities.remove(selectedPushpin);
    selectedPushpin.setOptions(selectedPushpinOptions);
    map.entities.push(selectedPushpin);    
}

function CenterMapByEntitiesRect() {
    var pins = map.entities;
    var length = pins.getLength();

    if (length == 0) { return; }

    var locations = [];
    var pushpin;
    for (var i = 0; i < length; i++) {
        pushpin = pins.get(i);
        locations.push(pushpin._location);
    }    

    map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations(locations) });
}

function MyLocationClickHandler() {
    FindMyLocation();
}

function FindMyLocation() {
    var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);    
    geoLocationProvider.removeAccuracyCircle();
    geoLocationProvider.getCurrentPosition({
        updateMapView: false,
        enableHighAccuracy: true,
        showAccuracyCircle: false,
        successCallback: function (args) {
            myLatitude = args.center.latitude;
            myLongitude = args.center.longitude;
            var accuracy = args.position.coords.accuracy;
            accuracy = accuracy < 1000 ? accuracy : 500;
            geoLocationProvider.addAccuracyCircle(args.center, accuracy, 5000, { polygonOptions: { fillColor: new Microsoft.Maps.Color(100, 100, 0, 100), strokeColor: new Microsoft.Maps.Color(100, 100, 0, 100) } });
        },
        errorCallback: function (object) {
            DisplayStatusChange('Error callback invoked, error code ' + object.errorCode);
        }
    });    
}