<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetRequestHandler.aspx.cs" Inherits="VinnitsaJunkFood.RequestHandler" %>
<%
    string requestAction;
    string opStatus = "";
    object locker = new object();

    lock (locker){

        if ((Request.QueryString.HasKeys()) && (Request.QueryString.Keys[0] == "RequestAction"))
        {
            requestAction = Request.QueryString.Get(0);
        }
        else { return; };

        switch (requestAction)
        {
            case "Initialize":
                Response.Write(JunkBackEnd.BusinessLayer.SiteManager.Instance.ReturnInitialData());
                break;
            case "GetComments":
                int id;
                if (Request.QueryString.Count == 2 &&
                    int.TryParse(Request.QueryString.Get(1), out id)){
                    Response.Write(JunkBackEnd.BusinessLayer.SiteManager.Instance.GetCommentsForOutlet(id));
                }
                break;
            default:
                break;
        }
    }
%>

