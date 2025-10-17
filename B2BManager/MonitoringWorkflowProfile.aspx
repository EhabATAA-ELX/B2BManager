<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="MonitoringWorkflowProfile.aspx.vb" Inherits="MonitoringWorkflowProfile" %>

<%@ Register Src="~/UserControls/MonitoringWorkflow.ascx" TagPrefix="uc1" TagName="MonitoringWorkflow" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:MonitoringWorkflow runat="server" ID="MonitoringWorkflow" ActivateEditMode="false" />
</asp:Content>

