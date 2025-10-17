<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InsightsPreferences.ascx.vb" Inherits="UserControls_InsightsPreferences" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <table class="Filters no-print" align="center">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Default Dashboard:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="300" AppendDataBoundItems="true" ID="ddlDashboard">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Home Page Chart:</asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color" Width="300" AppendDataBoundItems="true" ID="ddlCharts">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Default Environment:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="300" AppendDataBoundItems="true" ID="ddlEnvironment">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Default Country:</asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color" Width="300" AppendDataBoundItems="true" ID="ddlCountry">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCountrySplit" CssClass="Electrolux_light_bold Electrolux_Color">Country split by default:</asp:Label>
                </td>
                <td>
                    <label class="switch">
                        <asp:CheckBox runat="server" ID="chkBoxCountrySplit" />
                        <span class="slider round"></span>
                    </label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label runat="server" ID="lblErrorMessageInfo" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <table align="center">
                        <tr>
                            <td>
                                <asp:Panel runat="server" ID="btnCancelPanel" Visible="false">
                                    <a class="btn red" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrClosePreferencesWindow(false)" ><i class="fas fa-ban"></i> Cancel</a></a>
                                </asp:Panel>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="btnUpdate" CssClass="btn lightblue" OnClick="btnUpdate_Click" OnClientClick="ProcessButton(this, 'Saving...');" ><i class="fas fa-check"></i> Save Preferences</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnUpdate" />
    </Triggers>
</asp:UpdatePanel>
