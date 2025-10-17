<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsPreferences.aspx.vb" Inherits="EbusinessInsightsPreferences" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/InsightsPreferences.ascx" TagPrefix="uc1" TagName="InsightsPreferences" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">    
    <uc1:InsightsPreferences runat="server" id="InsightsPreferences" ShowCancelButton="true" />
</asp:Content>

