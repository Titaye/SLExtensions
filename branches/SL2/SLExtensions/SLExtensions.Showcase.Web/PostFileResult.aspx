<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostFileResult.aspx.cs" Inherits="SLExtensions.Showcase.Web.PostFileResult" EnableViewState="false" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:FileUpload runat="server" ID="uploadField"> 
            </asp:FileUpload><input type="submit" />
    <div>
    
        Post result
        <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            Post name : <%# Eval("Key")%> Value : <%# Eval("Value")%>

        </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
