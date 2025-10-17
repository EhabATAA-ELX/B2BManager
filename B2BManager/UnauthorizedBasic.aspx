<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UnauthorizedBasic.aspx.vb" Inherits="UnauthorizedBasic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="mainbox-404">
        <div class="access-denied">Acess denied</div>
        <div class="msg-404">
            You are not authorized to access this area/page<p>Please contact your administrator to check your access.</p>
        </div>
    </div>
</asp:Content>

