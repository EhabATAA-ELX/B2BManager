
Imports System.Data
Imports System.Diagnostics
Imports SAPRequestsLib.Models
Imports Telerik.Web.UI

Partial Public Class MonitoringWorkflowManagerWebForm
    Inherits System.Web.UI.Page


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=MonitoringWorkflowManager.aspx", True)
            Return
        End If

        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            RenderDropDownLists()
            ClsHelper.Log("Check Monitroing Workflows", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("chkBoxEditMode") Then
                    MonitoringWorkflowUC.ActivateEditMode = chkBoxEditMode.Checked
                End If

                If __EVENTTARGET.Contains("chkBoxExpandSubWorkflow") Then
                    MonitoringWorkflowUC.ExpandSubWorkflows = chkBoxExpandSubWorkflow.Checked
                End If

                If __EVENTTARGET.Contains("BtnSaveWorkflow") Then
                    SaveWorkflow()
                End If

                If __EVENTTARGET.Contains("BtnSaveWorkflowForLater") Then
                    SaveWorkflowForLater()
                End If

                If __EVENTTARGET.Contains("ddlEnvironment") Then
                    BindWorkflows()
                    If Not String.IsNullOrEmpty(ddlWorkflows.SelectedValue) Then
                        SelectWorkflow(ddlWorkflows.SelectedValue)
                    End If
                End If

                If __EVENTTARGET.Contains("BtnConfirmDeleteWorkflow") Then
                    Dim watch As Stopwatch = Stopwatch.StartNew()
                    Dim workflowID As Integer = ddlWorkflows.SelectedValue
                    If DeleteWorkflow() Then
                        BindWorkflows()
                        If Not String.IsNullOrEmpty(ddlWorkflows.SelectedValue) Then
                            SelectWorkflow(ddlWorkflows.SelectedValue)
                        End If
                        ClsHelper.Log("Delete Workflow", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>Workflow ID</b>:{0}", workflowID), watch.ElapsedMilliseconds, False, Nothing)
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "ShowOrCloseWindow('DeleteWorkflow',false);", True)
                    Else
                        lblDeleteWorkflowErrorMessage.Text = "An unexpected error has occurred. please try again later."
                        ClsHelper.Log("Delete Workflow", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
                    End If
                End If

                If __EVENTTARGET.Contains("ddlWorkflows") Or __EVENTTARGET.Contains("RefreshHiddenField") Then
                    If "BindWorkflows".Equals(__EVENTARGUMENT) Then
                        BindWorkflows(ddlWorkflows.SelectedValue)
                    End If
                    If Not String.IsNullOrEmpty(ddlWorkflows.SelectedValue) Then
                        SelectWorkflow(ddlWorkflows.SelectedValue)
                    End If
                End If
            End If
        Else
            If Not String.IsNullOrEmpty(ddlWorkflows.SelectedValue) Then
                SelectWorkflow(ddlWorkflows.SelectedValue)
            End If
        End If
    End Sub

    Private Sub RenderDropDownLists()
        Dim enivronments As List(Of ClsHelper.BasicModel) = GetMonitoringEnvironments()
        ddlEnvironment.DataSource = enivronments
        If enivronments.Count > 0 Then
            ddlEnvironment.DataBind()
            ddlEnvironment.SelectedValue = enivronments(0).ID.ToString()
            BindWorkflows()
        End If
    End Sub

    Protected Function GetMonitoringEnvironments() As List(Of ClsHelper.BasicModel)
        Dim list As List(Of ClsHelper.BasicModel)
        If Cache.Get("MonitoringEnvironments") Is Nothing Then
            list = ClsHelper.GetMonitoringEnvironments()
            Cache.Insert("MonitoringEnvironments", list)
        Else
            list = DirectCast(Cache.Get("MonitoringEnvironments"), List(Of ClsHelper.BasicModel))
        End If
        Return list
    End Function

    Public Sub BindWorkflows(Optional selectedValue As Integer = 0)
        ddlWorkflows.Items.Clear()
        Dim workflows As List(Of MonitoringWorkflow) = SAPRequestsLib.MonitoringWorkflowManager.GetWorkflowsByEnivronmentID(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString,
                                                                             ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(),
                                                                             ConfigurationManager.AppSettings("ErrorEmailTo").ToString(),
                                                                             ConfigurationManager.AppSettings("SMTPServer").ToString(),
                                                                             ddlEnvironment.SelectedValue)
        Dim selectedValueExists As Boolean = False
        If workflows.Count > 0 Then
            For Each workflow As MonitoringWorkflow In workflows
                Dim item As RadComboBoxItem = New RadComboBoxItem(workflow.Name, workflow.ID)
                item.ImageUrl = "Images/MonitoringWorkflow/workflow.png"
                ddlWorkflows.Items.Add(item)
                If workflow.ID = selectedValue Then
                    selectedValueExists = True
                End If
            Next
            ddlWorkflows.DataBind()
            If selectedValueExists Then
                ddlWorkflows.SelectedValue = selectedValue
            Else
                ddlWorkflows.SelectedValue = workflows(0).ID
            End If

        Else
            lblNoWorkflows.Visible = True
            fieldsetWorkflowSelection.Visible = False
            MonitoringWorkflowUC.Visible = False
        End If

    End Sub


    Private Sub SelectWorkflow(workflowID As Integer)
        MonitoringWorkflowUC.WorkflowID = workflowID
        lblWorkflowToDelete.Text = ddlWorkflows.SelectedItem.Text
    End Sub

    Private Function AddWorkflow() As Integer
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=MonitoringWorkflowManager.aspx", True)
            Return 0
        Else
            Return ClsHelper.AddMonitoringWorkflow(ClsSessionHelper.LogonUser.ID, ddlEnvironment.SelectedValue, txtBoxWorkflowName.Text)
        End If
    End Function

    Private Function DeleteWorkflow() As Boolean
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=MonitoringWorkflowManager.aspx", True)
            Return False
        Else
            Return ClsHelper.DeleteMonitoringWorkflow(ClsSessionHelper.LogonUser.ID, ddlWorkflows.SelectedValue)
        End If
    End Function
    Protected Sub SaveWorkflow()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim workflowID As Integer = AddWorkflow()
        If workflowID > 0 Then
            BindWorkflows(workflowID)
            SelectWorkflow(ddlWorkflows.SelectedValue)
            ClsHelper.Log("Save Workflow", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>Workflow ID</b>:{0}", workflowID), watch.ElapsedMilliseconds, False, Nothing)
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Add dialog", "ShowOrCloseWindow('NewWorkflow',false);", True)
        Else
            ErrorInfo.Text = "An unexpected error has occurred. please try again later."
            ClsHelper.Log("Save Workflow", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
        End If

    End Sub
    Protected Sub SaveWorkflowForLater()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim workflowID As Integer = AddWorkflow()
        If workflowID > 0 Then
            Dim currentValue As Integer = 0
            If Not String.IsNullOrEmpty(ddlWorkflows.SelectedValue) Then
                currentValue = ddlWorkflows.SelectedValue
            End If
            BindWorkflows(currentValue)
            SelectWorkflow(ddlWorkflows.SelectedValue)
            ClsHelper.Log("Save Workflow for later", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>Workflow ID</b>:{0}", workflowID), watch.ElapsedMilliseconds, False, Nothing)
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Add dialog", "ShowOrCloseWindow('NewWorkflow',false);", True)
        Else
            ErrorInfo.Text = "An unexpected error has occurred. please try again later."
            ClsHelper.Log("Save Workflow for later", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
        End If
    End Sub
End Class
