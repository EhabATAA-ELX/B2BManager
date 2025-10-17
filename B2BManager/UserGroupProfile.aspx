<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UserGroupProfile.aspx.vb" Inherits="UserGroupProfile" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/GroupInformation.ascx" TagPrefix="uc1" TagName="GroupInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/UsersManagement.css" rel="stylesheet" />    
    <script type="text/javascript">
        function FinsihGroupSubmit() {
            window.parent.EndGroupUpdate();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellpadding="5">
        <tr>
            <td>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Group ID:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="ImgTooltipHelp_lblGroupID" CssClass="defaultLink"></asp:Label>
                <div class="hidden" style='margin: 25px;' runat="server" id="TooltipContentHelp_lblGroupID"></div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Name:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="lblGroupName" CssClass="Electrolux_Color"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Description:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="lblDescription" CssClass="Electrolux_Color"></asp:Label>
            </td>
        </tr>        
    </table>

     <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ContentPlaceHolder1_ImgTooltipHelp_lblGroupID" />
        </TargetControls>
    </telerik:RadToolTipManager>

    <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
        <Tabs>
            <telerik:RadTab Text="Group Details" Width="160px" PageViewID="RadPageViewDetails"></telerik:RadTab>
            <telerik:RadTab Text="Users List" Width="160px" PageViewID="RadPageViewUsersList"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage" EnableAjaxSkinRendering="true">
        <telerik:RadPageView runat="server" ID="RadPageViewDetails">
            <uc1:GroupInformation runat="server" ID="GroupInformation" />
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageViewUsersList">
            <iframe runat="server" style="width: 100%; height: 420px;padding-top:15px; border: none" id="iframeUsersList"></iframe>
        </telerik:RadPageView>        
    </telerik:RadMultiPage>

</asp:Content>

