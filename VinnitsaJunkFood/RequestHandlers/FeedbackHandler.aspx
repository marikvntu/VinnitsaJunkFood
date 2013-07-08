<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FeedbackHandler.aspx.cs" Inherits="VinnitsaJunkFood.RequestHandlers.FeedbackHandler" %>

<%
    object locker = new object();
    lock (locker) {
        string requestAction = Request["RequestAction"];        
        if (requestAction != "SubmitFeedback") { return; };
        string subject = Request["Subject"];
        string contact = Request["ContactInfo"];
        string message = Request.Params["msg"];

        message = message.Replace("\n", "<br>");
        string sendStatus = SendMailMessage("junkfood.notification@mail.ru", "junkfood.vn@gmail.com", "", "", "Junkfood feedback: " + subject, message + "<br><br>contact info: " + contact);
        Response.Write(sendStatus);
    }
    
    
     
%>
