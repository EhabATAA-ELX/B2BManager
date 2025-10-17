<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessChangePreferences.aspx.vb" Inherits="EbusinessChangePreferences" %>

<%@ Register Src="~/UserControls/EbusinessPreferences.ascx" TagPrefix="uc1" TagName="EbusinessPreferences" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:EbusinessPreferences runat="server" ID="EbusinessPreferences" ShowCancelButton="true" />
</asp:Content>

