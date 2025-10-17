<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UserGroupCreator.aspx.vb" Inherits="UserGroupCreator" %>

<%@ Register Src="~/UserControls/GroupInformation.ascx" TagPrefix="uc1" TagName="GroupInformation" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/UsersManagement.css" rel="stylesheet" />    
    <script type="text/javascript">
        function FinsihGroupSubmit() {
            window.parent.EndGroupCreation();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:GroupInformation runat="server" ID="GroupInformation" />
</asp:Content>

