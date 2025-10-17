<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="B2BCacheManagerDisplayKeyValue.aspx.vb" Inherits="B2BCacheManagerDisplayKeyValue" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        div.RadGrid .rgPager .rgAdvPart {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td colspan="2">
                <div runat="server" id="CountryDetails"></div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Cache Key Name:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="cacheKeyNameLbl" CssClass="Electrolux_Color" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Cache Key Type:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="cacheKeyTypeLbl" CssClass="Electrolux_light_bold Electrolux_Color"></asp:Label>
            </td>
        </tr>
    </table>
    <div runat="server" id="HtmlInfo"></div>
    <asp:HiddenField runat="server" ID="dataSourceGUID" />
    <asp:UpdatePanel runat="server" ID="updatePanel1">
        <ContentTemplate>
            <div style="width: 100%; overflow-x: auto">
                <telerik:RadGrid runat="server" ID="gridResult" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="10" OnNeedDataSource="gridResult_NeedDataSource" GroupingEnabled="true">
                    <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gridResult" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

