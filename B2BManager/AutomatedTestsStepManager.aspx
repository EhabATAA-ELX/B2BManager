<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="AutomatedTestsStepManager.aspx.vb" Inherits="AutomatedTestsStepManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function ValidateAndProcessButton(sender, text) {
            if ($("#<%= txtBoxTarget.ClientID%>").val() != "") {
                ProcessButton(sender, text);
            }
        }
        function CloseTestStepWindows() {
            window.parent.ShowOrClosetestStepWindow('ManageTestStep', false);
            window.parent.ShowOrClosetestStepWindow('DeleteTestStep', false);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table runat="server" id="EditTable" style="width: 100%;">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblCommand" Text="Command" CssClass="Electrolux_light_bold Electrolux_Color width130px"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color" Width="280" ID="ddlCommandName">
                            <ItemTemplate>
                                <img src='<%# ResolveUrl(Eval("ImageUrl").ToString()) %>' width="auto" height="18" />
                                <span class="verticalAlignTop"><%# Eval("Command") %></span>
                            </ItemTemplate>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblTarget" Text="Target" CssClass="Electrolux_light_bold Electrolux_Color width130px"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light" Width="280" TextMode="MultiLine" Height="40" ID="txtBoxTarget"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtBoxTarget" runat="server" ID="rqFieldValidatorTarget" Text="* Required" ForeColor="Red" ValidationGroup="StepDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblValue" Text="Value" CssClass="Electrolux_light_bold Electrolux_Color width130px"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light" Width="280" TextMode="MultiLine" Height="75" ID="txtBoxValue"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color width130px">Description:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light" Width="280" ID="txtBoxDescription" TextMode="MultiLine" Height="75"></asp:TextBox>
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
                        <input type="button" class="btn bleu" id="btnCancelDashboardProfile" value="Cancel" onclick="CloseTestStepWindows()" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateTestStep" ClientIDMode="Static" Text="Save" OnClientClick="ValidateAndProcessButton(this, 'Saving...')" OnClick="btnSaveOrUpdateTestStep_Click" CausesValidation="true" ValidationGroup="StepDetails"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveOrUpdateTestStep" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

