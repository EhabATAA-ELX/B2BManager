
Partial Class MonitoringSelectAction
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Form.DefaultButton = CType(MonitoringActionsGrid.FindControl("btnSearch"), LinkButton).UniqueID
            Dim EnvironmentID As Integer = 0
            Dim WorkflowID As Integer = 0
            Integer.TryParse(Request.QueryString("EnvironmentID"), EnvironmentID)
            Integer.TryParse(Request.QueryString("WorkflowID"), WorkflowID)

            MonitoringActionsGrid.EnvironmentID = EnvironmentID
            MonitoringActionsGrid.LinkedWorkflowID = WorkflowID
            CType(Master.FindControl("title"), HtmlTitle).Text = "Workflow Select Action"
        End If
    End Sub
End Class
