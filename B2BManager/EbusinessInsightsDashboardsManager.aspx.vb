
Imports System.Data.SqlClient

Partial Class EbusinessInsightsDashboardsManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Load

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser

        If IsPostBack Then
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("SelectedDashboardIDToDelete") Then
                    If "SubmitDeleteDashboard".Equals(__EVENTARGUMENT) Then
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@DashboardID", CInt(SelectedDashboardIDToDelete.Value)))
                        parameters.Add(New SqlParameter("@UserGlobalID", logonUser.GlobalID))
                        If (ClsDataAccessHelper.ExecuteNonQuery("Insights.DeleteDashboard", parameters)) Then
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "ShowOrCloseWindow('DeleteDashboard',false);", True)
                        Else
                            lblDeleteActionErrorMessage.Text = "An unexpected error has occurred. please try again later."
                        End If
                    Else
                        SelectedDashboardIDToDelete.Value = __EVENTARGUMENT
                    End If

                End If
            End If
        End If
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Manage/Open Dashboard"
            btnNewChartPlaceHolder.Visible = ClsInsightsHelper.CanUserAddChart(logonUser)
        End If

        dashboardsGrid.DataSource = ClsInsightsHelper.GetDashboards(logonUser.GlobalID)
        dashboardsGrid.DataBind()


    End Sub

End Class
