<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ChatBotManager.aspx.vb" Inherits="Chatbot_ChatBotManager" %>

<%@ Register Src="~/UserControls/ToolsRepeater.ascx" TagPrefix="uc1" TagName="ToolsRepeater" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <br /><uc1:ToolsRepeater runat="server" ID="ToolsRepeater" />
</asp:Content>

