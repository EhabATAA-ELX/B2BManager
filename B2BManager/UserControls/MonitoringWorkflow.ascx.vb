Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Drawing
Imports System.Net
Imports System.Threading
Imports SAPRequestsLib
Imports SAPRequestsLib.Models
Imports SAPRequestsLib.MonitoringWorkflowManager
Imports Telerik.Web.UI

Partial Class UserControls_MonitoringWorkflow
    Inherits System.Web.UI.UserControl

    Private _workflowID As Integer
    Private _runWorkflow As Boolean
    Private _activateEditMode As Boolean
    Private _expandSubWorkflows As Boolean
    Private _ProcessID As Guid

    Public Property WorkflowID As Integer
        Get
            Return _workflowID
        End Get
        Set(value As Integer)
            _workflowID = value
        End Set
    End Property

    Public Property ProcessID As Guid
        Get
            Return _ProcessID
        End Get
        Set(value As Guid)
            _ProcessID = value
        End Set
    End Property

    Public Property RunWorkflow As Boolean
        Get
            Return _runWorkflow
        End Get
        Set(value As Boolean)
            _runWorkflow = value
        End Set
    End Property

    Public Property ActivateEditMode As Boolean
        Get
            Return _activateEditMode
        End Get
        Set(value As Boolean)
            _activateEditMode = value
        End Set
    End Property

    Public Property ExpandSubWorkflows As Boolean
        Get
            Return _expandSubWorkflows
        End Get
        Set(value As Boolean)
            _expandSubWorkflows = value
        End Set
    End Property

    Private Const ADD_SUB_ACTION_STRING As String = "<span class='action-button' title='Add sub-action' onclick='addSubAction(""{0}"");'>Add</span>"
    Private Const DELETE_ACTION_STRING As String = "<span class='action-button' title='Delete sub-action and its children' onclick='deleteAction(""{0}"");'>Remove</span>"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim workflow As MonitoringWorkflow = Nothing
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        WindowAddSubAction.VisibleOnPageLoad = False
        WindowDeleteActionAndChildren.VisibleOnPageLoad = False
        Dim RefreshWorkflowOnly As Boolean = False

        If WorkflowID = 0 Then
            If Not String.IsNullOrEmpty(CurrentWorkflowID.Value) Then
                WorkflowID = CInt(CurrentWorkflowID.Value)
            End If
        End If

        If __EVENTTARGET IsNot Nothing Then
            If __EVENTTARGET.Equals("WorkflowSelectedActionID") Then
                If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                    Dim args As String() = __EVENTARGUMENT.Split("|")
                    If args(0).Equals("Add") Then
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Show Add dialog", "ShowOrCloseWindowUC('AddSubAction',true);", True)
                        WindowAddSubAction.VisibleOnPageLoad = True
                        WorkflowSelectedActionID.Value = args(1)
                        Return
                    End If
                    If args(0).Equals("SubmitAction") Then
                        Dim subArgs As String() = args(1).Split(";")
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                        parameters.Add(New SqlParameter("@ParentWorkflowActionID", CInt(WorkflowSelectedActionID.Value)))
                        parameters.Add(New SqlParameter("@ActionID", CInt(subArgs(0))))
                        parameters.Add(New SqlParameter("@IsActionIDWorkflowID", CBool(subArgs(3))))
                        parameters.Add(New SqlParameter("@GroupChildren", CBool(subArgs(1))))
                        parameters.Add(New SqlParameter("@ExecuteChildrenOnFailure", CBool(subArgs(2))))
                        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                        ClsDataAccessHelper.ExecuteNonQuery("Monitoring.AddSubAction", parameters)
                        ClsHelper.Log("Add Workflow sub-action", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                        RefreshWorkflowOnly = True
                    End If
                End If
            End If

            If __EVENTTARGET.Equals("WorkflowSelectedActionIDToDelete") Then
                If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                    Dim args As String() = __EVENTARGUMENT.Split("|")

                    If args(0).Equals("OpenDeleteWindow") Then
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Show Add dialog", "ShowOrCloseWindowUC('DeleteActionAndChildren',true);", True)
                        WindowDeleteActionAndChildren.VisibleOnPageLoad = True
                        WorkflowSelectedActionIDToDelete.Value = args(1)
                    End If

                    If args(0).Equals("SubmitDeleteAction") Then
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                        parameters.Add(New SqlParameter("@WorkflowActionID", CInt(WorkflowSelectedActionIDToDelete.Value)))
                        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                        ClsDataAccessHelper.ExecuteNonQuery("Monitoring.DeleteActionAndChildren", parameters)
                        ClsHelper.Log("Delete Workflow Action And Children", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                        RefreshWorkflowOnly = True
                    End If

                End If

            End If

            If __EVENTTARGET.Contains("chkBoxEditMode") Then
                CurrentActivateEditMode.Value = ActivateEditMode
                RefreshWorkflowOnly = True
            Else
                Boolean.TryParse(CurrentActivateEditMode.Value, ActivateEditMode)
            End If

            If __EVENTTARGET.Contains("chkBoxExpandSubWorkflow") Then
                CurrentExpandSubWorkflows.Value = ExpandSubWorkflows
                RefreshWorkflowOnly = True
            Else
                Boolean.TryParse(CurrentExpandSubWorkflows.Value, ExpandSubWorkflows)
            End If

            If __EVENTTARGET.Contains("UpdateWorkflowName") Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@Name", txtboxWorkflowName.Text))
                parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                If (ClsDataAccessHelper.ExecuteNonQuery("Monitoring.UpdateWorkflowName", parameters)) Then
                    ClsHelper.Log("Update Workflow Name", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Bind Workflows", "__doPostBack('RefreshHiddenField','BindWorkflows');", True)
                Else
                    lblWorkflowNameErrorMessage.Text = "An unexpected error has occurred. please try again later."
                    ClsHelper.Log("Update Workflow Name", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
                End If
            End If

            If __EVENTTARGET.Contains("btnUpdateAutomationParameters") Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                parameters.Add(New SqlParameter("@MonitoringIntervalInSeconds", txtIntervalInSeconds.Value))
                parameters.Add(New SqlParameter("@ActivateAlerts", ChkBoxActivateAlerts.Checked))
                parameters.Add(New SqlParameter("@SendAlertsEmailTo", txtBoxAlert.Text))
                parameters.Add(New SqlParameter("@ActivateWarningNotifications", ChkBoxActivateWarningNotifications.Checked))
                parameters.Add(New SqlParameter("@SendWarningNotificationsEmailTo", txtBoxWarningNotificationsTo.Text))
                parameters.Add(New SqlParameter("@SendDailyReport", ChkBoxSendDailyReport.Checked))
                parameters.Add(New SqlParameter("@DailyReportHour", txtSendReportHour.Value))
                parameters.Add(New SqlParameter("@DailyReportMinute", txtSendReportMinute.Value))
                parameters.Add(New SqlParameter("@DailyReportEmailTo", txtBoxDailyReportEmailTo.Text))
                If (ClsDataAccessHelper.ExecuteNonQuery("Monitoring.UpdateWorkflowAutomationParameters", parameters)) Then
                    RefreshWorkflowOnly = True
                    ClsHelper.Log("Update Workflow Automation Parameters", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                Else
                    lblInfoUpdateAutomationParametersMessage.Text = "An unexpected error has occurred. please try again later."
                    ClsHelper.Log("Update Workflow Automation Parameters", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
                End If
            End If

            If __EVENTTARGET.Contains("btnChangeAutomationStatus") Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                If (ClsDataAccessHelper.ExecuteNonQuery("Monitoring.UpdateWorkflowAutomationStatus", parameters)) Then
                    RefreshWorkflowOnly = True
                    ClsHelper.Log("Change Workflow Automation Status", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                Else
                    lblInfoUpdateAutomationParametersMessage.Text = "An unexpected error has occurred. please try again later."
                    ClsHelper.Log("Change Workflow Automation Status", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
                End If
            End If

        End If

        If Not IsPostBack Then
            Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
            RadDateTimePickerFrom.SelectedDate = fromDate
            Dim toDate = fromDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)
            RadDateTimePickerTo.SelectedDate = toDate
            If WorkflowID > 0 Then
                If _runWorkflow Then
                    workflow = ExecuteWorkflow(WorkflowID, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), ClsSessionHelper.LogonUser.ID)
                    ProcessIDHiddenField.Value = workflow.ProcessID.ToString()
                    HideTabs(0, 3, 4)
                    ClsHelper.Log("Execute Workflow", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>Workflow ID</b>:{0}</br><b>Process ID</b>:{1}", WorkflowID, workflow.ProcessID), watch.ElapsedMilliseconds, False, Nothing)
                Else
                    workflow = GetWorkflowByID(WorkflowID, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), Nothing, ProcessID)
                    CurrentWorkflowUID.Value = workflow.WorkflowUID.ToString()
                    PauseIntervalsManager.UID = Guid.Parse(CurrentWorkflowUID.Value)
                    PauseIntervalsManager.pauseIntervals = MonitoringManager.GetMonitoringMessagePauseIntervals(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), workflow.WorkflowUID)
                    If ProcessID <> Nothing Then
                        ClsHelper.Log("Check Worfklow Execution Log", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>Workflow ID</b>:{0}</br><b>Process ID</b>:{1}", WorkflowID, ProcessID), watch.ElapsedMilliseconds, False, Nothing)
                    End If
                End If
                CurrentWorkflowID.Value = WorkflowID
                CurrentActivateEditMode.Value = ActivateEditMode
                CurrentExpandSubWorkflows.Value = ExpandSubWorkflows

                If ProcessID <> Nothing Then
                    ProcessIDHiddenField.Value = workflow.ProcessID.ToString()
                End If
                RenderStructure(workflow)
                RenderAutomationParameters(workflow)
                If workflow IsNot Nothing Then
                    If ProcessID = Nothing Then
                        RefreshExecutionLogs(WorkflowID)
                    Else
                        HideTabs(0, 3, 4)
                    End If
                Else
                    HideTabs(0, 1, 2, 3, 4)
                End If
            End If
        Else
            If WorkflowID > 0 Then
                CurrentWorkflowID.Value = WorkflowID
                If "ProcessIDHiddenField".Equals(__EVENTTARGET) And Not String.IsNullOrEmpty(ProcessIDHiddenField.Value) Then
                    Thread.Sleep(1000)
                    workflow = GetWorkflowByID(CurrentWorkflowID.Value, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), Nothing, Guid.Parse(ProcessIDHiddenField.Value))
                    RenderStructure(workflow)
                End If

                If (String.IsNullOrEmpty(ProcessIDHiddenField.Value) Or RefreshWorkflowOnly) And workflow Is Nothing Then
                    Dim IsGridActionOnly = False
                    If __EVENTTARGET IsNot Nothing Then
                        If __EVENTTARGET.Contains("monitoringExecutionLogsGrid") Then
                            IsGridActionOnly = True
                        End If
                    End If
                    If Not IsGridActionOnly Then
                        workflow = GetWorkflowByID(CurrentWorkflowID.Value, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString())
                        CurrentWorkflowUID.Value = workflow.WorkflowUID.ToString()
                        PauseIntervalsManager.UID = Guid.Parse(CurrentWorkflowUID.Value)
                        PauseIntervalsManager.pauseIntervals = MonitoringManager.GetMonitoringMessagePauseIntervals(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), workflow.WorkflowUID)
                        RenderStructure(workflow)
                        RenderAutomationParameters(workflow)
                        txtboxWorkflowName.Text = workflow.Name
                        If Not RefreshWorkflowOnly Then
                            If __EVENTTARGET.Contains("ddlWorkflows") Or __EVENTTARGET.Contains("ddlEnvironment") Or __EVENTTARGET.Contains("RefreshHiddenField") Then
                                RefreshExecutionLogs(WorkflowID)
                            Else
                                RefreshExecutionLogs(WorkflowID, False)
                            End If
                        End If
                    End If
                End If
            End If
        End If

        If workflow IsNot Nothing Then
            If workflow.Passed Is Nothing And Not String.IsNullOrEmpty(ProcessIDHiddenField.Value) Then
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "RefreshInfo", "RefreshInfo();", True)
            End If
            CurrentEnvironmentID.Value = workflow.EnvironmentID
        End If

        If Not String.IsNullOrEmpty(CurrentWorkflowUID.Value) Then
            PauseIntervalsManager.UID = Guid.Parse(CurrentWorkflowUID.Value)
        End If

    End Sub

    Public Property monitoringExecutionLogsGridDataSource As DataTable
        Get
            If WorkflowID = 0 Then
                If Not String.IsNullOrEmpty(CurrentWorkflowID.Value) Then
                    WorkflowID = CInt(CurrentWorkflowID.Value)
                End If
            End If
            If Session("monitoringExecutionLogsGridDataSource_" & WorkflowID) Is Nothing Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
                parameters.Add(New SqlParameter("@StartDate", RadDateTimePickerFrom.SelectedDate))
                parameters.Add(New SqlParameter("@EndDate", RadDateTimePickerTo.SelectedDate))
                Session("monitoringExecutionLogsGridDataSource_" & WorkflowID) = ClsDataAccessHelper.FillDataTable("[Monitoring].[GetWrokflowExecutionLogs]", parameters)
            End If
            Return CType(Session("monitoringExecutionLogsGridDataSource_" & WorkflowID), DataTable)
        End Get
        Set(value As DataTable)
            If WorkflowID = 0 Then
                If Not String.IsNullOrEmpty(CurrentWorkflowID.Value) Then
                    WorkflowID = CInt(CurrentWorkflowID.Value)
                End If
            End If
            Session("monitoringExecutionLogsGridDataSource_" & WorkflowID) = value
        End Set
    End Property

    Protected Sub monitoringExecutionLogsGrid_ItemDataBound(sender As Object, e As GridItemEventArgs)
        Try
            If TypeOf e.Item Is GridDataItem Then
                Dim item As GridDataItem = e.Item
                If Not item.DataItem.row.Item("Passed") Is DBNull.Value Then
                    If item.DataItem.row.Item("Passed") = False Then
                        RadToolTipManager1.TargetControls.Add("StatusImgTooltip_" + item.DataItem.row.Item("ProcessID").ToString(), True)
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle ItemDataBound exception
        End Try
    End Sub

    Private Sub HideTabs(ParamArray tabIndex() As Integer)
        For Each index As Integer In tabIndex
            RadTabStrip1.Tabs(index).Visible = False
        Next
    End Sub

    Private Sub RenderStructure(workflow As MonitoringWorkflow)
        Dim workflowStructureInfo As ClsHelper.MonitoringWorkflowInfo = New ClsHelper.MonitoringWorkflowInfo()
        workflowTreeDiv.InnerHtml = ""
        TreeViewStructure.Nodes.Clear()
        If workflow IsNot Nothing Then
            If workflow.actions IsNot Nothing Then
                Dim root As RadTreeNode = New RadTreeNode
                workflowTreeDiv.InnerHtml = FillWorkflowStructureInfo(workflow.GetTree(Nothing), root, workflow.ID, True)
                TreeViewStructure.Nodes.Add(root)
            End If
            txtboxWorkflowName.Text = workflow.Name
            lblWorkflowIDValue.Text = workflow.ID
            lblCreatedOnValue.Text = workflow.CreatedOn.ToString("dd/MM/yyyy HH:mm")
            lblCreatedByValue.Text = workflow.CreatedBy
            If workflow.LastModifiedOn IsNot Nothing Then
                lblLastModifiedByValue.Text = workflow.LastModifiedBy
                lblLastModifiedOnValue.Text = workflow.LastModifiedOnFormatted
            Else
                trLastModifiedOn.Visible = False
                trLastModifiedBy.Visible = False
            End If
            WorkflowDetailsContainer.Visible = True
        Else
            WorkflowDetailsContainer.Visible = False
        End If
    End Sub

    Private Sub RenderAutomationParameters(workflow As MonitoringWorkflow)
        If workflow IsNot Nothing Then
            ImgMonitoringStatus.ImageUrl = "~/Images/" & IIf(workflow.MonitoringEnabled, "active.png", "inactive.png")
            lblCurrentStatus.Text = IIf(workflow.MonitoringEnabled, "Enabled", "Disabled")
            lblCurrentStatus.ForeColor = IIf(workflow.MonitoringEnabled, Color.Green, Color.Red)
            btnChangeAutomationStatus.Text = IIf(workflow.MonitoringEnabled, "Disable Automation", "Enable Automation")
            btnChangeAutomationStatus.Attributes.Add("class", IIf(workflow.MonitoringEnabled, "btn red", "btn green"))
            workflowAutomationDetailsTable.Visible = True
            txtIntervalInSeconds.Value = workflow.MonitoringIntervalInSeconds
            ChkBoxActivateAlerts.Checked = workflow.ActivateAlerts
            txtBoxAlert.Text = workflow.SendAlertsEmailTo
            ChkBoxActivateWarningNotifications.Checked = workflow.ActivateWarningNotifications
            txtBoxWarningNotificationsTo.Text = workflow.SendWarningNotificationsEmailTo
            ChkBoxSendDailyReport.Checked = workflow.SendDailyReport
            txtSendReportHour.Value = workflow.DailyReportHour
            txtSendReportMinute.Value = workflow.DailyReportMinute
            txtBoxDailyReportEmailTo.Text = workflow.DailyReportEmailTo
        Else
            workflowAutomationDetailsTable.Visible = False
        End If
    End Sub

    Protected Sub ChkBoxSendDailyReport_CheckedChanged(sender As Object, e As EventArgs)
        txtSendReportHour.Enabled = ChkBoxSendDailyReport.Checked
        txtSendReportMinute.Enabled = ChkBoxSendDailyReport.Checked
        txtBoxDailyReportEmailTo.Enabled = ChkBoxSendDailyReport.Checked
    End Sub

    Private Sub RefreshExecutionLogs(workflowID As Integer, Optional refreshDataSource As Boolean = True)
        If refreshDataSource Then
            monitoringExecutionLogsGridDataSource = Nothing
        End If
        monitoringExecutionLogsGrid.DataSource = monitoringExecutionLogsGridDataSource
        monitoringExecutionLogsGrid.DataBind()
    End Sub

    Private Function FillWorkflowStructureInfo(enumerable As IEnumerable(Of TreeItem(Of MonitoringWorkflowAction)), ByRef root As RadTreeNode, ByVal workflowID As Integer, Optional IsRoot As Boolean = False, Optional ReadOnlyActions As Boolean = False) As String
        Dim monitoringWorkflowInfo As ClsHelper.MonitoringWorkflowInfo = New ClsHelper.MonitoringWorkflowInfo()
        Dim htmlContent As String = "<ul>"
        Dim Processing As Boolean = False
        If Not String.IsNullOrEmpty(ProcessIDHiddenField.Value) Then
            Processing = True
        End If
        For Each c As TreeItem(Of MonitoringWorkflowAction) In enumerable
            Dim imageName As String = GetImageNameByType(c.Item.monitoringActionType)
            Dim childTreeNode As RadTreeNode = IIf(IsRoot, root, New RadTreeNode())
            childTreeNode.Text = c.Item.Comments & " " & GetStatusByWorkflowAction(c.Item, Processing, True) & IIf(ActivateEditMode And Not ReadOnlyActions, IIf(IsRoot Or c.Item.monitoringActionType <> MonitoringActionType.Workflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, c.Item.ID), "") & IIf(Not IsRoot, String.Format(DELETE_ACTION_STRING, c.Item.ID), ""), "")
            childTreeNode.Value = c.Item.ID
            childTreeNode.ImageUrl = "~/Images/MonitoringWorkflow/" & imageName
            childTreeNode.Expanded = True

            htmlContent += "<li>"
            htmlContent += String.Format("<table {2}>{4}<tr><td><img src=""Images/MonitoringWorkflow/{0}"" width=""28"" height=""28"" /></td><td>{5}<b>{1}</b>&nbsp;{3}</td></tr></table>", imageName,
                                                                                                                                                                                        IIf(c.Item.monitoringActionType = MonitoringActionType.ApplicationPool, c.Item.InputParameter + " on " + c.Item.ServerName, c.Item.Comments),
                                                                                                                                                                                        IIf(Not Processing, "", "class='" & c.Item.ProcessingClassName & "'"),
                                                                                                                                                                                        GetStatusByWorkflowAction(c.Item, Processing),
                                                                                                                                                                                        IIf(ActivateEditMode And Not ReadOnlyActions, "<tr><td colspan='2' align='right'>" & IIf(IsRoot Or c.Item.monitoringActionType <> MonitoringActionType.Workflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, c.Item.ID), "") & IIf(Not IsRoot, String.Format(DELETE_ACTION_STRING, c.Item.ID), "") & "</td></tr>", ""),
                                                                                                                                                                                        IIf(String.IsNullOrEmpty(c.Item.CustomImageUrl), "", "<img src=""" & c.Item.CustomImageUrl & """ height=""15"" />&nbsp;"))

            If c.Item.GroupChildren = False Or c.Item.GroupChildren Is Nothing Then
                If c.Children.Count > 0 Then
                    If c.Item.monitoringActionType <> MonitoringActionType.Workflow Or Processing Or IsRoot Or ExpandSubWorkflows Then
                        htmlContent += FillWorkflowStructureInfo(c.Children, childTreeNode, workflowID, False, Not IsRoot And c.Item.monitoringActionType = MonitoringActionType.Workflow Or ReadOnlyActions)
                    End If
                End If
            Else
                Dim subContent As String = "<ul>"
                Dim AleradyAddedMonitoringWorkflowAction As List(Of MonitoringWorkflowAction) = New List(Of MonitoringWorkflowAction)()
                Dim IsParentWorkflow As Boolean = c.Item.monitoringActionType = MonitoringActionType.Workflow
                If (IsParentWorkflow And (ExpandSubWorkflows Or IsRoot)) Or Not IsParentWorkflow Or Processing Then
                    For Each parentChild As TreeItem(Of MonitoringWorkflowAction) In c.Children.OrderBy(Function(obj) obj.Item.LogDisplayPriority)
                        Dim comparableStatus As String = parentChild.Item.ProcessingClassName
                        If parentChild.Item.ProcessingClassName = "inprogress" Then
                            comparableStatus = "success"
                        End If
                        Dim ParentID As Integer = workflowID + SECURE_WORKFLOW_PARENT_VALUE
                        If Not parentChild.Item.ParentID Is Nothing Then
                            ParentID = parentChild.Item.ParentID
                        End If
                        Dim ReadOnlySubActions = IsParentWorkflow And Not ParentID = workflowID + SECURE_WORKFLOW_PARENT_VALUE Or ReadOnlyActions
                        If AleradyAddedMonitoringWorkflowAction.Where(Function(f) f.ServerName = parentChild.Item.ServerName And f.monitoringActionType = parentChild.Item.monitoringActionType And f.ComparableStatus = comparableStatus).ToList().Count = 0 Then
                            Dim childNode As RadTreeNode = New RadTreeNode()
                            If c.Children.Where(Function(fn) fn.Item.ServerName = parentChild.Item.ServerName AndAlso fn.Item.monitoringActionType = parentChild.Item.monitoringActionType And IIf(fn.Item.ProcessingClassName = "inprogress", "success", fn.Item.ProcessingClassName) = comparableStatus).ToList().Count > 1 Then
                                Dim passed As Boolean? = Nothing
                                Dim allPassed As Boolean = False
                                childNode.Value = parentChild.Item.ID.ToString() + "root"

                                Dim childrenContent As String = ""
                                For Each child As TreeItem(Of MonitoringWorkflowAction) In c.Children.Where(Function(fn) fn.Item.ServerName = parentChild.Item.ServerName AndAlso fn.Item.monitoringActionType = parentChild.Item.monitoringActionType And IIf(fn.Item.ProcessingClassName = "inprogress", "success", fn.Item.ProcessingClassName) = comparableStatus).OrderBy(Function(obj) obj.Item.LogDisplayPriority).ToList()
                                    childrenContent += String.Format("</br><span class='tree-span'>{0}<span><b>{1}</b>&nbsp;{2}{3}</span></span>", IIf(String.IsNullOrEmpty(child.Item.CustomImageUrl), "", "<img src=""" & child.Item.CustomImageUrl & """ height=""15"" />&nbsp;"), IIf(child.Item.monitoringActionType = MonitoringActionType.ApplicationPool, child.Item.InputParameter, child.Item.Comments), GetStatusByWorkflowAction(child.Item, Processing), IIf(ActivateEditMode And Not ReadOnlySubActions, IIf(IsRoot Or IsParentWorkflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, child.Item.ID) & "|", "") & String.Format(DELETE_ACTION_STRING, child.Item.ID), ""))
                                    Dim newChild As RadTreeNode = New RadTreeNode(IIf(child.Item.monitoringActionType = MonitoringActionType.ApplicationPool, child.Item.InputParameter, child.Item.Comments) & "&nbsp;" & GetStatusByWorkflowAction(child.Item, Processing, True) & IIf(ActivateEditMode And Not ReadOnlySubActions, "&nbsp;" & IIf(IsRoot Or IsParentWorkflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, child.Item.ID) & "|", "") & String.Format(DELETE_ACTION_STRING, child.Item.ID), ""), child.Item.ID)
                                    newChild.ImageUrl = IIf(String.IsNullOrEmpty(child.Item.CustomImageUrl), "~/Images/MonitoringWorkflow/" & GetImageNameByType(child.Item.monitoringActionType), "~/" & child.Item.CustomImageUrl)

                                    If child.Item.Passed = False Then
                                        passed = False
                                    End If

                                    allPassed = allPassed Or child.Item.Passed IsNot Nothing
                                    childNode.Nodes.Add(newChild)
                                Next


                                If allPassed = True And passed Is Nothing Then
                                    passed = True
                                End If
                                imageName = GetImageNameByType(parentChild.Item.monitoringActionType)
                                Dim nodeText As String = parentChild.Item.ServerName
                                If String.IsNullOrEmpty(nodeText) Then
                                    nodeText = GetTypeFriendlyName(parentChild.Item.monitoringActionType)
                                End If
                                If Processing And comparableStatus <> "success" Then
                                    nodeText += " (in " + parentChild.Item.ProcessingClassName + ")"
                                End If
                                childNode.Text = nodeText & " " & IIf(Processing, GetStatusImageByStatus(passed), "")
                                childNode.ImageUrl = "~/Images/MonitoringWorkflow/" & imageName
                                subContent += String.Format("<li><table {2}><tr><td><img src=""Images/MonitoringWorkflow/{0}"" width=""28"" height=""28"" /></td><td>{1}</td></tr>", imageName,
                                                                                                                                                                            nodeText,
                                                                                                                                                                            IIf(Not Processing, "", "class='" & parentChild.Item.ProcessingClassName & "'"))
                                subContent += "<tr><td colspan='2' align='left'>"
                                subContent += childrenContent
                                subContent += "</td></tr></table></li>"
                            Else
                                imageName = GetImageNameByType(parentChild.Item.monitoringActionType)
                                subContent += "<li>"
                                subContent += String.Format("<table {2}>{4}<tr><td><img src=""Images/MonitoringWorkflow/{0}"" width=""28"" height=""28"" /></td><td>{5}<b>{1}</b>&nbsp;{3}</td></tr></table>", imageName,
                                                                                                                                                                                                            IIf(parentChild.Item.monitoringActionType = MonitoringActionType.ApplicationPool, parentChild.Item.InputParameter + " on " + parentChild.Item.ServerName, parentChild.Item.Comments),
                                                                                                                                                                                                            IIf(Not Processing, "", "class='" & parentChild.Item.ProcessingClassName & "'"),
                                                                                                                                                                                                            GetStatusByWorkflowAction(parentChild.Item, Processing),
                                                                                                                                                                                                            IIf(ActivateEditMode And Not ReadOnlySubActions, "<tr><td colspan='2' align='right'>" & IIf(IsRoot Or IsParentWorkflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, parentChild.Item.ID) & "|", "") & String.Format(DELETE_ACTION_STRING, parentChild.Item.ID) & "</td></tr>", ""),
                                                                                                                                                                                                            IIf(String.IsNullOrEmpty(parentChild.Item.CustomImageUrl), "", "<img src=""" & parentChild.Item.CustomImageUrl & """ height=""15"" />&nbsp;"))
                                subContent += "</li>"
                                childNode.Text = IIf(String.IsNullOrEmpty(parentChild.Item.CustomImageUrl), "", "<img src=""" & parentChild.Item.CustomImageUrl & """ height=""15"" />&nbsp;") & IIf(parentChild.Item.monitoringActionType = MonitoringActionType.ApplicationPool, parentChild.Item.InputParameter + " on " + parentChild.Item.ServerName, parentChild.Item.Comments) & " &nbsp;" & GetStatusByWorkflowAction(parentChild.Item, Processing, True) & IIf(ActivateEditMode And Not ReadOnlySubActions, IIf(IsRoot Or IsParentWorkflow, "&nbsp;" & String.Format(ADD_SUB_ACTION_STRING, parentChild.Item.ID) & "|", "") & String.Format(DELETE_ACTION_STRING, parentChild.Item.ID), "")
                                childNode.Value = parentChild.Item.ID
                                childNode.ImageUrl = "~/Images/MonitoringWorkflow/" & imageName
                                childNode.Expanded = True
                            End If

                            Dim monitoringWorkflowAction As MonitoringWorkflowAction = New MonitoringWorkflowAction()

                            monitoringWorkflowAction.ServerName = parentChild.Item.ServerName
                            monitoringWorkflowAction.monitoringActionType = parentChild.Item.monitoringActionType
                            If parentChild.Item.ProcessingClassName = "inprogress" Then
                                monitoringWorkflowAction.ComparableStatus = "success"
                            Else
                                monitoringWorkflowAction.ComparableStatus = parentChild.Item.ProcessingClassName
                            End If

                            AleradyAddedMonitoringWorkflowAction.Add(monitoringWorkflowAction)
                            childTreeNode.Nodes.Add(childNode)

                        End If
                    Next
                    subContent += "</ul>"
                    If c.Children.Count > 0 Then
                        htmlContent += subContent
                    End If
                End If
            End If
            htmlContent += "</li>"
            If Not IsRoot Then
                root.Nodes.Add(childTreeNode)
            End If
        Next
        htmlContent += "</ul>"
        Return htmlContent
    End Function

    Private Function GetStatusImageByStatus(passed As Boolean?) As String
        Select Case passed
            Case True
                Return " Success <img src=""Images/Success.png"" height='15' />"
            Case False
                Return " Error occurred while execution a child action <img src=""Images/Error.png"" height='15' />"
            Case Else
                Return " Processing ..."
        End Select
    End Function

    Private Function GetStatusByWorkflowAction(workflowAction As MonitoringWorkflowAction, Processing As Boolean, Optional treeDisplay As Boolean = False) As String
        If Processing Then
            If workflowAction.Passed Is Nothing Then
                Return "&nbsp;" & workflowAction.OverallStatus
            Else
                If workflowAction.Passed = True Then
                    If (workflowAction.monitoringActionType = MonitoringActionType.SQLQuery) Then
                        If treeDisplay Then
                            Return String.Format("<img src=""Images/Success.png"" height='15' title=""{0}"" />", WebUtility.HtmlDecode(Regex.Replace(workflowAction.OutputValue.Replace("</br>", vbNewLine), "<(.|\n)*?>", "")))
                        Else
                            Return "<img src=""Images/Success.png"" height='15' />" & "&nbsp;" & workflowAction.OutputValue
                        End If

                    Else
                        Return workflowAction.OutputValue & "&nbsp;" & "<img src=""Images/Success.png"" height='15' />"
                    End If
                Else
                    Return workflowAction.OutputValue & "&nbsp;" & String.Format("<img src=""Images/Error.png"" height='15' title=""{0}"" />", workflowAction.ErrorMessage)
                End If
            End If
        End If
        Return ""
    End Function

    Private Function GetImageNameByType(type As MonitoringActionType) As String
        Return GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType = type).Single().DefaultImageName
    End Function

    Private Function GetTypeFriendlyName(type As MonitoringActionType) As String
        Return GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType = type).Single().Name
    End Function

    Protected Function GetMonitoringActionTypes() As List(Of MonitoringActionTypeInfo)
        Dim list As List(Of MonitoringActionTypeInfo)
        If Cache.Get("MonitoringActionTypes") Is Nothing Then
            list = ClsHelper.GetMonitoringActionTypes()
            Cache.Insert("MonitoringActionTypes", list)
        Else
            list = DirectCast(Cache.Get("MonitoringActionTypes"), List(Of MonitoringActionTypeInfo))
        End If
        Return list
    End Function
    Protected Sub monitoringExecutionLogsGrid_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        monitoringExecutionLogsGrid.DataSource = monitoringExecutionLogsGridDataSource
    End Sub
    Protected Sub RadTabStrip1_TabClick(sender As Object, e As RadTabStripEventArgs)
        If e.Tab.Index = 4 Then
            If WorkflowID = 0 Then
                If Not String.IsNullOrEmpty(CurrentWorkflowID.Value) Then
                    WorkflowID = CInt(CurrentWorkflowID.Value)
                End If
            End If
            RefreshExecutionLogs(WorkflowID)
        End If
    End Sub
End Class
