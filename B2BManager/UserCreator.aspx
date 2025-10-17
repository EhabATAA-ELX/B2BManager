<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UserCreator.aspx.vb" Inherits="UserCreator" %>
<%@ Register Src="~/UserControls/UserInformation.ascx" TagPrefix="uc1" TagName="UserInformation" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="CSS/UsersManagement.css" rel="stylesheet" />
    <script type="text/javascript">
        function FinsihUserSubmit() {
            window.parent.EndUserCreation();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:UserInformation runat="server" ID="UserInformation" />    
</asp:Content>

