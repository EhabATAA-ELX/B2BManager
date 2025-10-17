
Imports SAPRequestsLib.Models
Imports Telerik.Web.UI
Imports SAPRequestsLib.MonitoringWorkflowManager
Imports System.Data.SqlClient

Partial Class MonitoringActionProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim ActionID As Integer = 0
            Integer.TryParse(Request.QueryString("ActionID"), ActionID)

            If ActionID > 0 Then
                CType(Master.FindControl("title"), HtmlTitle).Text = "Display/edit Action " & ActionID.ToString()
                btnSubmit.Text = "Update"
                btnSubmit.OnClientClick = "ProcessButton('Update')"
            Else
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Action"
                btnSubmit.Text = "Save"
                btnSubmit.OnClientClick = "ProcessButton('Add')"
                trCustomImage.Visible = False
            End If

            RenderEnvironments()
            RenderActionTypes()
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim ActionID As Integer = 0
        Integer.TryParse(Request.QueryString("ActionID"), ActionID)

        If Not IsPostBack Then
            If ActionID > 0 Then
                Dim action As MonitoringWorkflowAction = GetActionByID(ActionID, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString())
                ddlEnvironment.SelectedValue = action.EnvironmentID
                ddlEnvironment.Enabled = False
                ddlType.SelectedValue = CInt(action.monitoringActionType)
                txtboxComments.Text = action.Comments
                txtboxInputParameter.Text = action.InputParameter
                txtboxServerName.Text = action.ServerName
                If GetRequiresConfigurationRatioByType(action.monitoringActionType) Then
                    RadNumericTextBoxAlertRatio.Value = action.AlertRatio
                    RadNumericTextBoxWarningRatio.Value = action.WarningRatio
                End If
                If Not String.IsNullOrEmpty(action.CustomImageUrl) Then
                    customImage.ImageUrl = action.CustomImageUrl
                Else
                    trCustomImage.Visible = False
                End If

            End If
        End If
        lblErrorInfo.Text = " "
        ActionExecutionInfo.VisibleOnPageLoad = False
        ManageControlsDisplayByActionType(CInt(ddlType.SelectedValue))
    End Sub

    Private Sub ManageControlsDisplayByActionType(type As MonitoringActionType)
        Select Case type
            Case MonitoringActionType.ApplicationPool
                ApplyControlChanges("Application Pool Name (*)")
                ChangeRatioParametersDisplay(False)
                Return
            Case MonitoringActionType.PingURL
                ApplyControlChanges("Address URL (*)", "Title (not required)", "NotRequired")
                ChangeRatioParametersDisplay(False)
                Return
            Case MonitoringActionType.WindowService
                ApplyControlChanges("Windows Service Name (*)")
                ChangeRatioParametersDisplay(False)
                Return
            Case MonitoringActionType.SQLQuery
                ApplyControlChanges("Query Text (*)", "Title (not required)", "NotRequired", TextBoxMode.MultiLine, 60, True)
                ChangeRatioParametersDisplay(True)
                Return
        End Select
    End Sub

    Private Sub ChangeRatioParametersDisplay(Shown As Boolean)
        trAlertRatio.Visible = Shown
        trWarningRatio.Visible = Shown
    End Sub

    Private Sub ApplyControlChanges(inputParameterLabel As String, Optional serverNameLabel As String = "Server Name (*)", Optional serverNameValidationGroup As String = "ActionSubmit", Optional inputParameterTextMode As TextBoxMode = TextBoxMode.SingleLine, Optional inputParameterHeight As Integer = 25, Optional showQueryInfo As Boolean = False)
        lblInputParameter.Text = inputParameterLabel
        lblServerName.Text = serverNameLabel
        requiredFieldValidatorServerName.ValidationGroup = serverNameValidationGroup
        txtboxInputParameter.TextMode = inputParameterTextMode
        txtboxInputParameter.Height = inputParameterHeight
        IsServerNameRequired.Value = IIf(serverNameValidationGroup.Equals("ActionSubmit"), "true", "false")
        trInfoQuery.Visible = showQueryInfo
    End Sub


    Private Sub RenderEnvironments()
        Dim enivronments As List(Of ClsHelper.BasicModel) = GetMonitoringEnvironments()
        ddlEnvironment.DataSource = enivronments
        If enivronments.Count > 0 Then
            ddlEnvironment.DataBind()
            ddlEnvironment.SelectedValue = IIf(enivronments.Count > 1, 0, enivronments(0).ID.ToString())
        End If
    End Sub

    Private Function GetRequiresConfigurationRatioByType(type As MonitoringActionType) As String
        Return GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType = type).Single().RequiresConfigurationRatio
    End Function

    Private Function GetImageNameByType(type As MonitoringActionType) As String
        Return GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType = type).Single().DefaultImageName
    End Function

    Private Function GetTypeFriendlyName(type As MonitoringActionType) As String
        Return GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType = type).Single().Name
    End Function

    Private Sub RenderActionTypes()
        Dim availableTypes As List(Of MonitoringActionTypeInfo) = GetMonitoringActionTypes().Where(Function(fn) fn.monitoringActionType <> MonitoringActionType.Workflow And
                                                                                                             fn.monitoringActionType <> MonitoringActionType.CustomAction).ToList()

        If availableTypes.Count > 0 Then
            For Each actionType As MonitoringActionTypeInfo In availableTypes
                Dim item As RadComboBoxItem = New RadComboBoxItem(actionType.Name, actionType.ActionTypeID)
                item.ImageUrl = "~/Images/MonitoringWorkflow/" + actionType.DefaultImageName
                ddlType.Items.Add(item)
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
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=MonitoringWorkflowManager.aspx", True)
            Return
        Else
            Dim ActionID As Integer = 0
            Integer.TryParse(Request.QueryString("ActionID"), ActionID)
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
            If ActionID = 0 Then
                parameters.Add(New SqlParameter("@EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
            End If
            parameters.Add(New SqlParameter("@ActionID", ActionID))
            parameters.Add(New SqlParameter("@ActionTypeID", CInt(ddlType.SelectedValue)))
            parameters.Add(New SqlParameter("@Comments", txtboxComments.Text))
            parameters.Add(New SqlParameter("@ServerName", txtboxServerName.Text))
            parameters.Add(New SqlParameter("@InputParameter", txtboxInputParameter.Text))
            If CInt(ddlType.SelectedValue) = MonitoringActionType.SQLQuery Then
                parameters.Add(New SqlParameter("@WarningRatio", RadNumericTextBoxWarningRatio.Value))
                parameters.Add(New SqlParameter("@AlertRatio", RadNumericTextBoxAlertRatio.Value))
            End If
            If ClsHelper.AddMonitoringAction(parameters) > 0 Then
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Finish", "CloseWindowAndRefreshGrid();", True)
            Else
                lblErrorInfo.Text = "An unexpected error has occurred. please try again later."
            End If
        End If
    End Sub
    Protected Sub btnExecute_Click(sender As Object, e As EventArgs)
        Dim exception As Exception = Nothing
        Dim OutputValue As String = Nothing
        Dim ResultRatio As Decimal? = Nothing
        Dim action As MonitoringWorkflowAction = New MonitoringWorkflowAction()
        action.ServerName = txtboxServerName.Text
        action.InputParameter = txtboxInputParameter.Text
        action.monitoringActionType = ddlType.SelectedValue
        Dim currentStatusLabel As String = "Current status:&nbsp;<b>{0}</b>"
        If CInt(ddlType.SelectedValue) = MonitoringActionType.SQLQuery Then
            action.AlertRatio = RadNumericTextBoxAlertRatio.Value
            currentStatusLabel = "{0}"
        End If
        RunActionManually(OutputValue, exception, action, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString, ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString(), ConfigurationManager.AppSettings("SMTPServer").ToString(), ResultRatio)
        ExecutionInfo.InnerHtml = String.Format("<img src=""Images/MonitoringWorkflow/{0}"" width=""20"" height=""20"" />", GetImageNameByType(CInt(ddlType.SelectedValue))) _
            & "&nbsp;" & GetTypeFriendlyName(CInt(ddlType.SelectedValue)) & " action execution was compleded with " _
            & IIf(exception IsNot Nothing, "failure  <img src=""Images/Error.png"" height='15' />", "success  <img src=""Images/Success.png"" height='15' />") & "</br>" _
            & String.Format(currentStatusLabel, OutputValue)
        If exception IsNot Nothing Then
            ExecutionInfo.InnerHtml += "</br>" & exception.Message
        End If
        ExecutionInfo.Attributes.Add("class", IIf(exception Is Nothing, IIf(ResultRatio IsNot Nothing And ResultRatio < RadNumericTextBoxWarningRatio.Value, "warning", "success"), "error"))
        ActionExecutionInfo.VisibleOnPageLoad = True
    End Sub
End Class
