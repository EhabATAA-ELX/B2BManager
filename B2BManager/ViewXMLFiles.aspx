<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ViewXMLFiles.aspx.vb" Inherits="ViewXMLFiles" %>

<%@ Register Src="~/UserControls/XMLPageView.ascx" TagName="WebUserControl" TagPrefix="uc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script src="Scripts/Global.js"></script>
    <script src="Scripts/clipboard.min.js"></script>
    <script type="text/javascript">
        function RenderRefresh() {
            $('#<%= BtnSendRequestToSap.ClientID %>').addClass("loadingBackground").html("Sending..").prop('disabled', true);
            return false;
        }
        
        function SendToSapFinish() {
            $('#<%= BtnSendRequestToSap.ClientID %>').removeClass("loadingBackground").html("Confirm").prop('disabled', false);
            ShowOrCloseWindow("SendToSAP", false);
            ShowOrCloseWindow("SendToSAPInfo", true);
            return false;
        }

        function ShowOrCloseWindow(windowIdentifier, Show) {
            var oWnd = null;
            switch (windowIdentifier) {
                case "Monitor":
                    oWnd = $find("<%= WindowMonitor.ClientID %>");
                    break;
                case "SendToSAP":
                    oWnd = $find("<%= WindowSendToSAP.ClientID %>");
                    break;
                case "SendToSAPInfo":
                    oWnd = $find("<%= WindowSendToSAPInfo.ClientID %>");
                    break;
            }
            if (oWnd != null) {
                if (Show) {
                    oWnd.show();
                }
                else {
                    oWnd.close();
                }
            }
            return false;
        }

        function ProcessButton(sender) {

            switch (sender) {
                case "SaveMointoringMessageForLater":
                    $('#<%= BtnSaveMointoringMessageForLater.ClientID %>').addClass("loadingBackground").html("Saving for later..").prop('disabled', true);
                    break;
                case "SaveMointoringMessageAndEnable":
                    $('#<%= BtnSaveMointoringMessageAndEnable.ClientID %>').addClass("loadingBackground").html("Saving and enabling..").prop('disabled', true);
                    break;
            }
            return false;
        }

    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
                <Tabs>
                    <telerik:RadTab Text="Request" Width="200px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage">
                <telerik:RadPageView runat="server" ID="RadPageView1">
                    <div id="divXMLRequest" runat="server" class="XMLFile"></div>
                    <button class="btn bleu" id="BtnCopyRequest" runat="server" data-clipboard-action="copy" data-clipboard-target="#ContentPlaceHolder1_divXMLRequest">Copy XML</button>
                    <button class="btn bleu" id="BtnViewRequestInBrowser" runat="server">View in Browser</button>
                    <button class="btn bleu" id="BtnDownloadRequestXML" runat="server">Download XML</button>
                    <span runat="server" id="SpanConfirmSendRequestToSap">
                        <button class="btn bleu" id="BtnConfirmSendRequestToSap" onclick="ShowOrCloseWindow('SendToSAP',true)">Send to SAP</button>
                    </span>
                    <span runat="server" id="SpanMonitorThisMessage">
                        <button class="btn bleu" id="BtnMonitorThisMessage" onclick="ShowOrCloseWindow('Monitor',true)">Monitor This Message</button>
                    </span>
                    <div id="divXMLReplySAP" runat="server" visible="false" class="XMLFile"></div>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnCopyRequest" />
            <asp:AsyncPostBackTrigger ControlID="BtnViewRequestInBrowser" />
            <asp:AsyncPostBackTrigger ControlID="BtnDownloadRequestXML" />
        </Triggers>
    </asp:UpdatePanel>



    <telerik:RadWindow ID="WindowSendToSAP" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to SAP" Behaviors="Close" Width="400" Height="150px">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                <ContentTemplate>
                    <h4>&nbsp;Are you sure you want to send this request to SAP?</h4>
                    <table align="right">
                        <tr>
                            <td>
                                <button class="btn red" id="BtnCancelSendRequestToSap" onclick="ShowOrCloseWindow('SendToSAP',false)">Cancel</button>
                            </td>
                            <td>
                                <asp:LinkButton class="btn green" id="BtnSendRequestToSap" runat="server" OnClientClick="RenderRefresh()">Confirm</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BtnSendRequestToSap" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow ID="WindowSendToSAPInfo" runat="server" RenderMode="Lightweight" Modal="false" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send to SAP result" Behaviors="Close" Width="800" Height="200px">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                <ContentTemplate>
                    <div id="divSendToSapInfo" runat="server"></div>
                    <table align="right">
                        <tr>
                            <td>
                                <button class="btn bleu" id="btnViewRequestReplyXML" visible="false" runat="server">View Request/Reply XML</button>
                            </td>
                            <td>
                                <button class="btn green" onclick="ShowOrCloseWindow('SendToSAPInfo',false)">Ok</button>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>

    </telerik:RadWindow>

    <telerik:RadWindow ID="WindowMonitor" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Manage monitoring parameters" Behaviors="Close" Width="500" Height="380px" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                <ContentTemplate>
                    <table align="center" style="margin-top: 30px">
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblIntervalInSeconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Monitoring interval:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="15" Value="30" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="3600" ID="txtIntervalInSeconds" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                            <td align="left">
                                <asp:Label runat="server" ID="lblIntervalUnit" CssClass="Electrolux_light_bold Electrolux_Color">Seconds</asp:Label>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblSendDailyReport" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send daily report:</asp:Label>
                            </td>
                            <td colspan="2" class="PaddingTop5px">
                                <asp:CheckBox ID="ChkBoxSendDailyReport" AutoPostBack="true" OnCheckedChanged="ChkBoxSendDailyReport_CheckedChanged" runat="server" />
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblSendDailyReportOn" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report every day at:</asp:Label>
                            </td>
                            <td colspan="2" align="left">
                                <telerik:RadNumericTextBox runat="server" MinValue="0" Value="12" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="23" ID="txtSendReportHour" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                                <asp:Label runat="server" ID="lblSepearator" CssClass="Electrolux_light_bold Electrolux_Color">:</asp:Label>
                                <telerik:RadNumericTextBox runat="server" MinValue="0" Value="0" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="59" ID="txtSendReportMinute" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblDailyReportEmailTo" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report to:</asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtBoxDailyReportEmailTo" Text="b2b.support@electrolux.com" CssClass="Electrolux_light Electrolux_Color width180px" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblExpectedResponseTimeInMilliseconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Expected Response Time:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="500" Value="1200" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="900000" ID="txtExpectedResponseTimeInMilliseconds" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                            <td align="left">
                                <asp:Label runat="server" ID="lblExpectedResponseTimeInMillisecondsUnit" CssClass="Electrolux_light_bold Electrolux_Color">Milliseconds</asp:Label>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblWorstAcceptableResponseTimeInMilliseconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Worst Acceptable Response Time:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="500" Value="1200" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="900000" ID="txtWorstAcceptableResponseTimeInMilliseconds" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                            <td align="left">
                                <asp:Label runat="server" ID="lblWorstAcceptableResponseTimeInMillisecondsUnit" CssClass="Electrolux_light_bold Electrolux_Color">Milliseconds</asp:Label>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblActivatePerformanceDegradationAlerts" CssClass="Electrolux_light_bold Electrolux_Color width230px">Alert Performance Degradation:</asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:CheckBox ID="ChkBoxActivatePerformanceDegradationAlerts" Height="30" OnCheckedChanged="ChkBoxActivatePerformanceDegradationAlerts_CheckedChanged" CssClass="PaddingTop5px" AutoPostBack="true" runat="server" />
                                <asp:Panel CssClass="floatRight" runat="server" ID="PnlAlert" Visible="false">
                                    <asp:Label runat="server" ID="lblAfter" CssClass="Electrolux_light_bold Electrolux_Color">After</asp:Label>
                                    <telerik:RadNumericTextBox runat="server" MinValue="1" Value="3" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="30" ID="txtPerformanceDegradationAlertMessagesCount" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                    </telerik:RadNumericTextBox>
                                    <asp:Label runat="server" ID="lblMessages" CssClass="Electrolux_light_bold Electrolux_Color">Message(s)</asp:Label>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="lblInfoSaveMonitoringMessage" CssClass="Electrolux_light_bold" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <button class="btn red" id="BtnCancelMonitoringMessage" onclick="ShowOrCloseWindow('Monitor',false)">Cancel</button>
                                <asp:LinkButton CssClass="btn bleu" ID="BtnSaveMointoringMessageForLater" runat="server" Text="Save for later" OnClientClick="ProcessButton('SaveMointoringMessageForLater')" ></asp:LinkButton>
                                <asp:LinkButton CssClass="btn green" ID="BtnSaveMointoringMessageAndEnable" runat="server" Text="Save and enable" OnClientClick="ProcessButton('SaveMointoringMessageAndEnable')"></asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ChkBoxActivatePerformanceDegradationAlerts" />
                    <asp:AsyncPostBackTrigger ControlID="ChkBoxSendDailyReport" />
                    <asp:AsyncPostBackTrigger ControlID="BtnSaveMointoringMessageForLater" />
                    <asp:AsyncPostBackTrigger ControlID="BtnSaveMointoringMessageAndEnable" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>



    <script type="text/javascript">        
        var clipboard = new Clipboard('.btn');
    </script>

</asp:Content>

