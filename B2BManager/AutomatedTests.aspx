<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="AutomatedTests.aspx.vb" Inherits="AutomatedTests" %>
<%@ Register Src="~/UserControls/ToolsRepeater.ascx" TagPrefix="uc1" TagName="ToolsRepeater" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <br /><uc1:ToolsRepeater runat="server" ID="ToolsRepeater" />
</asp:Content>