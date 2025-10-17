<%@ Page Language="VB" AutoEventWireup="false" CodeFile="B2BUsersWithSameEmail.aspx.vb" Inherits="B2BUsersWithSameEmail" MasterPageFile="~/BasicMasterPage.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        div.RadGrid .rgPager .rgAdvPart {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table style="border-collapse:separate; border-spacing:1em;">
        <tr>
            <td colspan="2">
                <i class="fas fa-exclamation-triangle" style="color: #0094ff; margin-top: 2px;"></i>
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">This email is used for notifications in multiple accounts</asp:Label>
            </td>
        </tr>
        <tr>

        </tr>
        <tr>
            <td style="width:50px">
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="EmailLbl" CssClass="Electrolux_light_bold" ForeColor="#0070c5"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="updatePanel1">
        <ContentTemplate>
            <div style="width: 100%; overflow-x: auto">
                <telerik:RadGrid runat="server" ID="gridResult" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="10" OnNeedDataSource="gridResult_NeedDataSource" GroupingEnabled="true">
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                   
                     <NoRecordsTemplate>No records found.</NoRecordsTemplate>

                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Country" DataField="CY_NAME_ISOCODE" />
                        <telerik:GridBoundColumn HeaderText="Customer" DataField="C_CUID" />
                        <telerik:GridBoundColumn HeaderText="Login" DataField="varUserName" />
                        <telerik:GridBoundColumn HeaderText="Name" DataField="varName" />
                    </Columns>
                         </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gridResult" />
        </Triggers>
    </asp:UpdatePanel>
 </asp:Content>
