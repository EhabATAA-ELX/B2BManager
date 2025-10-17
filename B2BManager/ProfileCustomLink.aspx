<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ProfileCustomLink.aspx.vb" Inherits="ProfileCustomLink" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table align="center" style="margin: 15px">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color width130px">Link Name:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxLinkName"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ControlToValidate="txtBoxLinkName" runat="server" ID="rqFieldValidatorChartTitle" Text="* Required" ForeColor="Red" ValidationGroup="LinkDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color width130px">Link URL:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light" Width="350px" TextMode="MultiLine" Height="40" ID="txtBoxLinkUrl"></asp:TextBox>                        
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ControlToValidate="txtBoxLinkUrl" runat="server" ID="RequiredFieldValidator1" Text="* Required" ForeColor="Red" ValidationGroup="LinkDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Link Icon color:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadColorPicker ID="RadColorPickerGroup" ShowIcon="true" ShowEmptyColor="false" Width="230" runat="server"></telerik:RadColorPicker>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align:center">
                        <asp:Label runat="server" ID="lblInfoMessage" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <input type="button" class="btn red" id="btnCloseWindowLink" value="Cancel" onclick="window.parent.CloseLinktWindow(false)" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateLink" Text="Save" ValidationGroup="LinkDetails" OnClick="btnSaveOrUpdateLink_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveOrUpdateLink" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

