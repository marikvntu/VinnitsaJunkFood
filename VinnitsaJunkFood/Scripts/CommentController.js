function CommentsController($scope, $http) {
    $scope.comments = [];
    var commentsActive = false;    
    $scope.urlHeader = "../JunkService.svc/";   
    $scope.currentOutletId = 0;
    $scope.voteInfo = null;

    $scope.$on('getCommentsEvent', function (event, outletId) {
        $scope.currentOutletId = outletId;
        var url = $scope.urlHeader + 'GetComments?Id=' + outletId;        
        AjaxGet(url, $scope.handleCommentsLoaded);
    });


    $scope.handleCommentsLoaded = function (data, status) {                
        //force redraw
        $scope.$apply(function () {
            $scope.comments = data;
        });
        
        UpdateCommentCount(data.length);
        $scope.refreshThumbsUI();
    }

    function UpdateCommentCount (count) {
        var commentTabLabel = $("#CommentTabLabel");
        var commentTabText = commentTabLabel.text();
        commentTabText = commentTabText.substring(0, commentTabText.indexOf("(") + 1) + count + ")";
        commentTabLabel.text(commentTabText);
    }
    
    $scope.VoteCommentHandler = function (commentID, thumbsUp ) {
        var thumbs = thumbsUp ? "up" : "down";

        $scope.voteInfo = {
                            commentId: commentID,
                            vote: thumbs
        };

        var url = $scope.urlHeader +
            "VoteComment?CommentId=" +
            commentID + "&Thumbs=" + thumbs
            + "&OutletId=" + $scope.currentOutletId;
        
        AjaxPost(url, null, $scope.handleCommentVoted);
    }

    $scope.handleCommentVoted = function (data, status) {
        if (isNaN(data)) {
            DisplayStatusChange(data);
            return;
        }

        jQuery.each($scope.comments, function (index, item) {
            if (item.EntityID == $scope.voteInfo.commentId) {
                item.CommentRating = data;
            }
        });

        //force redraw
        $scope.$apply();

        $scope.refreshThumbsUI();
    }

    $scope.AddNewCommentHandler = function () {
        commentsActive = !commentsActive;
        var commentButtonHtml = "";

        if (commentsActive) {
            $("#NewCommentBlock").show();
            $("#SubmitCommentButton").show();
            commentButtonHtml = "<i class=\x22icon-cancel\x22></i>" + dictionary.cancel[langId];
        }
        else {
            $("#SubmitCommentButton").hide();
            $("#NewCommentBlock").hide();
            commentButtonHtml = "<i class=\x22icon-comments\x22></i>" + dictionary.addComment[langId];
            $("#NewCommentText").val("");
        }
        $("#AddCommentButton").html(commentButtonHtml);
    }
    
    $scope.SubmitNewCommentHandler = function () {
        var commentText = $("#NewCommentText").val();
        var username = $("#CommentUserName").val();

        if (username.length < 3 || commentText.length < 3) {
            DisplayStatusChange(dictionary.commentLength[langId]);
            return;
        }

        var requestObject = {            
            EntityID: $scope.currentOutletId,
            UserName: username,
            CommentText: commentText
        };        
        
        $scope.url = $scope.urlHeader+ "AddNewComment";
                    
        $scope.AddNewCommentHandler();

        AjaxPost($scope.url, requestObject, $scope.handleCommentsLoaded);
    }

    $scope.refreshThumbsUI = function(){
        if ($scope.voteInfo == null) {return;}

        var classSelector = ".icon-thumbs-" + $scope.voteInfo.vote;
        $("#CommentRating" + $scope.voteInfo.commentId + " > " + classSelector).css("color", "black");
    }    
}
