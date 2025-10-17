<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="AutomatedTestsElementManager.aspx.vb" Inherits="AutomatedTestsElementManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function ProcessbtnSaveOrUpdateContainer(btn, processText) {
            if ($('#<%= txtBoxName.ClientID%>').val() != "") {
                ProcessButton(btn, processText);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table runat="server" id="EditTable" style="width: 100%;">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color width130px">Parent Container:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlParentContainer">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="--- Placed in root level ---" Value="0" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblName" CssClass="Electrolux_light_bold Electrolux_Color width130px"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxName"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtBoxName" runat="server" ID="rqFieldValidatorName" Text="* Required" ForeColor="Red" ValidationGroup="ContainerDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color width130px">Description:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxDescription" TextMode="MultiLine" Height="75"></asp:TextBox>
                    </td>
                </tr>
            </table>

            <table align="center" runat="server" id="DeleteTable" style="margin-top: 10px">
                <tr class="Height30px">
                    <td colspan="3" align="center" style="width: 380px; margin: 25px;">
                         <asp:Label runat="server" ID="lblDelete" Width="380" CssClass="Electrolux_light_bold Electrolux_Color"></asp:Label>
                    </td>
                </tr>
            </table>
            <table align="center">
                <tr>
                    <td align="center">
                        <asp:Label runat="server" ID="lblErrorMessageInfo" Width="380" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <input type="button" class="btn bleu" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrCloseWindow('ManageTestElement', false); window.parent.ShowOrCloseWindow('DeleteElement', false)" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateContainer" ClientIDMode="Static" Text="Save" OnClientClick="ProcessbtnSaveOrUpdateContainer(this, 'Saving...')" OnClick="btnSaveOrUpdateContainer_Click" CausesValidation="true" ValidationGroup="ContainerDetails"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveOrUpdateContainer" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

