<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ApplicationCacheManager.aspx.vb" Inherits="ApplicationCacheManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div style="width: 100%; padding-left: 50px; padding-top: 20px; text-align: left">
                <h1>List of objects in cache:</h1>
                <asp:Table runat="server" ID="cacheItemsTable" CellPadding="5" CellSpacing="5">
                </asp:Table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

