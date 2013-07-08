var priceListReadOnly = true;

function ModifyPriceListPrep() {
    //needed to eliminate a case when no outlets selected
    //but the button is still active     
    if (!IsOutletSelected()) {
        DisplayStatusChange(dictionary.selectOutlet[langId]);
        return false;
    }

    //make Edit visible;
    ShowSearch();
    DisplayStatusChange(dictionary.canAddAssortment[langId]);

    priceListReadOnly = false;

    //make price cells editable                  
    $("#PriceTable td:odd").each(function () {
        $(this).html('<input type ="text" class="PriceCell" maxlength="5" value="' + $(this).text() + '">');
    });

    //Show price list modification block
    $("#PriceListModificationButtons").show();

    //disable the modify and remove meal buttons
    ButtonEnabler("ModifyPricelistButton", false);
    ButtonEnabler("RemoveMealFromPrice", false);
    ButtonEnabler("AddMealFromAssortment", false);

    return true;
}

function GenerateParamStringFromPriceList(priceList) {
    var paramString = "";
    var separator = "÷";
    for (index in priceList) {
        paramString += priceList[index].EntityName + separator;
        paramString += priceList[index].Price.toLocaleString() + "|";
    }
    paramString = paramString.slice(0, -1);
    return paramString;
}

function PostValidationTableProcessor(isInvalid) {
    if (isInvalid) {
        DisplayStatusChange(dictionary.priceFormat[langId] + "7 12.8 11.5 15.75 18 22,50");
    }
    else {
        //is validation succeeded        
        $("#PriceListModificationButtons").hide();
        ButtonEnabler("ModifyPricelistButton", true);

        //reverse transormation to table cells
        $('#PriceTable td:odd').each(function () {
            $(this).html($(this.children).val());
        });
    }
}

function PricelistPreValidation() {
    var price = null;
    //validate entered prices
    // successful regex! : /^\d+((\.|\,)\d{2})?$/i
    var regEx = /^\d+((\.|\,)(\d{2}|\d{1}))?$/i;
    var isValid = true;    
    $("#PriceTable td:odd").each(function () {        
        //if the entered value is invalid mark as red;
        price = $(this.children).val().toString();
        if (!regEx.test(price)) {
            isValid = false;
            $(this.children).css("border", "2px solid red");
        }
        else {
            $(this.children).css("border", "none");            
        }        
    });
    return isValid;
}

function UpdatePriceListFromTable(priceList) {
    var price = null;
    var index = 0;
    $("#PriceTable td:odd").each(function () {
        price = $(this.children).val().toString();
        priceList[index].Price = price;
        index++;
    });
    return priceList;
}

function IsOutletSelected() {
    return ($("#OutletsBox :selected").length > 0);
}