<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UserProfile.aspx.vb" Inherits="UserProfile" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/UserInformation.ascx" TagPrefix="uc1" TagName="UserInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="CSS/UsersManagement.css" rel="stylesheet" />
    <style type="text/css">
        .profile-user-img {
            height: 100px !important;
            min-width:100px !important;
        }
    </style>
    <script type="text/javascript">
        function UploadFile(obj) {
            if (obj.value != '') {
                var length = obj.files[0].size;
                var type = obj.files[0].type;
                if (type.indexOf("image") == -1) {
                    var oWnd = $find("<%= WindowInvalidExtension.ClientID%>");
                    oWnd.show();
                }
                else {
                    document.getElementById("<%=uploadButton.ClientID %>").click();
                }
            }
        }
        function CloseWindowInvalidExtension() {
            var oWnd = $find("<%= WindowInvalidExtension.ClientID%>");
            oWnd.close();
        }
        function deleteLogoClick() {
            document.getElementById("<%=btnDeleteLogo.ClientID %>").click();
        }
        function TryAgain() {
            CloseWindowInvalidExtension();
            $("#changeLogoLbl").trigger("click");
        }
        function FinsihUserSubmit() {
            window.parent.EndUserUpdate();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table style="min-width: 800px">
        <tr>
            <td>
                <table cellpadding="5">
                    <tr>
                        <td rowspan="6">
                            <div class="text-center">
                                <asp:Image CssClass="profile-user-img img-fluid img-circle" ID="userImage" runat="server" />
                                <asp:FileUpload ID="FileUploadControl" ClientIDMode="Static" accept="image/*" AllowMultiple="false" CssClass="hidden" runat="server" onchange="UploadFile(this);" />
                                <asp:Button runat="server" ID="uploadButton" Text="" Style="display: none;" OnClick="uploadButton_Click" />
                                <br />
                                <label id="changeLogoLbl" for="FileUploadControl" class="defaultLink">Upload Photo</label>
                                <span id="deleteLogoLbl" class="defaultLink" runat="server" visible="false" onclick="deleteLogoClick()" >| Reset</span>
                                <asp:Button runat="server" ID="btnDeleteLogo" Text="" OnClick="btnDeleteLogo_Click" Style="display: none;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">User ID:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="ImgTooltipHelp_lblUserID" CssClass="defaultLink"></asp:Label>
                            <div class="hidden" style='margin: 25px;' runat="server" id="TooltipContentHelp_lblUserID"></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Full name:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblDisplayName" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login name:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLoginName" CssClass="Electrolux_Color textHighlithed"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblEmail" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Last connected on:</asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLastConnectionDate" CssClass="Electrolux_Color"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <fieldset>
                    <legend class="Electrolux_Color">User Tags</legend>
                    <table cellpadding="5" cellspacing="5">
                        <tr>
                            <td runat="server" id="UserTagsTD"></td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ContentPlaceHolder1_ImgTooltipHelp_lblUserID" />
        </TargetControls>
    </telerik:RadToolTipManager>

    <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
        <Tabs>
            <telerik:RadTab Text="User Details" Width="160px" PageViewID="RadPageViewDetails"></telerik:RadTab>
            <telerik:RadTab Text="Access Rights" Width="160px" PageViewID="RadPageViewAccessRights"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage" EnableAjaxSkinRendering="true">
        <telerik:RadPageView runat="server" ID="RadPageViewDetails">
            <uc1:UserInformation runat="server" ID="UserInformation" />    
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageViewAccessRights">
            <telerik:RadTreeView runat="server" CheckBoxes="true" ID="treeToolsAndActions" CssClass="ToolsAndActions" TriStateCheckBoxes="true" BorderStyle="None" Height="520" CheckChildNodes="true">
                 </telerik:RadTreeView>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <telerik:RadWindow ID="WindowInvalidExtension" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Invalid Logo" Behaviors="Close" Width="450" Height="160">
        <ContentTemplate>
            <table style="width:100%;padding:15px" cellpadding="15px" cellspacing="5px"> 
                <tr>
                    <td style="text-align:justify" class="Electrolux_Color">
                        It seems that the file you are trying to upload is not a valid image or its extension is not supported. Please try again with a valid image
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center">
                        <input type="button" value="Cancel" class="btn red" onclick="CloseWindowInvalidExtension()" />
                        <input type="button" value="Try again" class="btn bleu" onclick="TryAgain()" />
                    </td>
                </tr>
            </table>
           
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

