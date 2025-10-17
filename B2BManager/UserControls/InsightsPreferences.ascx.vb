
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI
Partial Class UserControls_InsightsPreferences
    Inherits System.Web.UI.UserControl

    Private _ShowCancelButton As Boolean

    Public Property ShowCancelButton As Boolean
        Get
            Return _ShowCancelButton
        End Get
        Set
            _ShowCancelButton = Value
        End Set
    End Property
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If Not IsPostBack Then
            RenderControls()
        End If
    End Sub
    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        btnCancelPanel.Visible = ShowCancelButton
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.LogonUser.DefaultEnvironmentID.ToString())
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.LogonUser.DefaultSOPIDs, True)
        chkBoxCountrySplit.Checked = ClsSessionHelper.LogonUser.DefaultCountrtySplitStatus
        Dim defaultdashboard As ClsInsightsHelper.Dashobard = ClsInsightsHelper.GetDashboardByID(0, ClsSessionHelper.LogonUser.GlobalID)
        ddlCharts.Items.Clear()
        ddlCharts.Items.Add(New RadComboBoxItem("Not specified", 0))
        If defaultdashboard IsNot Nothing Then
            If defaultdashboard.Areas IsNot Nothing Then
                For Each area As ClsInsightsHelper.Area In defaultdashboard.Areas
                    If area.Charts IsNot Nothing Then
                        For Each chart As ClsInsightsHelper.Chart In area.Charts
                            Dim chartItem As RadComboBoxItem = New RadComboBoxItem(chart.Name, chart.ID)
                            chartItem.ImageUrl = "../" + ClsInsightsHelper.GetImageUrlFromChartType(chart.Type)
                            chartItem.Selected = chart.ID = ClsSessionHelper.LogonUser.HomePageChartID
                            ddlCharts.Items.Add(chartItem)
                        Next
                    End If
                Next
            End If
        End If
        ddlDashboard.Items.Clear()
        ddlDashboard.Items.Add(New ListItem("Not specified", 0))
        Dim dashboardsDT As DataTable = ClsInsightsHelper.GetDashboards(ClsSessionHelper.LogonUser.GlobalID)
        If dashboardsDT IsNot Nothing Then
            For Each row As DataRow In dashboardsDT.Rows
                Dim item As ListItem = New ListItem(row("Name"), row("ID"))
                item.Selected = ClsSessionHelper.LogonUser.DefaultDashboardID = row("ID")
                ddlDashboard.Items.Add(item)
            Next
        End If
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
        parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
        parameters.Add(New SqlParameter("@DefaultDashboardID", ddlDashboard.SelectedValue))
        parameters.Add(New SqlParameter("@HomePageChartID", ddlCharts.SelectedValue))
        parameters.Add(New SqlParameter("@DefaultEnvironmentID", ddlEnvironment.SelectedValue))
        parameters.Add(New SqlParameter("@DefaultSOPIDs", ddlCountry.SelectedValue))
        parameters.Add(New SqlParameter("@DefaultCountrtySplitStatus", chkBoxCountrySplit.Checked))
        If (ClsDataAccessHelper.ExecuteNonQuery("Insights.SavePreferences", parameters)) Then
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
            logonUser.DefaultCountrtySplitStatus = chkBoxCountrySplit.Checked
            logonUser.DefaultDashboardID = ddlDashboard.SelectedValue
            logonUser.DefaultEnvironmentID = ddlEnvironment.SelectedValue
            logonUser.DefaultSOPIDs = ddlCountry.SelectedValue
            logonUser.HomePageChartID = ddlCharts.SelectedValue
            ClsSessionHelper.LogonUser = logonUser
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "window.parent.ShowOrClosePreferencesWindow(false);", True)
            ClsHelper.Log("Insights Update Preferences", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
        Else
            lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
            ClsHelper.Log("Insights Update Preferences", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "Failed to update preferences")
        End If

    End Sub

End Class
