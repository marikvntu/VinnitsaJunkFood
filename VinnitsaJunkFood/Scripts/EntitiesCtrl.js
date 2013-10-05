function EntitiesCtrl($scope, $http, $timeout) {
    $scope.getUrlHeader = '../RequestHandlers/GetRequestHandler.aspx?RequestAction=';
    $scope.putUrlHeader = '../RequestHandlers/PutRequestHandler.aspx?RequestAction=';
    $scope.urlHeader = "../JunkService.svc/";
    $scope.url;// = $scope.getUrlHeader + 'Initialize&timestamp=' + Date.now();
    $scope.outlets = [];
    $scope.filteredOutlets = [];
    $scope.assortment = [];
    $scope.priceList = [];
    $scope.backupAssortment = [];
    $scope.currentOutlet = -1;
    $scope.selectedOutletObject = null;

    var outletRatingNode = $('#SelectedOutletRating');    

    $scope.hadleEntityUploaded = function (data) {
        DisplayStatusChange(data);
        $timeout($scope.fetch, 150);
    }

    $scope.addMealButtonHandler = function () {
        var mealName = $('#AssortmtentFilterQueryBox').val();
        //TODO: make possible to write description
        var description = "";
        var meal = {
            EntityID: 0,
            EntityName: mealName,
            MealDescription: description
        };        
        
        var url = $scope.urlHeader + "SubmitMeal";
        AjaxPost(url, meal, $scope.hadleEntityUploaded);
    }

    $scope.onAssortmentListChange = function () {
        $scope.filteredMeals = FilterArrayByEntityName($("#AssortmtentFilterQueryBox").val(), $scope.assortment);

        $scope.FilterOutletsBySearchPhraseAndMeals();
        $scope.addPinsToMap();
        CenterMapByEntitiesRect();

        //display AddMealButton only in case of no existing meals
        if ($('#AssortmtentFilterQueryBox').val().trim().length === 0) {
            ButtonEnabler("AddMealButton", false);
            return;
        }

        ButtonEnabler("AddMealButton", $('#AssortmentBox option').length == 0);
    }

    $scope.onOutletClick = function (id) {
        $scope.currentOutlet = id;
        $scope.assortment = $scope.backupAssortment;
        $scope.filteredMeals = $scope.backupAssortment;
        $scope.selectedOutletObject = null;
        $scope.priceListReadOnly = true;

        //fire event to the comment controller
        $scope.$broadcast('getCommentsEvent', id);

        ControlRightPaneVisibility(true);

        //find entity coordinates and center map on it
        $.each($scope.outlets, function (k, v) {
            if (v.EntityID != id) { return; }

            var location = new Microsoft.Maps.Location(v.Latitude, v.Longitude);
            CenterMap(location);
            PaintSelectedPushpin(v.Latitude, v.Longitude);
            $scope.selectedOutletObject = v;

            //Each outlet has its own price list to be populated
            $scope.priceList = [];

            if (v.AssortmentPriceList == null || v.AssortmentPriceList.length == 0) { return; }
            $.each(v.AssortmentPriceList, $scope.fillPriceList);
        });

        RenderUiForOutletClick($scope.selectedOutletObject.ImageUrl, $scope.selectedOutletObject.OutletRating);
    }

    $scope.fillPriceList = function (k, v) {
        var meal = {};
        $.each($scope.assortment, function (key, val) {
            if (val.EntityID == v.AssortmentId) {
                meal.EntityName = val.EntityName;
                meal.Price = v.Price
                $scope.priceList.push(meal);
                return;
            }
        });
    }

    $scope.pinClicked = function (e) {
        if (e.targetType != 'pushpin') { return; }

        var pinLoc = e.target.getLocation();
        //hide it to prevent errors in re-painting
        HideInfobox(e);
        CenterMap(pinLoc);

        //find coordinates and center map on it
        $.each($scope.outlets, function (k, v) {
            if (v.Latitude == pinLoc.latitude &&
              v.Longitude == pinLoc.longitude) {
                $scope.onOutletClick(v.EntityID);

                //select listbox item that corresponds pushpin
                $("#OutletsBox :contains(" + v.EntityName + ")").attr('selected', true);
            }
        });
    }

    $scope.addPushpin = function (index, value) {
        var pushpin = AddPushpinWoHandler(value.Latitude, value.Longitude, standardPushpinOptions, value);
        //Add click evnt handler on pushpin
        Microsoft.Maps.Events.addHandler(pushpin, 'click', $scope.pinClicked);
    }

    $scope.handleEntitiesLoaded = function (data) {
            //var data = JSON.parse(response.Data);
            $scope.outlets = data.OutletList;
            $scope.assortment = data.AssortmentList;
            $scope.backupAssortment = data.AssortmentList;            

            $scope.clearAllFilters();
            $scope.$apply();

            $scope.addPinsToMap();
    }

    $scope.addPinsToMap = function () {
        ClearMap();
        $.each($scope.filteredOutlets, $scope.addPushpin);
    }

    $scope.outletChanged = function () {
        $scope.FilterOutletsBySearchPhraseAndMeals();

        $scope.addPinsToMap();

        CenterMapByEntitiesRect();

        if (!IsOutletSelected()) {
            ButtonEnabler("ModifyPricelistButton", false);
            $scope.priceList = [];
            return true;
        }
    }

    $scope.modifyPricelistButtonHandler = function () {
        if (!ModifyPriceListPrep()) { return; }

        $scope.substractOutletMenuFromAssortment();
    }

    $scope.substractOutletMenuFromAssortment = function () {
        $scope.backupAssortment = $scope.filteredMeals;
        $scope.filteredMeals = [];
        var meal = null;
        var mealCellsInPrice = $("#PriceTable td:even");
        for (index in $scope.backupAssortment) {
            meal = $scope.backupAssortment[index];
            if (mealCellsInPrice.length == 0 || mealCellsInPrice.text().indexOf(meal.EntityName) == -1) {
                $scope.filteredMeals.push(meal);
            }
        }
    }

    $scope.removeFromPriceClickHandler = function () {
        //add incomplete entity to the assortment
        //ID in this case is redundant
        var meal = [];
        var previousRowPriceCell = $(previousRow).has("td:even");

        //prevent user from deleting old rows
        if (!previousRowPriceCell.children()[1].children[0].hasAttribute("newRow")) {
            DisplayStatusChange(dictionary.deleteExistingRows[langId]);
            return;
        };

        meal.EntityName = previousRowPriceCell.text();
        var index = FindIndexOfElementInArray($scope.priceList, meal.EntityName);
        if (index == -1) { return; }

        $scope.priceList.splice(index, 1);
        $scope.filteredMeals.push(meal);
        ButtonEnabler("RemoveMealFromPrice", false);
    }

    $scope.addMealFromAssortmentHandler = function () {
        var mealName = $("#AssortmentBox :selected").text();
        var index = FindIndexOfElementInArray($scope.filteredMeals, mealName);
        if (index == -1) { return; }

        var priceEntry = {
            EntityName: $scope.filteredMeals[index].EntityName,
            Price: 0
        };
        //remove from assortment list
        $scope.filteredMeals.splice(index, 1);
        //add to price list
        $scope.priceList.push(priceEntry);

        //wait until UI is refreshed after adding new row
        $scope.$watch('newrow', function () {
            $("#PriceTable tr:last").find("td:odd").html('<input type ="text" class="PriceCell" newRow maxlength="5">');
        });
        ButtonEnabler("RemoveMealFromPrice", $scope.priceList.length > 0);
        ButtonEnabler("AddMealFromAssortment", false);
    }

    $scope.SaveClickHandler = function () {
        var isValid = PricelistPreValidation();
        //Send request to the server, get response        
        if (!isValid) {
            PostValidationTableProcessor(!isValid);
            return;
        }
        $scope.priceList = UpdatePriceListFromTable($scope.priceList);

        $scope.SendModifiedPriceData();
    }

    $scope.SendModifiedPriceData = function () {
        $scope.url = $scope.urlHeader+ 'UpdatePriceList';

        var priceString = GenerateParamStringFromPriceList($scope.priceList);

        var baseEntity = {
            EntityID : $scope.currentOutlet,
            EntityName : priceString
        }        
                
        AjaxPost($scope.url, baseEntity, $scope.handlePriceListSent);
    }

    $scope.handlePriceListSent = function (response) {
        PostValidationTableProcessor(false);
        priceListReadOnly = true;
        DisplayStatusChange(response);
        $timeout($scope.fetch, 150);
    }

    $scope.SaveNewOutletHandler = function () {
        var sendData = PrepareSendData();

        $scope.url = $scope.urlHeader + "AddNewOutlet";
        
        AjaxPost($scope.url, sendData, $scope.hadleOutletUploaded);
    }

    $scope.hadleOutletUploaded = function (data) {
        AddOutletClick();
        $scope.hadleEntityUploaded(data);        
    }

    $scope.FilterOutletsBySearchPhraseAndMeals = function (mealObject) {
        var outletKey = $("#OutletSearchBox").val().toUpperCase();
        var filteredAssortment = [];
        var mealKey = "";
        if (typeof mealObject == "undefined") {
            mealKey = $("#AssortmtentFilterQueryBox").val().toUpperCase();
            filteredAssortment = $scope.filteredMeals;
        }
        else {
            filteredAssortment.push(mealObject);
            mealKey = mealObject.EntityName;
        }

        var blankMealResultSet = $scope.filteredMeals.length == 0;
        //var ignoreMeals = !blankMealResultSet && $scope.filteredMeals.length == $scope.assortment.length;

        if (outletKey.length == 0 && mealKey.length == 0) {
            $scope.filteredOutlets = $scope.outlets;
            return;
        }

        if (blankMealResultSet) {
            $scope.filteredOutlets = [];
            return;
        }

        $scope.filteredOutlets = [];

        $.each($scope.outlets, function (key, Outletvalue) {
            if (Outletvalue.EntityName.toUpperCase().indexOf(outletKey) >= 0) {
                if (OutletHasFilteredMeal(Outletvalue, filteredAssortment)) {
                    $scope.filteredOutlets.push(Outletvalue)
                }
            }
        });
    }

    $scope.ratingClick = function () {
        if ($scope.selectedOutletObject == null) { return; }

        var newRating = outletRatingNode.RatingValue();

        outletRatingNode.RatingValue($scope.selectedOutletObject.OutletRating); //skip selection in case of failure

        $scope.url = $scope.urlHeader
                     + "RateOutlet"
                     + "?OutletId=" + $scope.currentOutlet
                     + "&Rating=" + newRating;

        AjaxPost($scope.url, null, $scope.handleRatingLoaded);        
    }

    $scope.handleRatingLoaded = function (data, status) {       
        $scope.selectedOutletObject.OutletRating = data.OutletRating;
        $scope.selectedOutletObject.Votes = data.Votes;
        $scope.$apply();

        $scope.updateRating();
    }

    $scope.updateRating = function () {
        outletRatingNode.RatingVote('off');
        outletRatingNode.RatingValue($scope.selectedOutletObject.OutletRating);

        for (var i in $scope.outlets) {
            if ($scope.outlets[i].EntityID = $scope.selectedOutletObject.EntityID) {
                $scope.outlets[i].OutletRating = $scope.selectedOutletObject.OutletRating;
                $scope.outlets[i].Votes = $scope.selectedOutletObject.Votes;
                return;
            }
        }
    }

    $scope.onAssortmentClick = function (mealObject) {
        if (priceListReadOnly) {
            $scope.FilterOutletsBySearchPhraseAndMeals(mealObject);
            $scope.addPinsToMap();
            CenterMapByEntitiesRect();
        }
        ButtonEnabler("AddMealFromAssortment", true);
    }

    $scope.onAssortmentDblClick = function (mealName) {
        if (!priceListReadOnly) { $scope.addMealFromAssortmentHandler(); }
    }

    $scope.clearAllFilters = function () {
        $("#AssortmtentFilterQueryBox").val("");
        $scope.filteredMeals = FilterArrayByEntityName($("#AssortmtentFilterQueryBox").val(), $scope.assortment);

        $("#OutletSearchBox").val("");
        $scope.FilterOutletsBySearchPhraseAndMeals();
    }

    $scope.showAllButtonHandler = function () {
        ControlRightPaneVisibility(false);
        ControlLeftPaneVisibility(false);
        $scope.clearAllFilters();
        $scope.addPinsToMap();
        CenterMapByEntitiesRect();
    }

    $scope.fetch = function () {
        //$scope.url = $scope.getUrlHeader + 'Initialize&timestamp=' + Date.now();
        $scope.url = "../JunkService.svc/Initialize";        
        AjaxGet($scope.url, $scope.handleEntitiesLoaded);
    };

    // Defer fetch for 500 miliseconds to give everything an opportunity layout
    $timeout($scope.fetch, 500);
};