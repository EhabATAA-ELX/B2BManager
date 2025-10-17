<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <img src="Images/lake.jpg" width="250" />
    <asp:Button runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" Text="Submit" />
</asp:Content>

