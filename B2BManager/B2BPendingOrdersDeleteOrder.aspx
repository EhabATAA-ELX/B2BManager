<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="B2BPendingOrdersDeleteOrder.aspx.vb" Inherits="B2BPendingOrdersDeleteOrder" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function CloseWindow(windows) {
            window.parent.ShowAndRefreshGrid(windows);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdfOrderID" />
            <table align="left">
                <tr>
                    <td colspan="2" align="center">
                        <h4>&nbsp;Are you sure you want to delete this order?</h4>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 15px">
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Deletion reason:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxDeletionReason" CssClass="Electrolux_light_bold Electrolux_Color width180px"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="requiredFieldValidatorReason" ValidationGroup="Deletion" ForeColor="Red" ControlToValidate="txtBoxDeletionReason">* Required</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label runat="server" ForeColor="Red" ID="lblInfo"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <button class="btn red" id="BtnCancelDeleteOrder" onclick="window.parent.CloseWindow('WindowDeleteOrder')">Cancel</button>
                        <asp:LinkButton class="btn green" ID="BtnDeleteOrder" OnClick="BtnDeleteOrder_Click" ValidationGroup="Deletion" runat="server">Confirm</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnDeleteOrder" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

