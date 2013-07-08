var LeftPaneShown = false;
var RightPaneShown = false;
var previousRow = null;
var commentsActive = false;

function ControlRightPaneVisibility(show) {
    if (RightPaneShown && show) { return; }
    RightPaneShown = show;

    var mapWidth = 100;
    mapWidth -= LeftPaneShown ? 30 : 0;
    mapWidth -= show ? 20 : 0;    
    ShowAndResizePanels(show, "OutletRightPane", mapWidth);
}

function ControlLeftPaneVisibility(show) {
    if (LeftPaneShown && show) { return; }
    LeftPaneShown = show;

    var mapWidth = 100;
    mapWidth -= RightPaneShown ? 20 : 0;
    mapWidth -= show ? 30 : 0;    
    ShowAndResizePanels(show, "main", mapWidth);
}

function ShowAndResizePanels(show, panelName, mapWidth) {
    var display = show ? "block" : "none"

    $("#bingMap").css("width", mapWidth + "%");
    $("#" + panelName).css("display", display);
}

function ButtonEnabler(buttonId, enabled) {
    if (enabled) {
        $('#' + buttonId).removeAttr("disabled");
    }
    else {
        $('#' + buttonId).attr("disabled", "disabled");
    }
}

function PriceListRowClicked(row) {
    $(previousRow).css("background-color", "inherit");
    $(row).css("background-color", "grey");
    previousRow = row;
    ButtonEnabler("RemoveMealFromPrice", true);
}

function FindIndexOfElementInArray(array, elementName) {    
    for (index in array) {
        if (array[index].EntityName.replace(/\s+/g, '') == elementName.replace(/\s+/g, '')) {
            return index;
        }
    }
    //return -1 if nothing found
    return -1;
}

function AddOutletClick() {
    ToggleMapClick();
    var buttonName = "";
    if (newOutletSelection) { $("#NewOutletBlock").show(); }
        else { $("#NewOutletBlock").hide();}
    var newHtml = newOutletSelection ?
        "<i class=\x22icon-cancel\x22></i>" + dictionary.cancel[langId] :
        "<i class=\x22icon-new\x22></i>" + dictionary.addOutletNormal[langId]

    $("#NewOutletButton").html(newHtml);
}

function SearchButtonClickHandler() {
    ControlLeftPaneVisibility(true);
    $("#SearchLink").click();
}

function EditButtonClickHandler() {
    ControlLeftPaneVisibility(true);        
    $("#EditLink").click();
}

//Move to Ui Transformations
function DisplayStatusChange(response) {
    $("#StatusBar").text(response);
    $("#StatusBar").fadeIn(2000, HideSubmitStatus);
}

function HideSubmitStatus () {
    $("#StatusBar").fadeOut(8000, null);
}

function OnAccordionHeaderClick(obj) {    
    var parentLi = $(obj).parent("li");
    var target = $(obj).parent('li').children("div");

    if (parentLi.hasClass("active")) {
        target.slideUp(undefined, function () {
            parentLi.removeClass("active");
        });
    }
    else {
        target.slideDown();
        parentLi.addClass("active");
    }
}

function ShowSearch() {
    var searchPanel = $("#SearchAccordeonContainer");
    if (!searchPanel.hasClass("active")) {
        OnAccordionHeaderClick(searchPanel.children("a"));
    }
}

function FeedbackClick() {
    var subject = dictionary.subject[langId] + ": <input id=\x22feedbackSubject\x22 type=\x22text\x22 class=\x22feedbackText\x22 title=\x22" + dictionary.subjectTitle[langId] + "\x22 />";
    var contactInfo = "<br><br>" + dictionary.contactInfo[langId] + ": <input type=\x22text\x22 id=\x22feedbackContact\x22 class=\x22feedbackText\x22/ title=\x22" + dictionary.contactInfoTitle[langId] + "\x22 >";
    var message = "<br><br>" + dictionary.feedbackMessage[langId] + "*: <textarea class=\x22feedbackText\x22 id=\x22feedbackMessage\x22 title=\x22" + dictionary.feedbackMessageTitle[langId] + "\x22></textarea><br>";
    var required = "<br> * - " + dictionary.required[langId];
    var dialogHtml = "<div class=\x22feedbackDiv\x22 >" + subject + contactInfo + message + "</div>" + required;
    
    $.Dialog({
        'title': dictionary.feebackTitle[langId],
        'content': dialogHtml,
        'draggable': false,
        'overlay': true,       
        'buttonsAlign': 'center',               
        'buttons': {
            Ok :{
                'action': function () {
                    var feedbackContainer = $(".feedbackDiv");
                    var feedbackSubject = $(feedbackContainer).find("#feedbackSubject").val();
                    var feedbackContact = $(feedbackContainer).find("#feedbackContact").val();
                    var feedbackMessage = $(feedbackContainer).find("#feedbackMessage").val();
                    SendFeedback(feedbackSubject, feedbackContact, feedbackMessage);
                }
            },
            Cancel: {
                'action': function () { }
            }
        }
    });
}

function SendFeedback(subject, contactInfo, message) {
    if (message.length == 0) {
        DisplayStatusChange(dictionary.emptyFeedbackMessage[langId]);
        return;
    }

    $.ajax({
        url: '../RequestHandlers/FeedbackHandler.aspx?RequestAction=SubmitFeedback&Subject=' + subject + '&ContactInfo=' + contactInfo,
        method: "POST",
        data: {msg :message},
        dataType : "text",
        success: FeedbackSubmitted
    });
}

function FeedbackSubmitted(data) {    
    var status = data == "success" ? dictionary.feedbackSuccess[langId] : dictionary.feedbackFailed[langId];
    DisplayStatusChange(status + " junkfood.vn@gmail.com");
}

function OnShowMinusedCommentClick(link) {
    $(link).hide();
    $(link).parent().prev().show();    
}