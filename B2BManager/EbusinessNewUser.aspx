<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessNewUser.aspx.vb" Inherits="EbusinessNewUser" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/UserDetails.ascx" TagPrefix="uc1" TagName="UserDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <script type="text/javascript">   
        function CloseWindow(Action) {
            var cid = '<%= GetCompanyGlobalID() %>';
            var entityName = 'User';
            if (cid == "undefined") {
                entityName = "Super User"
            }

            if (Action == 'Submit') {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> '+ entityName +' has been created with success, use the button to create a new one or just press on x to close this window and refresh the '+entityName+' list.</p>'
                          + '</br> <button type="button" class="btn blue btn-submit" onclick="CreateNewUser('+cid+')">New '+entityName+'</button></div> ', 
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            if (window.parent !=null) {
                                if (typeof window.parent.ShowAndRefreshGridCreateUser == "function") {
                                    window.parent.ShowAndRefreshGridCreateUser(Action);
                                }
                            }
                            if (window.opener != null) {
                                if (typeof window.opener.ShowAndRefreshGridCreateUser == "function") {
                                    window.opener.ShowAndRefreshGridCreateUser(Action);
                                }
                                window.close();
                            }
                        }
                    }
                });
            }
            else {
                if (window.parent != null) {
                    if (typeof window.parent.ShowAndRefreshGridCreateUser == "function") {
                        window.parent.ShowAndRefreshGridCreateUser(Action);
                    }
                }
                if (window.opener != null) {
                    window.close();
                }
            }
        }

        function CreateNewUser(cid) {
             if (window.opener != null) {
                 window.opener.NewUser(cid);
             }
             else {
                 window.parent.NewUser(cid);
             }
         }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:UserDetails runat="server" ID="UserDetails" />
</asp:Content>

