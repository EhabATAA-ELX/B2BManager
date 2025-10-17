<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="SAPOrderTypeManagement.aspx.vb" Inherits="SAPOrderTypeManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <script type="text/javascript">
        function CloseWindow() {
            if (typeof window.parent.CloseWindow == "function") {
                window.parent.CloseWindow();
            }
        }

        function ProcessButton(sender) {
            if ($("#<%= txtBoxOrderType.ClientID %>").val().length == 0) {
                return;
            }
            if ($("#<%= txtBoxSAPSalesDocType.ClientID %>").val().length == 0) {
                return;
            }
            switch (sender) {
                case "Add": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Submitting..").prop('disabled', true);
                    break;
                }
                case "Update": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                    break;
                }
            }

            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="2" style="margin: 15px; width: 480px" align="center">
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">B2B Order Type (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxOrderType" MaxLength="2" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left" >
                        <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxOrderType" ControlToValidate="txtBoxOrderType" ForeColor="Red" ErrorMessage="* required" />
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">SAP Sales Doc Type (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxSAPSalesDocType" MaxLength="4" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left" >
                        <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxSAPSalesDocType" ControlToValidate="txtBoxSAPSalesDocType" ForeColor="Red" ErrorMessage="* required" />
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Order Reason:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxOrderReason" MaxLength="4" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Shipping Condition:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxShippingCondition" MaxLength="4" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:Label runat="server" ID="lblErrorInfo" ForeColor="Red" Text=" "></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="ButtonsTR">
                    <td colspan="3" align="center">
                        <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" CausesValidation="false" OnClientClick="CloseWindow()">Cancel</asp:LinkButton>
                        <asp:LinkButton runat="server" ID="btnSubmit" class="btn bleu" Text="Submit changes" OnClick="btnSubmit_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

