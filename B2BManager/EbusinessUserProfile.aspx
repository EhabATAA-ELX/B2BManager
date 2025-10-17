<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessUserProfile.aspx.vb" Inherits="EbusinessUserProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/UserDetails.ascx" TagPrefix="uc1" TagName="UserDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">

        function CloseWindow(Action) {
            if (Action == 'Submit') {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> User has been updated with success</p></div>', 
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            if (window.parent) {
                                if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                                    window.parent.ShowAndRefreshGridUpdate(Action);
                                }
                            }
                            if (window.opener) {
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
                __doPostBack("Refresh","");
            }
        }
        function LoadContent() {
            var EnvID = $('#ContentPlaceHolder1_UserDetails_HD_EnvironmentID').val()
            var Email =$('#ContentPlaceHolder1_UserDetails_txtBoxEmail').val()
            var oWnd = $find("<%= WindowUsersWithsameEmail.ClientID %>");
            oWnd.setUrl("B2BUsersWithSameEmail.aspx?HideHeader=true&Email=" + Email + "&EnvironmentID=" + EnvID);
            oWnd.show();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table style="min-width: 800px; width: 100%" cellspacing="5" cellpadding="5">
        <tr>
            <td>
                <table cellpadding="5" cellspacing="5" class="card card-primary card-outline" style="vertical-align: top">
                    <tr valign="top">
                        <td runat="server" id="imageTR" rowspan="6">
                            <div class="text-center">
                                <asp:Image runat="server" ID="userTypeImage" Width="60" />
                            </div>
                            <h3 class="pprofile-username text-center">
                                <asp:Label runat="server" ID="ImgTooltipHelp_lblUserID" CssClass="defaultLink linkColor Electrolux_Color textHighlithed"></asp:Label>
                                <div class="hidden" style='margin: 25px;' runat="server" id="TooltipContentHelp_lblUserID"></div>
                            </h3>
                        </td>
                    </tr>
                    <tr valign="top" runat="server" id="loginEmailTR">
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login email:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLoginEmail" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:Button runat="server" ID="btnTransformInSuperuser" Text="Transform into superuser"  OnClick="btnTransform_Click" UseSubmitBehavior="false" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td style="width: 130px;">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login name:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLoginName" CssClass="Electrolux_Color textHighlithed"></asp:Label>
                        </td>
                        <td style="width: 130px;" runat="server" id="CustomerCodeLabelNameTD">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Customer Code:</asp:Label>
                        </td>
                        <td runat="server" id="CustomerCodeLabelTD">
                            <asp:Label runat="server" ID="lblCustomerCode" CssClass="Electrolux_Color textHighlithed defaultLink linkColor"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td style="width: 130px;">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Last connected on:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLastConnectionDate" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                        <td style="width: 130px;" runat="server" id="CustomerNameLabelNameTD">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Customer Name:</asp:Label>
                        </td>
                        <td rowspan="3" runat="server" id="CustomerNameLabelTD">
                            <asp:Label runat="server" ID="lblCustomerName" CssClass="Electrolux_Color textHighlithed"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top" runat="server" id="displayNameTR">
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Display Name:</asp:Label></td>
                        <td>
                            <asp:Label runat="server" ID="lblDisplayName" CssClass="Electrolux_Color"></asp:Label></td>
                    </tr>
                    <tr valign="top" runat="server" id="emailTR">
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Notifications email:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblEmail" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <div class="card card-primary">
                    <div class="card-header">
                        <h3 class="rtsTxt" style="font-size: 13px">User Tags</h3>
                    </div>
                    <!-- /.card-header -->
                    <div class="card-body" runat="server" id="UserTagsTD" style="min-height: 92px"></div>
                    <!-- /.card-body -->
                </div>
            </td>
        </tr>
    </table>
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ContentPlaceHolder1_ImgTooltipHelp_lblUserID" />
        </TargetControls>
    </telerik:RadToolTipManager>

    <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0" ValidationGroup="TabStripValidation" EnableAjaxSkinRendering="true">
        <Tabs>
            <telerik:RadTab Text="User Details" Width="160px" PageViewID="RadPageViewDetails"></telerik:RadTab>
            <telerik:RadTab Value="CustomerList" Text="Customer List" Width="160px" Visible="false" runat="server"></telerik:RadTab>
            <telerik:RadTab Value="ActionsHisotry" Text="Actions History" Width="160px" PageViewID="RadPageViewActivityHistory"></telerik:RadTab>
            <telerik:RadTab Value="Contacts" Text="Contacts" Width="160px" PageViewID="RadPageViewContacts"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage">
        <telerik:RadPageView runat="server" ID="RadPageViewDetails">
            <uc1:UserDetails runat="server" ID="UserDetails" />
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageViewCustomerList" CssClass="card">
            <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeCustomerList"></iframe>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageViewActivityHistory">
            <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeActivityLog"></iframe>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageViewContacts">
            <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeContacts"></iframe>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
        <telerik:RadWindow ID="WindowUsersWithsameEmail" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="700px" Height="600px" runat="server">
    </telerik:RadWindow>
</asp:Content>

