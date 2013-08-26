<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PutRequestHandler.aspx.cs" Inherits="VinnitsaJunkFood.RequestHandler" %>
<%
    string requestAction;
    string opStatus = "";
    int outletID;        
    object locker = new object();    
    
    lock (locker){
        requestAction = Request["RequestAction"];
        if (requestAction == null){ return; };    
        
        switch (requestAction){
            case "AddAssortmentEntity":
                if (Request.QueryString.Count != 2) { Response.Write("Wrong url param key number"); return; }        
                string mealJson = Request.QueryString[1];
                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.SubmitAssortmentEntity(mealJson);
                Response.Write(opStatus);                
                break;

            case "UpdatePriceList":
                if (Request.QueryString.Count != 2) { Response.Write("Wrong url param key number"); return; }                
                string priceListParams;                
                if (Request.Form.Count == 1){
                    priceListParams = Request.Form[0];
                }
                else{
                    Response.Write("Failed: Price list is empty");
                    return;
                }

                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.SubmitPriceList(Request["OutletID"], priceListParams);
                Response.Write(opStatus);                
                break;

            case "AddNewOutlet":
                if (Request.QueryString.Count != 1) { Response.Write("Wrong url param key number"); return; }                        
                string JsonParamString; 
                
                if (Request.Form.Count == 1){
                    JsonParamString = Request.Form[0];
                }
                else {
                    Response.Write("Failed: Could not detect outlet data in the request");
                    return; }
                
                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.SubitNewOutlet(JsonParamString);
                Response.Write(opStatus);
                break;

            case "AddNewComment":
                if (Request.QueryString.Count != 3) { Response.Write("Wrong url param key number"); return;}

                if (!int.TryParse(Request["OutletID"], out outletID)){
                        Response.Write("Failed: Could not convert " + Request["OutletID"] + " into an outletID");
                        return;
                }

                string checkUsername = Request["UserName"].ToUpper();

                if (Session["username"] != null && (string)Session["username"] != checkUsername){
                    Response.Write("Failed: You cannot comment under other nick other than: " + (string)Session["username"]);
                    return;
                }

                if (Session["username"] == null){
                    Session.Add("username", checkUsername);
                }

                string commentText;

                if (Request.Form.Count == 1){
                    commentText = Request.Form[0];
                }
                else {
                    Response.Write("Failed: Could not detect comment data in the request");
                    return; }

                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.SubmitComment(outletID, Request["UserName"], commentText);
                Response.Write(opStatus);               

                break; 
            case "RateOutlet" :
                if (Request.QueryString.Count != 3) { Response.Write("Wrong url param key number"); return; }
                string outletIdString = Request["OutletID"];
                
                /*if (!int.TryParse(outletIdString, out outletID)) {
                    Response.Write("Failed: Could not convert " + Request["OutletID"] + " into an outletID");
                        return;
                }                
                int newRating;

                if (!int.TryParse(Request["Rating"], out newRating)){
                    Response.Write("Failed: Could not convert " + Request.QueryString[2] + " into a new rating");
                    return;
                }*/

                if (Session[outletIdString] == "Voted"){
                    Response.Write("Failed: You already voted for this outlet");
                    return;
                }

                Session.Add(outletIdString, "Voted");

                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.RateOutlet(outletIdString, Request["Rating"]);
                Response.Write(opStatus);
                 
                break;   
            case "VoteComment":
                if (Request.QueryString.Count != 4) { Response.Write("Wrong url param key number"); return; }
                string commentIdString = Request["CommentId"];
                string thumbs = Request["Thumbs"];                
                int commentId;

                if (!int.TryParse(Request["OutletID"], out outletID)){
                    Response.Write("Failed: Could not convert " + Request["OutletID"] + " into an outletID");
                        return;
                }

                if (!int.TryParse(commentIdString, out commentId)){
                    Response.Write("Failed: Could not convert " + commentIdString + " into commentId");
                    return;
                }

                if (Session["Comment:"+commentIdString] == "CommentVoted"){
                    Response.Write("Failed: You already voted for this comment");
                    return;
                }

                Session.Add("Comment:"+commentIdString, "CommentVoted");                

                opStatus = JunkBackEnd.BusinessLayer.SiteManager.Instance.VoteComment(outletID, commentId, thumbs);
                Response.Write(opStatus);
                break;
            default:
                break;
        }
    }
%>
