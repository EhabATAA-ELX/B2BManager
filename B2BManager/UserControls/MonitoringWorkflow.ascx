<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MonitoringWorkflow.ascx.vb" Inherits="UserControls_MonitoringWorkflow" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/PauseIntervalsManager.ascx" TagPrefix="uc1" TagName="PauseIntervalsManager" %>



<script type="text/javascript">
    function RefreshInfo() {
        __doPostBack('ProcessIDHiddenField', null)
    }
    function OnClientNodeClicking(sender, eventArgs) {
        eventArgs.set_cancel(true);
    }
    function addSubAction(actionID) {
        __doPostBack("WorkflowSelectedActionID", "Add|" + actionID);
    }
    function deleteAction(actionID) {
        __doPostBack("WorkflowSelectedActionIDToDelete", "OpenDeleteWindow|" + actionID);
    }

    function SubmitAction(action) {
        __doPostBack("WorkflowSelectedActionID", "SubmitAction|" + $("#SelectedAction").val() + ";" + $("#chkboxGroupChildren")[0].checked.toString() + ";" + $("#chkboxExecuteChildrenOnFailure")[0].checked.toString() + ";" + $("#WorkflowSelectedActionIsWorkflow").val());
    }

    function ShowOrCloseWindowUC(windowIdentifier, Show) {
        var oWnd = null;
        switch (windowIdentifier) {
            case "AddSubAction":
                $("#SelectedActionInfo").html("No action is selected");
                $("#SelectedAction").val("");
                $("#WorkflowSelectedActionIsWorkflow").val("");
                $("#chkboxGroupChildren")[0].checked = false;
                $("#chkboxExecuteChildrenOnFailure")[0].checked = false;
                $("#ErrorInfoSubAction").text("");
                oWnd = $find("<%= WindowAddSubAction.ClientID %>");
                break;
            case "WindowSelectAction":
                oWnd = $find("<%= WindowSelectAction.ClientID %>");
                break;
            case "DeleteActionAndChildren":
                oWnd = $find("<%= WindowDeleteActionAndChildren.ClientID%>");
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

    function DisplaySelectActionWindow() {
        var url = 'MonitoringSelectAction.aspx?EnvironmentID=' + $("#<%= CurrentEnvironmentID.ClientID%>").val() + "&WorkflowID=" + $("#CurrentWorkflowID").val();
        if ($(window).height() > 900 && $(window).width() > 1350) {
            var oWnd = $find("<%= WindowSelectAction.ClientID %>");
            oWnd.setUrl(url + "&HideHeader=true");
            oWnd.show();
        }
        else {
            popup(url);
        }
    }

    function SelectAction(action) {
        $("#SelectedActionInfo").html("<img src='" + action.ImageUrl + "' width='20' height='20' />&nbsp;<span style='vertical-align:top;color:#2e672e;'>" + action.Comments + "</span>");
        $("#SelectedAction").val(action.ID);
        $("#WorkflowSelectedActionIsWorkflow").val(action.IsActionIDWorkflowID);
        $("#ErrorInfoSubAction").text("");
        ShowOrCloseWindowUC("WindowSelectAction", false);
    }

    function ProcessButtonUC(sender) {

        switch (sender) {
            case "AddSubAction":
                if ($("#SelectedAction").val() == "") {
                    $("#ErrorInfoSubAction").text("Please select an action");
                }
                else {
                    $('#BtnAddSubAction').addClass("loadingBackground").html("Submitting..").prop('disabled', true);
                    SubmitAction();
                }

                break;
            case "UpdateWorkflowName":
                $("#lblWorkflowNameErrorMessage").text(" ");
                if ($("#txtboxWorkflowName").val() != "") {
                    $('#UpdateWorkflowName').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                }
                break;
            case "DeleteActionAndChildren":
                $('#BtnDeleteActionAndChildren').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                __doPostBack("WorkflowSelectedActionIDToDelete", "SubmitDeleteAction");
                break;
            case "UpdateAutomationParameters":
                $('#btnUpdateAutomationParameters').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                break;
            case "ChangeAutomationStatus":
                $('#btnChangeAutomationStatus').addClass("loadingBackground").html("Processing..").prop('disabled', true);
                break;
        }
        return false;
    }
</script>

<telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="1">
    <Tabs>
        <telerik:RadTab Text="Details" PageViewID="pageViewWorkflowDetails" Width="180px"></telerik:RadTab>
        <telerik:RadTab Text="Graph Display" PageViewID="pageViewWorkflowDisplay" Width="180px"></telerik:RadTab>
        <telerik:RadTab Text="Tree Display" PageViewID="pageViewWorkflowTreeDisplay" Width="180px"></telerik:RadTab>
        <telerik:RadTab Text="Automation" PageViewID="pageViewWorkflowAutomation" Width="180px"></telerik:RadTab>
        <telerik:RadTab Text="Execution Logs" PageViewID="pageViewWorkflowLogs" Width="180px"></telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>
<asp:HiddenField runat="server" ID="CurrentWorkflowID" Value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="ProcessIDHiddenField" Value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="CurrentEnvironmentID" Value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="CurrentActivateEditMode" Value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="CurrentExpandSubWorkflows" Value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="CurrentWorkflowUID" Value="" ClientIDMode="Static" />

<telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="1" CssClass="outerMultiPage">
    <telerik:RadPageView runat="server" ID="pageViewWorkflowDetails">
        <asp:UpdatePanel runat="server" ID="UpdatePanel6">
            <ContentTemplate>
                <div runat="server" id="WorkflowDetailsContainer" class="basic-container">
                    <table>
                        <tr>
                            <td class="width180px">
                                <asp:Label runat="server" ID="lblWorkflowID" CssClass="Electrolux_light_bold Electrolux_Color">Workflow ID</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblWorkflowIDValue" ForeColor="GrayText" CssClass="Electrolux_light_bold"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblWorkflowName" CssClass="Electrolux_light_bold Electrolux_Color">Name</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtboxWorkflowName" ClientIDMode="Static" CssClass="Electrolux_Color" ValidationGroup="WorkflowName" Width="400"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" Text="* Required" ForeColor="Red" ControlToValidate="txtboxWorkflowName" ValidationGroup="WorkflowName"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="UpdateWorkflowName" ClientIDMode="Static" ValidationGroup="WorkflowName" CausesValidation="true" Text="Update Name" CssClass="btn bleu" OnClientClick="ProcessButtonUC('UpdateWorkflowName')"></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:Label runat="server" ID="lblWorkflowNameErrorMessage" ClientIDMode="Static" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblCreatedOn" CssClass="Electrolux_light_bold Electrolux_Color">Created on</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblCreatedOnValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblCreatedBy" CssClass="Electrolux_light_bold Electrolux_Color">Created by</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblCreatedByValue"></asp:Label>
                            </td>
                        </tr>
                        <tr runat="server" id="trLastModifiedOn">
                            <td>
                                <asp:Label runat="server" ID="lblLastModifiedOn" CssClass="Electrolux_light_bold Electrolux_Color">Last modified on</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblLastModifiedOnValue"></asp:Label>
                            </td>
                        </tr>
                        <tr runat="server" id="trLastModifiedBy">
                            <td>
                                <asp:Label runat="server" ID="lblLastModifiedBy" CssClass="Electrolux_light_bold Electrolux_Color">Last Modified by</asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblLastModifiedByValue"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UpdateWorkflowName" />
            </Triggers>
        </asp:UpdatePanel>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="pageViewWorkflowDisplay">
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <div class="tree" runat="server" id="workflowTreeDiv"></div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ProcessIDHiddenField" />
            </Triggers>
        </asp:UpdatePanel>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="pageViewWorkflowTreeDisplay">
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <div class="treeStructureContainer">
                    <telerik:RadTreeView runat="server" OnClientNodeClicking="OnClientNodeClicking" AllowNodeEditing="false" EnableDragAndDrop="false" ID="TreeViewStructure"></telerik:RadTreeView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ProcessIDHiddenField" />
            </Triggers>
        </asp:UpdatePanel>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="pageViewWorkflowAutomation">
        <div class="basic-container">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" MultiPageID="RadMultiPage2" SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab Text="Parameters" PageViewID="pageViewWorkflowAutomationDetails" Width="180px"></telerik:RadTab>
                            <telerik:RadTab Text="Pause Intervals" PageViewID="pageViewWorkflowAutomationPauseIntervals" Width="180px"></telerik:RadTab>
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage runat="server" ID="RadMultiPage2" SelectedIndex="0" CssClass="outerMultiPage">
                        <telerik:RadPageView runat="server" ID="pageViewWorkflowAutomationDetails">
                            <table align="left" style="margin-left: 25px" runat="server" id="workflowAutomationDetailsTable">
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color width230px">Current automation status</asp:Label>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Image Width="20" Height="20" runat="server" ID="ImgMonitoringStatus" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCurrentStatus"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="lblIntervalInSeconds" CssClass="Electrolux_light_bold Electrolux_Color width230px">Runs every</asp:Label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox runat="server" MinValue="15" Value="30" ShowSpinButtons="true" DataType="Integer" Width="70" MaxValue="3600" ID="txtIntervalInSeconds" CssClass="Electrolux_light_bold TextAlignCenter">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td align="left">
                                        <asp:Label runat="server" ID="lblIntervalUnit" CssClass="Electrolux_light_bold Electrolux_Color">Seconds</asp:Label>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color width230px">Activate alerts</asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:CheckBox ID="ChkBoxActivateAlerts" Height="30" CssClass="PaddingTop5px" runat="server" />
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send alerts to</asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtBoxAlert" CssClass="Electrolux_light width180px" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="lblActivateWarningNotifications" CssClass="Electrolux_light_bold Electrolux_Color width230px">Activate warning notifications</asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:CheckBox ID="ChkBoxActivateWarningNotifications" Height="30" CssClass="PaddingTop5px" runat="server" />
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send warning notifications to</asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtBoxWarningNotificationsTo" CssClass="Electrolux_light width180px" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="lblSendDailyReport" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send daily report</asp:Label>
                                    </td>
                                    <td colspan="2" class="PaddingTop5px">
                                        <asp:CheckBox ID="ChkBoxSendDailyReport" runat="server" />
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="lblSendDailyReportOn" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report every day at</asp:Label>
                                    </td>
                                    <td colspan="2" align="left">
                                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="12" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="23" ID="txtSendReportHour" CssClass="Electrolux_light_bold TextAlignCenter">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                        <asp:Label runat="server" ID="lblSepearator" CssClass="Electrolux_light_bold Electrolux_Color">:</asp:Label>
                                        <telerik:RadNumericTextBox runat="server" MinValue="0" Value="0" ShowSpinButtons="false" DataType="Integer" Width="40" MaxValue="59" ID="txtSendReportMinute" CssClass="Electrolux_light_bold TextAlignCenter">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td class="width230px">
                                        <asp:Label runat="server" ID="lblDailyReportEmailTo" CssClass="Electrolux_light_bold Electrolux_Color width230px">Send report to</asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtBoxDailyReportEmailTo" CssClass="Electrolux_light width180px" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="Height30px">
                                    <td colspan="3" align="center">
                                        <asp:Label runat="server" ID="lblInfoUpdateAutomationParametersMessage" CssClass="Electrolux_light_bold" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <asp:LinkButton runat="server" ID="btnUpdateAutomationParameters" ClientIDMode="Static" OnClientClick="ProcessButtonUC('UpdateAutomationParameters')" CssClass="btn blue" Text="Update Parameters"></asp:LinkButton>
                                        <asp:LinkButton runat="server" ID="btnChangeAutomationStatus" ClientIDMode="Static" OnClientClick="ProcessButtonUC('ChangeAutomationStatus')"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </telerik:RadPageView>
                        <telerik:RadPageView runat="server" ID="pageViewWorkflowAutomationPauseIntervals">
                            <uc1:PauseIntervalsManager runat="server" ID="PauseIntervalsManager" IsUIDWorkflowID="true" />
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnUpdateAutomationParameters" />
                    <asp:AsyncPostBackTrigger ControlID="btnChangeAutomationStatus" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="pageViewWorkflowLogs">
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <div class="basic-container">
                    <table>
                        <tr>
                            <td colspan="3">
                                <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">&ensp;&ensp;Show excutions logged between:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadDateTimePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                                    <Calendar runat="server">
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today">
                                                <ItemStyle CssClass="rcToday" />
                                            </telerik:RadCalendarDay>
                                        </SpecialDays>
                                    </Calendar>
                                </telerik:RadDateTimePicker>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTo" CssClass="Electrolux_light_bold Electrolux_Color">And:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadDateTimePicker ID="RadDateTimePickerTo" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                                    <Calendar runat="server">
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today">
                                                <ItemStyle CssClass="rcToday" />
                                            </telerik:RadCalendarDay>
                                        </SpecialDays>
                                    </Calendar>
                                </telerik:RadDateTimePicker>
                            </td>
                            <td>
                                <input type="button" class="btn bleu" id="btnRefreshLogs" value="Refresh" onclick="RefreshWithProcess(this,'Refreshing...')" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
                        runat="server" RelativeTo="Element" Position="MiddleRight">
                    </telerik:RadToolTipManager>
                    <telerik:RadGrid runat="server" MasterTableView-ShowHeadersWhenNoRecords="false" OnItemDataBound="monitoringExecutionLogsGrid_ItemDataBound" OnNeedDataSource="monitoringExecutionLogsGrid_NeedDataSource" ID="monitoringExecutionLogsGrid" CssClass="MonitoringGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="20" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Display" HeaderStyle-Width="40">
                                    <ItemTemplate>
                                        <img src='Images/MonitoringWorkflow/workflow.png' width="20" class="MoreInfoImg" height="20" title="Display Workflow"
                                            onclick="DisplayExecutionLog('<%# Eval("ProcessID").ToString() %>','<%# Eval("WorkflowID").ToString() %>')" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="Passed" AllowFiltering="false" UniqueName="Passed" HeaderText="" HeaderStyle-Width="28">
                                    <ItemTemplate>
                                        <img src='Images/<%# Eval("StatusImageName").ToString()%>.png' class="<%# IIf(Eval("OverallStatus").ToString().Equals("Error"), "MoreInfoImg", "")%>" id="StatusImgTooltip_<%# Eval("ProcessID").ToString()%>" width="18" height="18" title="<%# Eval("OverallStatus") %>" />
                                        <div class="hidden" id="StatusTooltipContent_<%# Eval("ProcessID").ToString()%>">
                                            <b>Error details:</b><br />
                                            Message: <%# Eval("ErrorMessage").ToString()%>
                                        </div>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="OverallStatus" HeaderText="Overall status">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="StartedOn" HeaderText="Started on">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="FinishedOn" HeaderText="Finished on">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="ExecutedBy" HeaderText="Executed By">
                                </telerik:GridBoundColumn>
                                <telerik:GridCheckBoxColumn AllowSorting="true" AllowFiltering="true" DataField="ManuallyExecuted" HeaderText="Manually executed?">
                                </telerik:GridCheckBoxColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="TotalSucceededActions" HeaderText="Total Succeeded Actions">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="TotalFailedActions" HeaderText="Total Failed Actions">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn AllowFiltering="true" Aggregate="None" UniqueName="ElapsedTime" DataField="ElapsedTime" SortExpression="ElapsedTime" HeaderText="Execution Time (ms)">
                                    <ItemTemplate>
                                        <span class="<%#  ClsHelper.GetElapsedTimeFont(Eval("ElapsedTime"), 15)%>"><%# Eval("ElapsedTime")%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <NoRecordsTemplate>
                                The selected workflow wasn't executed yet.
                            </NoRecordsTemplate>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="monitoringExecutionLogsGrid" />
            </Triggers>
        </asp:UpdatePanel>
    </telerik:RadPageView>
</telerik:RadMultiPage>

<telerik:RadWindow ID="WindowSelectAction" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" OnClientShow="setPopupTitle" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="1350px" Height="900px" runat="server">
</telerik:RadWindow>


<telerik:RadWindow ID="WindowAddSubAction" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add Monitoring Action" Behaviors="Close" Width="620px" Height="290px" runat="server">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <asp:HiddenField ID="WorkflowSelectedActionID" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="WorkflowSelectedActionIsWorkflow" runat="server" ClientIDMode="Static" />
                <table style="margin: 25px;" align="center">
                    <tr class="Height30px">
                        <td class="width180px">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width230px">Linked Action:</asp:Label>
                        </td>
                        <td>
                            <div runat="server" id="SelectedActionInfo" clientidmode="Static" style="width: 230px; height: 60px"></div>
                            <asp:HiddenField runat="server" ID="SelectedAction" ClientIDMode="Static" />
                        </td>
                        <td style="vertical-align: top; padding-top: 3px" onclick='DisplaySelectActionWindow()' class="MoreInfoImg">
                            <img src="Images/select.png" width="20" /><span style="vertical-align: top;" class='action-button'>Select Action</span>
                        </td>
                    </tr>
                    <tr class="Height30px">
                        <td class="width180px">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width230px">Group children:</asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkboxGroupChildren" />
                        </td>
                    </tr>
                    <tr class="Height30px">
                        <td class="width180px">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width230px">Execute children on failure:</asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkboxExecuteChildrenOnFailure" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Label runat="server" ID="ErrorInfoSubAction" ClientIDMode="Static" ForeColor="Red" Text=" "></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <input type="button" class="btn red" id="BtnCancelAddSubAction" value="Cancel" onclick="ShowOrCloseWindowUC('AddSubAction', false)" />
                            <input type="button" class="btn bleu" id="BtnAddSubAction" value="Submit" onclick="ProcessButtonUC('AddSubAction')" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="WorkflowSelectedActionID" />
            </Triggers>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow ID="WindowDeleteActionAndChildren" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Monitoring Action" Behaviors="Close" Width="400px" Height="150px" runat="server">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <asp:HiddenField ID="WorkflowSelectedActionIDToDelete" runat="server" ClientIDMode="Static" />
                <table align="center" style="margin-top: 10px">
                    <tr class="Height30px">
                        <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete this action and sub-action(s)?</td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Label runat="server" ID="lblDeleteActionAndChildrenErrorMessage" ForeColor="Red" Text=" "></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <input type="button" class="btn red" id="BtnCancelDeleteActionAndChildren" value="Cancel" onclick="ShowOrCloseWindowUC('DeleteActionAndChildren', false)" />
                            <input type="button" class="btn green" id="BtnDeleteActionAndChildren" value="Delete" onclick="ProcessButtonUC('DeleteActionAndChildren')" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="WorkflowSelectedActionIDToDelete" />
            </Triggers>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
