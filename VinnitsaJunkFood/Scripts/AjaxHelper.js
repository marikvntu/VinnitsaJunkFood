$(document)
    .ajaxStart(function () {
        $("#windows8").show();
    })
    .ajaxStop(function () {
        $("#windows8").hide();
    })
    .ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
        DisplayStatusChange(dictionary.errorCouldNotProcess[langId] + ajaxSettings.url + dictionary.withStatus[langId] + thrownError);
    });

function AjaxGet(url, successCallback) {
    $.get(url)
       .done(function (response) {
           if (!response.Success) {
               DisplayStatusChange(response.ErrorMessage);
               return;
           }

           var data = typeof (response.Data) == "string" ? $.parseJSON(response.Data) : response.Data;
           successCallback(data);
       });
}

function AjaxPost(url, requestObject, successCallback) {
    var dataString = JSON.stringify({ requestData: requestObject });

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: url,
        data: dataString,
        dataType: "json",
        success: function (response) {
            if (!response.Success) {
                DisplayStatusChange(response.ErrorMessage);
                return;
            }
            successCallback(response.Data);
        }        
    });
}



