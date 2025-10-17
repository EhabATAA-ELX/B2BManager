<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessContactProfile.aspx.vb" Inherits="EbusinessContactProfile" %>

<%@ Register Src="~/UserControls/ContactDetails.ascx" TagPrefix="uc1" TagName="ContactDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ContactDetails runat="server" ID="ContactDetails" />
</asp:Content>

