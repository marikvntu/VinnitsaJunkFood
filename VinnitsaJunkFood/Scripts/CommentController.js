function CommentsController($scope, $http) {
    $scope.comments = [];
    var commentsActive = false;
    $scope.getUrlHeader = '../RequestHandlers/GetRequestHandler.aspx?RequestAction=';
    $scope.putUrlHeader = '../RequestHandlers/PutRequestHandler.aspx?RequestAction=';
    $scope.currentOutletId = 0;
    $scope.voteInfo = null;

    $scope.$on('getCommentsEvent', function (event, outletId) {
        $scope.currentOutletId = outletId;
        var url = $scope.getUrlHeader + 'GetComments&Id=' + outletId;
        $http.get(url).success($scope.handleCommentsLoaded);
    });


    $scope.handleCommentsLoaded = function (data, status) {
        if (typeof data == "string") { DisplayStatusChange(data); return; }
        $scope.comments = data;

        $("#windows8").css("display", "none");
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
        $scope.voteInfo =  {
                            commentId: commentID,
                            vote: thumbs
                            };

        var url = $scope.putUrlHeader +
            "VoteComment&CommentId=" +
            commentID + "&Thumbs=" + thumbs
            + "&OutletId=" + $scope.currentOutletId;

        $http.post(url)
             .success($scope.handleCommentVoted);
    }

    $scope.handleCommentVoted = function (data, status) {
        if (isNaN(data)) {
            DisplayStatusChange(data);
            return;
        }

        $.grep($scope.comments, function (item) {
            if (item.EntityID == $scope.voteInfo.commentId) {
                item.CommentRating = data;
            }
        });
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

        $scope.url = $scope.putUrlHeader
                    + "AddNewComment"
                    + "&OutletId=" + $scope.currentOutlet
                    + "&UserName=" + username;

        $scope.AddNewCommentHandler();
        $("#windows8").css("display", "block");

        $http({
            url: $scope.url,
            method: "POST",
            data: commentText,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).success($scope.handleCommentsLoaded);
    }

    $scope.refreshThumbsUI = function(){
        if ($scope.voteInfo == null) {return;}

        var classSelector = ".icon-thumbs-" + $scope.voteInfo.vote;
        $("#CommentRating" + $scope.voteInfo.commentId + " > " + classSelector).css("color", "black");
    }    
}
