<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="MonitoringProfile.aspx.vb" Inherits="MonitoringProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/XMLPageView.ascx" TagName="WebUserControl" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/PauseIntervalsManager.ascx" TagPrefix="uc1" TagName="PauseIntervalsManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script src="Scripts/Global.js"></script>
    <script src="Scripts/clipboard.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
                <Tabs>
                    <telerik:RadTab Text="Details" Width="200px"></telerik:RadTab>
                    <telerik:RadTab Text="Pause Intervals" Width="200px"></telerik:RadTab>
                    <telerik:RadTab Text="XML Request" Width="200px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage">
                <telerik:RadPageView runat="server" ID="RadPageView1">
                    <table align="left" style="margin-top: 30px; width: 100%; margin-left: 25px">
                        <tr>
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblMessageTitle" CssClass="Electrolux_light_bold Electrolux_Color width230px">Message ID:</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblMessage" CssClass="Electrolux_light width230px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblWcfB2BWebServiceURLTitle" CssClass="Electrolux_light_bold Electrolux_Color width230px">WCF B2B Web Service URL:</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" Enabled="false" ID="lblWcfB2BWebServiceURL" CssClass="Electrolux_light width230px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblWcfMethodNameTitle" CssClass="Electrolux_light_bold Electrolux_Color width230px">Method (Name/URL):</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblWcfMethodName" CssClass="Electrolux_light width230px"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table align="left" style="margin-left: 25px">
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblIntervalInSeconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Monitoring interval:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="15" Value="30" ShowSpinButtons="true" Enabled="false" DataType="Integer" Width="70" MaxValue="3600" ID="txtIntervalInSeconds" CssClass="Electrolux_light_bold TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                            <td align="left">
                                <asp:Label runat="server" ID="lblIntervalUnit" CssClass="Electrolux_light_bold Electrolux_Color">Seconds</asp:Label>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server"  ID="lblSendDailyReport" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send daily report:</asp:Label>
                            </td>
                            <td colspan="2" class="PaddingTop5px">
                                <asp:CheckBox ID="ChkBoxSendDailyReport" Enabled="false" runat="server" />
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblSendDailyReportOn" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report every day at:</asp:Label>
                            </td>
                            <td colspan="2" align="left">
                                <telerik:RadNumericTextBox runat="server" MinValue="0" Enabled="false" Value="12" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="23" ID="txtSendReportHour" CssClass="Electrolux_light_bold TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                                <asp:Label runat="server" ID="lblSepearator" CssClass="Electrolux_light_bold Electrolux_Color">:</asp:Label>
                                <telerik:RadNumericTextBox runat="server" Enabled="false" MinValue="0" Value="0" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="59" ID="txtSendReportMinute" CssClass="Electrolux_light_bold TextAlignCenter">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblDailyReportEmailTo" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report to:</asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtBoxDailyReportEmailTo" Text="b2b.support@electrolux.com" Enabled="false" CssClass="Electrolux_light width180px" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" ID="lblExpectedResponseTimeInMilliseconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Expected Response Time:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="500" Enabled="false" Value="1200" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="900000" ID="txtExpectedResponseTimeInMilliseconds" CssClass="Electrolux_light_bold TextAlignCenter">
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
                                <telerik:RadNumericTextBox runat="server" MinValue="500" Enabled="false" Value="1200" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="900000" ID="txtWorstAcceptableResponseTimeInMilliseconds" CssClass="Electrolux_light_bold TextAlignCenter">
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
                                <asp:CheckBox ID="ChkBoxActivatePerformanceDegradationAlerts" Enabled="false" Height="30" CssClass="PaddingTop5px" runat="server" />
                                <asp:Panel CssClass="floatRight" runat="server" ID="PnlAlert" Visible="false">
                                    <asp:Label runat="server" ID="lblAfter" CssClass="Electrolux_light_bold Electrolux_Color">After</asp:Label>
                                    <telerik:RadNumericTextBox runat="server" MinValue="1" Enabled="false" Value="3" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="30" ID="txtPerformanceDegradationAlertMessagesCount" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
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
                    </table>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageView2">
                    <uc1:PauseIntervalsManager runat="server" id="PauseIntervalsManagerUC" IsUIDWorkflowID="False" />
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageView3">
                    <uc1:WebUserControl ID="xmlPageView" runat="server" />
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">        
        var clipboard = new Clipboard('.btn');
    </script>
</asp:Content>

