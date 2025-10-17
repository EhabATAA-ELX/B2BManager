
Partial Class MonitoringWorkflowProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        Dim WorkflowID As Integer = 0
        Dim RunWorkflow As Boolean = False
        Dim ProcessID As Guid = Nothing
        Boolean.TryParse(Request.QueryString("RunWorkflow"), RunWorkflow)
        If Not String.IsNullOrEmpty(Request.QueryString("ProcessID")) Then
            Guid.TryParse(Request.QueryString("ProcessID"), ProcessID)
        End If

        Integer.TryParse(Request.QueryString("WorkflowID"), WorkflowID)

        MonitoringWorkflow.RunWorkflow = RunWorkflow
        MonitoringWorkflow.WorkflowID = WorkflowID
        MonitoringWorkflow.ProcessID = ProcessID

        If RunWorkflow Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Workflow Manual Execution"
        Else
            CType(Master.FindControl("title"), HtmlTitle).Text = "Monitoring Workflow"
        End If
    End Sub


End Class
