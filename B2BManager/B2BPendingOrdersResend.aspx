<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/BasicMasterPage.master" CodeFile="B2BPendingOrdersResend.aspx.vb" Inherits="B2BPendingOrdersResend" %>

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
            <h4>&nbsp;You are about to resend order(s)?</h4>
            <table align="right">
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label runat="server" ForeColor="Red" ID="lblInfo"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn red" id="BtnCancelSendRequestToSap" onclick="window.parent.CloseWindow('WindowSendToSAP')">Cancel</button>
                    </td>
                    <td>
                        <asp:LinkButton class="btn green" ID="BtnSendRequestToSap" runat="server">Confirm</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnSendRequestToSap" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
