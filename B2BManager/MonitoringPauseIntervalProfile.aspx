<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="MonitoringPauseIntervalProfile.aspx.vb" Inherits="MonitoringPauseIntervalProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function CloseWindow() {

            if (window.parent) {
                window.parent.ShowOrClosePauseIntervalWindow("ManagePauseInterval", false);
            }
            if (window.opener) {
                window.close();
            }
            return false;
        }

        function FinishChange() {
            if (window.parent) {
                window.parent.BindPauseIntervals();
            }
            if (window.opener) {
                window.opener.BindPauseIntervals();
            }
            CloseWindow();
        }
        
        function ProcessPauseIntervalButton(sender) {

            switch (sender) {
                case "Save":
                    $('#<%= BtnSavePauseInterval.ClientID %>').addClass("loadingBackground").html("Saving..").prop('disabled', true);
                    break;
                case "Update":
                    $('#<%= BtnSavePauseInterval.ClientID %>').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                    break;
            }
            return false;
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table align="center" style="margin-top: 30px">
                <tr class="Height30px">
                    <td class="width230px">
                        <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color width230px">Occurs every day:</asp:Label>
                    </td>
                    <td colspan="2" class="PaddingTop5px">
                        <asp:CheckBox ID="ChkBoxOccursEveryDay" AutoPostBack="true" OnCheckedChanged="ChkBoxOccursEveryDay_CheckedChanged" Checked="true" runat="server" />
                    </td>
                </tr>
                <tr class="Height30px">
                    <td class="width230px">
                        <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color width230px">Start pausing at:</asp:Label>
                    </td>
                    <td colspan="2" align="left">
                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="12" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="23" ID="txtStartOccurenceHour" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                        <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color">:</asp:Label>
                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="00" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="59" ID="txtStartOccurenceMinute" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr class="Height30px">
                    <td class="width230px">
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color width230px">End pausing at:</asp:Label>
                    </td>
                    <td colspan="2" align="left">
                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="16" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="23" ID="txtEndOccurenceHour" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                        <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">:</asp:Label>
                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="00" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="59" ID="txtEndOccurenceMinute" CssClass="Electrolux_light_bold Electrolux_Color TextAlignCenter">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr class="Height30px">
                    <td class="width230px">
                        <asp:Label runat="server" ID="Label6" CssClass="Electrolux_light_bold Electrolux_Color width230px">Start occurrence from:</asp:Label>
                    </td>
                    <td colspan="2" align="left">
                        <telerik:RadDateTimePicker ID="RadDateTimePickerOccursFrom" Enabled="false" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>
                </tr>
                <tr class="Height30px">
                    <td class="width230px">
                        <asp:Label runat="server" ID="Label7" CssClass="Electrolux_light_bold Electrolux_Color width230px">End occurrence on:</asp:Label>
                    </td>
                    <td colspan="2" align="left">
                        <telerik:RadDateTimePicker ID="RadDateTimePickerOccursTo" Enabled="false" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>
                </tr>
                <tr class="Height30px">
                    <td colspan="3" align="center">
                        <asp:Label runat="server" ID="Label14" CssClass="Electrolux_light_bold" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <button class="btn red" runat="server" id="BtnCancelSavePauseInterval" onclick="CloseWindow()">Cancel</button>
                        <asp:LinkButton CssClass="btn green" ID="BtnSavePauseInterval" runat="server" OnClick="BtnSavePauseInterval_Click" Text="Save" OnClientClick="ProcessPauseIntervalButton('Save')"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnSavePauseInterval" />
            <asp:AsyncPostBackTrigger ControlID="ChkBoxOccursEveryDay" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

