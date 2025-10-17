Imports System.Diagnostics

Partial Class MonitoringActionsManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Load

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        End If

        Dim watch As Stopwatch = Stopwatch.StartNew()
        If Not IsPostBack Then
            Form.DefaultButton = CType(MonitoringActionsGrid.FindControl("btnSearch"), LinkButton).UniqueID
        End If
        ClsHelper.Log("Check Monitroing Actions", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
    End Sub

End Class
