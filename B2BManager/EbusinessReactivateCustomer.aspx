<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessReactivateCustomer.aspx.vb" Inherits="EbusinessReactivateCustomer" %>

<%@ Register Src="~/UserControls/CustomerDetails.ascx" TagPrefix="uc1" TagName="CustomerDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <script type="text/javascript">
         function CloseWindow(Action) {
             if (Action == 'SubmitCreate') {
                 $.magnificPopup.open({
                     items: {
                         src: '<div class="white-popup"style="color:#3d9970"></br><p style="text-align:justify"><i class="far fa-check-circle" style="font-size:18pt;vertical-align: middle;"></i>Customer has been reactivated with sucess</p></div>', 
                         type: 'inline'
                     },
                     callbacks: {
                         close: function () {
                             if (window.parent != null) {
                                 if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                                     window.parent.ShowAndRefreshGridUpdate(Action);
                                 }
                             }

                             if (window.opener != null) {
                                 if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                                     window.opener.ShowAndRefreshGridUpdate(Action);
                                 }
                                 window.close();
                             }
                         }
                     }
                 });
             }
             else {
                 if (window.parent != null) {
                     if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                         window.parent.ShowAndRefreshGridUpdate(Action);
                     }
                 }

                 if (window.opener != null) {
                     if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                         window.opener.ShowAndRefreshGridUpdate(Action);
                     }
                     window.close();
                 }
             }
         }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:CustomerDetails runat="server" ID="CustomerDetails" />
</asp:Content>

