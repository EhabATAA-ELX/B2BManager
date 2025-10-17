<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Unauthorized.aspx.vb" Inherits="Unauthorized" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="mainbox-404">
        <div class="access-denied">Acess denied</div>
        <div class="msg-404">
            You are not authorized to access this area/page<p>Please contact your administrator to check your access.</p>
        </div>
    </div>
</asp:Content>

