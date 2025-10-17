<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessNewCustomer.aspx.vb" Inherits="EbusinessNewCustomer" %>

<%@ Register Src="~/UserControls/CustomerDetails.ascx" TagPrefix="uc1" TagName="CustomerDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
     <script type="text/javascript">
         function CloseWindow(Action) {
             if (Action == 'SubmitCreate') {
                 $.magnificPopup.open({
                     items: {
                         src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer has been created with success, use the button to create a new one or just press on x to close this window and refresh the customer list.</p>'
                          + '</br> <button type="button" class="btn blue btn-submit" onclick="CreateNewCustomer()">Create another customer</button></div> ', 
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
                                 if (typeof window.opener.ShowAndRefreshGridUpdate == "function") {
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
                     if (typeof window.opener.ShowAndRefreshGridUpdate == "function") {
                         window.opener.ShowAndRefreshGridUpdate(Action);
                     }
                     window.close();
                 }
             }
         }
         
         function ReloadCompanyDeleted(CustomerCode,CustomerID) {
             var str = "The customer code <b>"+CustomerCode+"</b> is taken by a deleted customer.</br> Would you like to reload its card?";
             confirmDialog(str, CustomerID, $('#ContentPlaceHolder1_CustomerDetails_HD_EnvID').val());             
         }
         function CreateNewCustomer() {
             if (window.opener != null) {
                 window.opener.NewCustomer();
             }
             else {
                 window.parent.NewCustomer();
             }
         }
         function confirmDialog(message, CustomerID,EnvID) {
             var dialog = '<div class="white-popup">';
             dialog += '<div class="content text-justify"><p>' + message + '</p></div>';
             dialog += '<div class="actions">';
             dialog += '<button type="button" class="btn btn-cancel red"><i class="fas fa-ban"></i>No</button> ';
             dialog += '<button type="button" class="btn btn-primary btn-submit">Yes</button>';
             dialog += '</div>';
             dialog += '</div>';

             $.magnificPopup.open({
                 modal: true,
                 items: {
                     src: dialog,
                     type: 'inline'
                 },
                 callbacks: {
                     open: function () {
                         var $content = $(this.content);

                         $content.on('click', '.btn-submit', function () {
                             $.magnificPopup.close();
                             $(document).off('keydown', keydownHandler);
                             if (window.opener != null) {
                                 window.location = 'EbusinessReactivateCustomer.aspx?cid=' + CustomerID + '&envid=' + EnvID
                             }
                             else {
                                 window.parent.ReloadDeletedCompany(CustomerID, EnvID, this);
                             }
                         });

                         $content.on('click', '.btn-cancel', function () {
                             $.magnificPopup.close();
                             $(document).off('keydown', keydownHandler);
                              window.parent.ShowAndRefreshGridUpdate('Cancel');
                         });

                         var keydownHandler = function (e) {
                             if (e.keyCode == 13) {
                                 $content.find('.btn-submit').click();
                                 return false;
                             } else if (e.keyCode == 27) {
                                 $content.find('.btn-cancel').click();
                                 return false;
                             }
                         };
                         $(document).on('keydown', keydownHandler);
                     }
                 }
             });
         };

     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table style="width:100%;text-align:center;margin-top: 5px;padding: 10px;background-color: #badbff;">
        <tr>
            <td style="padding-top: 10px;padding-bottom: 10px;">
                <i class="fas fa-exclamation-triangle" style="color: #0094ff; margin-top: 2px;"></i> <span>You are about to create a new account in B2B, do not forget to also request for its activation in SAP via master data team</span>
            </td>
        </tr>
    </table>    
        <div style="padding:5px;margin:5px">            
            <uc1:CustomerDetails runat="server" ID="CustomerDetails" />
        </div>
</asp:Content>

