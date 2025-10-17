<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EbusinessPreferences.ascx.vb" Inherits="UserControls_EbusinessPreferences" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <table class="Filters" align="center">
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Default Environment:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color Width316px" ID="ddlEnvironment" DataTextField="Name" DataValueField="ID">
                    </asp:DropDownList>
                </td>                
            </tr>
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Default Country:</asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color Width316px" Width="316" AppendDataBoundItems="true" ID="ddlCountry">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Display/Manage by default:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color Width316px" ID="ddlManagementType" AppendDataBoundItems="true">
                        <asp:ListItem Selected="True" Text="Customer & users" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Super users" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Expand rows on search by default:</asp:Label>
                </td>
                <td>
                    <label class="switch">
                        <input runat="server" id="chkBoxExpandOnSearch" type="checkbox" />
                        <span class="slider round"></span>
                    </label>
                </td>
            </tr>
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color">Activate window mode by default:</asp:Label>
                </td>
                <td>
                    <label class="switch">
                        <input runat="server" id="chkBoxDisplayMode" type="checkbox" />
                        <span class="slider round"></span>
                    </label>
                </td>
            </tr>
            <tr>
                <td class="width230px">
                    <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color">Default sorting for customers by:</asp:Label>
                </td>
                <td class="Width316px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="220" ID="ddlCustomersDefaultSortingBy" AppendDataBoundItems="true">
                    </asp:DropDownList>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" ID="ddlSortDirection" AppendDataBoundItems="true">
                        <asp:ListItem Selected="True" Text="Ascending" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Descending" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="width230px" style="vertical-align: top">
                    <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color">Fields shown by default in customer, user & super user lists:</asp:Label>
                </td>
                <td>
                    <telerik:RadTreeView runat="server" ID="tvFields" CheckBoxes="true" ShowLineImages="false" CheckChildNodes="true" TriStateCheckBoxes="true" Width="320" Height="200">
                    </telerik:RadTreeView>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label runat="server" ID="lblErrorMessageInfo" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center">
                    <table align="center">
                        <tr>
                            <td>
                                <asp:Panel runat="server" Visible="false" ID="btnCancelPanel">
                                    <a id="btnCancel" class="btn red" onclick="window.parent.CloseChangePreferencesWindow()" ><i class="fas fa-ban"></i> Cancel</a>
                                </asp:Panel>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="btnSavePreferences" OnClick="btnSavePreferences_Click" CssClass="btn lightblue" OnClientClick="ProcessButton(this, 'Saving...');" ><i class="fas fa-check"></i> Save Preferences</asp:LinkButton>
                            </td>
                        </tr>
                    </table>                                       
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSavePreferences" />
    </Triggers>
</asp:UpdatePanel>
