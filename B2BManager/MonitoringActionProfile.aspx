<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" ValidateRequest="false" AutoEventWireup="false" CodeFile="MonitoringActionProfile.aspx.vb" Inherits="MonitoringActionProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function ProcessButton(sender) {
            if ($("#IsServerNameRequired").val() == "true") {
                if ($("#txtboxServerName").val().length == 0) {
                    return false;
                }
            }            

            if ($("#txtboxInputParameter").val().length == 0) {
                return false;
            }

            switch (sender) {
                case "Execute" :{
                    $('#btnExecute').addClass("loadingBackground").html("Running..").prop('disabled', true);
                    break;
                }
                case "Add": {
                    $('#btnSubmit').addClass("loadingBackground").html("Submitting..").prop('disabled', true);
                    break;
                }
                case "Update": {
                    $('#btnSubmit').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                    break;
                }
            }
            
            return false;
        }

        function CloseWindowAndRefreshGrid() {
            if (window.opener) {
                window.opener.RunSearch();
            }
            else {
                if (window.parent) {
                    window.parent.RunSearch();
                }
            }

            return CloseWindow();
        }

        function CloseWindow() {

            if (window.opener) {
                window.close();
            }
            else {
                window.parent.CloseActionProfileWindow();
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div style="padding: 15px">
                <asp:HiddenField runat="server" ClientIDMode="Static" ID="IsServerNameRequired" Value="true" />
                <table style="width:100%">
                    <tr>
                        <td class="width120px">
                            <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" DataTextField="Name" DataValueField="ID">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblType" CssClass="Electrolux_light_bold Electrolux_Color">Type</asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlType" AutoPostBack="true" DataTextField="ID" DataValueField="Name">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server"  ID="lblComments" CssClass="Electrolux_light_bold Electrolux_Color">Description</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtboxComments" CssClass="Electrolux_Color" Width="400" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr runat="server" id="trCustomImage">
                        <td>
                            <asp:Label runat="server"  ID="lblCustomImage" CssClass="Electrolux_light_bold Electrolux_Color">Custom Image</asp:Label>
                        </td>
                        <td>
                            <asp:Image runat="server" ID="customImage" Width="16" Height="16" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server"  ID="lblServerName" CssClass="Electrolux_light_bold Electrolux_Color">Server Name (*)</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtboxServerName" ClientIDMode="Static" ValidationGroup="ActionSubmit" CssClass="Electrolux_Color" Width="400" ></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="requiredFieldValidatorServerName" ControlToValidate="txtboxServerName" ForeColor="Red" Text="* Required" ValidationGroup="ActionSubmit"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server"  ID="lblInputParameter" CssClass="Electrolux_light_bold Electrolux_Color">Input Parameter (*)</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtboxInputParameter"  ClientIDMode="Static" CssClass="Electrolux_Color" ValidationGroup="ActionSubmit" Width="400"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="requiredFieldValidatorInputParameter" ControlToValidate="txtboxInputParameter" ForeColor="Red" Text="* Required" ValidationGroup="ActionSubmit"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr runat="server" id="trInfoQuery">
                        <td colspan="3">
                            <span style="font-size:9pt" class="Electrolux_Color"><span style="color:#f2c5c8;">[INFO]</span> please note that your query needs to return at least a decimal value named "Ratio". e.g. SELECT 100 As Ratio. This value will be compared with the alert ratio to say whatever the execution is succeeded or not.</span>
                        </td>
                    </tr>
                    <tr runat="server" id="trAlertRatio">
                        <td>
                            <asp:Label runat="server" ID="lblAlertRatio" CssClass="Electrolux_light_bold Electrolux_Color">Alert Ratio (%)</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="1" Value="40" ShowSpinButtons="true" DataType="Decimal" Width="70" MaxValue="99" ID="RadNumericTextBoxAlertRatio" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                        <NumberFormat GroupSeparator="" DecimalDigits="2"  />
                                    </telerik:RadNumericTextBox>
                        </td>
                    </tr>                    
                    <tr runat="server" id="trWarningRatio">
                        <td>
                            <asp:Label runat="server" ID="lblWarningRatio" CssClass="Electrolux_light_bold Electrolux_Color">Warning Ratio (%)</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="1" Value="60" ShowSpinButtons="true" DataType="Decimal" Width="70" MaxValue="99" ID="RadNumericTextBoxWarningRatio" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                        <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                    </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:Label runat="server" ID="lblErrorInfo" ForeColor="Red" Text=" "></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()" />
                            <asp:LinkButton runat="server" CssClass="btn lightblue" ID="btnExecute" ClientIDMode="Static" Text="Run Manually" OnClick="btnExecute_Click" OnClientClick="ProcessButton('Execute')" CausesValidation="true" ValidationGroup="ActionSubmit"></asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn bleu" ID="btnSubmit" ClientIDMode="Static" CausesValidation="true" OnClick="btnSubmit_Click" ValidationGroup="ActionSubmit"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>

            <telerik:RadWindow ID="ActionExecutionInfo" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Execution Info" ShowContentDuringLoad="false" Behaviors="Close" Width="440px" Height="250px" runat="server">
                <ContentTemplate>
                    <div runat="server" id="ExecutionInfo" style="width:100%;height:210px;overflow-y:auto;padding:5px;"></div>
                </ContentTemplate>
            </telerik:RadWindow>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlType" />
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
            <asp:AsyncPostBackTrigger ControlID="btnExecute" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

