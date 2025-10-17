<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="MonitoringTool.aspx.vb" Inherits="MonitoringTool" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <script src="Scripts/ECharts/echarts-all.js"></script>
    <script src="Scripts/Monitoring.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <ClientEvents OnRequestStart="RequestStart" />
        <ClientEvents OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" ClientIDMode="Static" IsSticky="true" Transparency="10" runat="server" Style="position: absolute;">
        <asp:Image ID="Image1" runat="server" AlternateText="Loading..." ImageUrl="Images/Loading.gif" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
    </telerik:RadToolTipManager>
    <asp:UpdatePanel runat="server" ID="GridUpdatePanel">
        <ContentTemplate>
            <telerik:RadGrid runat="server" ID="gridSearch" CssClass="MonitoringGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="20" GroupingEnabled="true">
                <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                    <Columns>
                        <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Actions">
                            <ItemTemplate>
                                <img src='Images/edit.png' width="20" class="MoreInfoImg" height="20" title="Display/edit details"
                                    onclick="popup('MonitoringProfile.aspx?MessageID=<%# Eval("MessageID").ToString() %>')" />
                                <img id="chartImg_<%# Eval("MessageID") %>" src='Images/<%# IIF( Eval("IsActive"), "deleteChart", "disabledChart")  %>.png' width="20" class="<%# IIF( Eval("IsActive"), "LineChartImg", "LineChartImgDisabled")  %>" height="20" title="Show/hide lines"
                                    onclick="<%# IIF( Eval("IsActive"), "ToogleLayer('" & Eval("MessageID").ToString() & "')", "return false;")  %>" />
                                <img id="Manage_<%# Eval("MessageID") %>" src='Images/<%# IIF( Eval("IsActive"), "pause", "play")  %>.png' style="max-width:20px;max-height:20px;" title='<%# IIF( Eval("IsActive"), "Disable", "Enable")  %>' class="MoreInfoImg" 
                                    onclick="<%# "ChangeActivationStatus('" & Eval("MessageID").ToString() & "')" %>" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridCheckBoxColumn AllowSorting="true" AllowFiltering="true" DataField="IsActive"  HeaderText="Active?">
                        </telerik:GridCheckBoxColumn>
                        <telerik:GridTemplateColumn DataField="RealTimeStatus" AllowFiltering="true" UniqueName="RealTimeStatus" HeaderText="Real Time Status">
                            <ItemTemplate>
                                <span class="notification-style" <%# "style='background-color:" & Eval("RealTimeStatusColor") & "'" %>><%# Eval("RealTimeStatus") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="MessageType" HeaderText="Message Type">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="SOP_ID" AllowFiltering="true" UniqueName="SOP_ID" HeaderText="Monitoring Country">
                            <ItemTemplate>
                                <img src='Images/Flags/<%# Eval("CY_NAME_ISOCODE").ToString() %>.png' width="20" height="16" title="<%# Eval("CY_NAME") %>" />
                                <span class="verticalAlignTop"><%# Eval("SOP_ID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="Environment" HeaderText="Environment">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="MessageIntervalInSeconds" HeaderText="Interval In Seconds">
                        </telerik:GridBoundColumn>
                        <telerik:GridCheckBoxColumn AllowSorting="true" AllowFiltering="true" DataField="SendDailyReport" HeaderText="Send Daily Report">
                        </telerik:GridCheckBoxColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="DailyReportTime" HeaderText="Daily Report Time">
                            <ItemTemplate>
                                <span class="verticalAlignTop"><%# ClsHelper.GetTime(Eval("DailyReportHour"), Eval("DailyReportMinute")) %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="DailyReportEmailTo" HeaderText="Daily Report Email To">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="ExpectedResponseTimeInMilliseconds" HeaderText="Expected Response Time (ms)">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="WorstAcceptableResponseTimeInMilliseconds" HeaderText="Worst Acceptable Response Time (ms)">
                        </telerik:GridBoundColumn>
                        <telerik:GridCheckBoxColumn AllowSorting="true" AllowFiltering="true" DataField="ActivatePerformanceDegradationAlerts" HeaderText="Activate Performance Degradation Alerts?">
                        </telerik:GridCheckBoxColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="PerformanceDegradationAlertMessagesCount" HeaderText="Performance Degradation Alerts After (message)">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="CreatedBy" HeaderText="Created By" HeaderStyle-Width="60">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="CreatedOn" HeaderText="Created On" HeaderStyle-Width="60">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gridSearch" />
        </Triggers>
    </asp:UpdatePanel>
    <br />
    <table align="center">
        <tr>
            <td>Display data recorded in the last
            </td>
            <td>
                <select name="dllTime" id="dllTime" class="ddlChartLineTime" style="width: 150px;" onchange="GetData()">
                    <option selected="selected" value="15">15 minutes</option>
                    <option value="30">30 minutes</option>
                    <option value="45">45 minutes</option>
                    <option value="60">one hour</option>
                    <option value="120">2 hours</option>
                    <option value="180">3 hours</option>
                    <option value="240">4 hours</option>
                    <option value="720">12 hours</option>
                    <option value="1440">24 hours</option>
                </select>
            </td>
        </tr>
    </table>

    <!-- preparing a DOM with width and height for ECharts -->
    <div id="mainChart" style="width: 100%; height: 600px;"></div>
</asp:Content>

