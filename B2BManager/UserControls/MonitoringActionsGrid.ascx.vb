
Imports System.Data
Imports System.Data.SqlClient
Imports SAPRequestsLib.Models
Imports Telerik.Web.UI

Partial Class UserControls_MonitoringActionsGrid
    Inherits System.Web.UI.UserControl

    Private _EnvironmentID As Integer
    Private _LinkedWorkflowID As Integer

    Public Property EnvironmentID As Integer
        Get
            Return _EnvironmentID
        End Get
        Set(value As Integer)
            _EnvironmentID = value
        End Set
    End Property

    Public Property LinkedWorkflowID As Integer
        Get
            Return _LinkedWorkflowID
        End Get
        Set(value As Integer)
            _LinkedWorkflowID = value
        End Set
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            RenderEnvironments()
            RenderActionTypes()
            If EnvironmentID > 0 Then
                monitoringActionsGrid.PageSize = 15
                CurrentEnvironmentID.Value = EnvironmentID
                If LinkedWorkflowID > 0 Then
                    CurrentLinkedWorkflowID.Value = LinkedWorkflowID
                End If
            End If
            RunSearch(True)
        Else
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

            If Not String.IsNullOrEmpty(CurrentEnvironmentID.Value) Then
                EnvironmentID = CInt(CurrentEnvironmentID.Value)
                If Not String.IsNullOrEmpty(CurrentLinkedWorkflowID.Value) Then
                    LinkedWorkflowID = CInt(CurrentLinkedWorkflowID.Value)
                End If
            End If

            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("CurrentEnvironmentID") Then
                    RunSearch(True)
                End If

                If __EVENTTARGET.Contains("SelectedActionIDToDelete") Then
                    If "SubmitDeleteAction".Equals(__EVENTARGUMENT) Then
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@ActionID", CInt(SelectedActionIDToDelete.Value)))
                        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                        If (ClsDataAccessHelper.ExecuteNonQuery("Monitoring.DeleteAction", parameters)) Then
                            RunSearch(True)
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "ShowOrCloseWindow('DeleteAction',false);", True)
                        Else
                            lblDeleteActionErrorMessage.Text = "An unexpected error has occurred. please try again later."
                        End If
                    Else
                        SelectedActionIDToDelete.Value = __EVENTARGUMENT
                    End If

                End If
            End If
        End If

    End Sub

    Public Property actionsGridDataSource As DataTable
        Get
            If Not String.IsNullOrEmpty(ddlEnvironment.SelectedValue) Then
                If Session("actionsGridDataSource_" + EnvironmentID.ToString()) Is Nothing Then
                    Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parameters.Add(New SqlParameter("@EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
                    If Not String.IsNullOrWhiteSpace(txtBoxSearchInDetails.Text) Then
                        parameters.Add(New SqlParameter("@Criteria", txtBoxSearchInDetails.Text))
                    End If
                    parameters.Add(New SqlParameter("@ActionTypes", ddlType.SelectedValue))
                    If LinkedWorkflowID > 0 Then
                        parameters.Add(New SqlParameter("@LinkedWorkflowID", LinkedWorkflowID))
                    End If
                    Session("actionsGridDataSource_" + EnvironmentID.ToString()) = ClsDataAccessHelper.FillDataTable("[Monitoring].[GetMonitoringActions]", parameters)
                End If
            Else
                Session("actionsGridDataSource_" + EnvironmentID.ToString()) = New DataTable()
            End If

            Return CType(Session("actionsGridDataSource_" + EnvironmentID.ToString()), DataTable)
        End Get
        Set(value As DataTable)
            Session("actionsGridDataSource_" + EnvironmentID.ToString()) = value
        End Set
    End Property

    Private Sub RenderEnvironments()
        Dim enivronments As List(Of ClsHelper.BasicModel) = GetMonitoringEnvironments()

        If EnvironmentID = 0 And enivronments.Count > 1 Then
            ddlEnvironment.AppendDataBoundItems = True
            ddlEnvironment.Items.Insert(0, New ListItem("All", "0"))
        End If

        ddlEnvironment.DataSource = enivronments
        If enivronments.Count > 0 Then
            ddlEnvironment.DataBind()
            ddlEnvironment.SelectedValue = IIf(EnvironmentID > 0, EnvironmentID, IIf(enivronments.Count > 1, 0, enivronments(0).ID.ToString()))
        End If

        If EnvironmentID > 0 Then
            ddlEnvironment.Enabled = False
        End If
    End Sub

    Private Sub RenderActionTypes()
        Dim availableTypes As List(Of MonitoringActionTypeInfo) = GetMonitoringActionTypes().Where(Function(fn) EnvironmentID > 0 Or
                                                                                                             (fn.monitoringActionType <> SAPRequestsLib.MonitoringWorkflowManager.MonitoringActionType.Workflow And
                                                                                                             fn.monitoringActionType <> SAPRequestsLib.MonitoringWorkflowManager.MonitoringActionType.CustomAction)).ToList()

        If availableTypes.Count > 0 Then
            ddlType.Items.Add(New RadComboBoxItem("All", ""))
            For Each actionType As MonitoringActionTypeInfo In availableTypes
                Dim item As RadComboBoxItem = New RadComboBoxItem(actionType.Name, actionType.ActionTypeID)
                item.ImageUrl = "~/Images/MonitoringWorkflow/" + actionType.DefaultImageName
                ddlType.Items.Add(item)
                ddlType.Items(0).Value += IIf(String.IsNullOrEmpty(ddlType.Items(0).Value), item.Value, "," & item.Value)
            Next
            ddlType.DataBind()
        End If

    End Sub

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

    Private Sub RunSearch(Optional forceLoad As Boolean = False)
        If forceLoad Then
            actionsGridDataSource = Nothing
        End If
        monitoringActionsGrid.DataSource = actionsGridDataSource
        monitoringActionsGrid.DataBind()
        monitoringActionsGrid.MasterTableView.GetColumn("Select").Visible = EnvironmentID > 0
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        RunSearch(True)
    End Sub
    Protected Sub monitoringActionsGrid_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        monitoringActionsGrid.DataSource = actionsGridDataSource
    End Sub
End Class
