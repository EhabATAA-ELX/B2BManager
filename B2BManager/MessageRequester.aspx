<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="MessageRequester.aspx.vb" ValidateRequest="false" Inherits="MessageRequester" %>

<%@ Register Src="~/UserControls/MessageRequesterControl.ascx" TagPrefix="uc1" TagName="MessageRequesterControl" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript" src="Scripts/CodeMirror/lib/codemirror.js?v=2"></script>
    <link href="CSS/jquery-ui.css" rel="stylesheet" />
    <link href="Scripts/CodeMirror/lib/codemirror.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/CodeMirror/mode/xml/xml.js"></script>
    <link href="CSS/AutomatedTests.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/CodeMirror/placeholder.js"></script>
    <style type="text/css">
        .CodeMirror {
            height:650px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <uc1:MessageRequesterControl runat="server" ID="MessageRequesterControl" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>