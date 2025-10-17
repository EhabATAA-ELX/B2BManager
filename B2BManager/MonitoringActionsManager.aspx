<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="MonitoringActionsManager.aspx.vb" Inherits="MonitoringActionsManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/MonitoringActionsGrid.ascx" TagPrefix="uc1" TagName="MonitoringActionsGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:MonitoringActionsGrid runat="server" id="MonitoringActionsGrid" />
</asp:Content>


