function PrepareSendData() {

    if (newPushpin == null) {
        DisplayStatusChange(dictionary.selectOutlet[langId]);
        return;
    }

    var latitude = newOutletLatitude;
    var longitude = newOutletLongitude;
    var outletName = $("#NewOutletName").val();
    var outletRating = $("#NewOutletRating").val() != "" ? parseInt($("#NewOutletRating").val()) : 0;
    var outletDescription = $("#NewOutletDescription").val();
    var imageUrl = $("#NewOutletURL").val();
    var voteCount = outletRating != 0 ? 1 : 0;

    if (outletName.length < 3) {
        DisplayStatusChange(dictionary.minOutletName[langId]);
        return;
    }

    if (outletDescription.length > 2000) {
        DisplayStatusChange(dictionary.outletDescriptionMaxLen[langId]);
        return;
    }

    if (outletRating == 0 && $("#NewOutletRating").val().replace(/\s+/g, ' ').length > 1) {
        DisplayStatusChange(dictionary.ratingConstraint[langId]);
        return;
    }

    var sendData = {
        Latitude: latitude,
        Longitude: longitude,
        EntityName: outletName,
        OutletRating: outletRating,
        Description: outletDescription,
        ImageUrl: imageUrl,
        Votes: voteCount
    }

    return sendData;
}


function RenderUiForOutletClick(ImageUrl, OutletRating) {
    var display = ImageUrl == null || ImageUrl.length == 0 ?
                            'none' :
                            'block';
    $("#OutletImage").css("display", display);

    $('#SelectedOutletRating').RatingVote('on');
    $('#SelectedOutletRating').RatingValue(OutletRating);

    //Enable menu edit button, hide price edit buttons
    ButtonEnabler("ModifyPricelistButton", true);
    ButtonEnabler("AddCommentButton", true);
    $("#PriceListModificationButtons").hide();
}

function FilterArrayByEntityName (name, array) {
    var filteredArray = [];
    name = name.toUpperCase()
    var i = 0;
    for (var i = 0; i < array.length; i++) {
        if (array[i].EntityName.toUpperCase().indexOf(name) >= 0) { filteredArray.push(array[i]) }
    }

    return filteredArray;
}

function OutletHasFilteredMeal (outlet, assortment) {
    if (outlet.AssortmentPriceList.length == 0) { return false; }

    var i, j = 0;
    var meal;
    for (var i = 0; i < outlet.AssortmentPriceList.length; i++) {
        meal = outlet.AssortmentPriceList[i];
        for (var j = 0; j < assortment.length; j++) {
            if (assortment[j].EntityID == meal.AssortmentId) {
                return true;
            }
        }
    }

    return false;
}