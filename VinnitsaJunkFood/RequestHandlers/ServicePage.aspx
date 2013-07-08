<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServicePage.aspx.cs" Inherits="VinnitsaJunkFood.ServicePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page for maintanence and diagnostics</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            
    </div>
        <asp:Button ID="WriteToLog" runat="server" OnClick="WriteToLog_Click" Text="Wite to log file" />
        <asp:TextBox ID="LogText" runat="server"></asp:TextBox>
        <p>
            &nbsp;</p>
        <asp:Label ID="ErrorLabel" runat="server" Text="Error appears here"></asp:Label>
    </form>
</body>
</html>
