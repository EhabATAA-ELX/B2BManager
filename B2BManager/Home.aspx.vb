
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading

Partial Class Home
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            If Not IsPostBack Then
                RefreshData()
            End If
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            RefreshData()
        End If
    End Sub

    Private Sub RenderDefaultChart(user As ClsUser)
        Dim countrySplit As Boolean = user.DefaultCountrtySplitStatus
        Dim environmentID As Integer = user.DefaultEnvironmentID
        Dim sopIDs As String = user.DefaultSOPIDs
        Dim chart As ClsInsightsHelper.Chart = ClsInsightsHelper.GetChartByID(user.HomePageChartID)
        Dim uid As String = "TEMP" & chart.ID & "-" & Guid.NewGuid.ToString() & "-" & ClsSessionHelper.LogonUser.GlobalID.ToString()
        If chart.CacheResult Then
            uid = "C" & chart.ID & sopIDs.Replace(",", "").GetHashCode() & environmentID & IIf(countrySplit, "1", "0")
        End If
        Dim CreationStoredProcedureName As String = Nothing
        If chart IsNot Nothing Then
            If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
                CreationStoredProcedureName = chart.CreationStoredProcedureName.Replace("[SOPIDs]", sopIDs) _
                                                                                                   .Replace("[EnvironmentID]", environmentID) _
                                                                                                   .Replace("[CountrySplit]", IIf(countrySplit, "1", "0"))
            End If
        End If
        Dim chartType As ClsInsightsHelper.InsightChartType = chart.Type
        Dim data_gs_height As Integer = chart.data_gs_height
        ClsInsightsHelper.ProcessDataAsync(CreationStoredProcedureName, uid)
        ' Change Pie chart type to Horizontal Bar Chart when country split is applicable and applied
        If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
            If chartType = ClsInsightsHelper.InsightChartType.PieChart And chart.CreationStoredProcedureName.Contains("[CountrySplit]") And countrySplit Then
                chartType = ClsInsightsHelper.InsightChartType.HorizontalBarChart
                data_gs_height += 2
            End If
        End If

        Dim userControlPath As String = ClsInsightsHelper.LoadControlByType(chartType)
        If Not String.IsNullOrEmpty(userControlPath) Then
            Dim userControl As ClsChartUserControl = LoadControl(userControlPath)
            userControl.Name = chart.Name
            userControl.Type = chartType
            userControl.ChartUID = Guid.NewGuid
            userControl.SubTitle = chart.SubTitle
            userControl.ChartID = chart.ID
            userControl.UID = uid
            tblDefaultChart.Controls.Add(userControl)
        End If
    End Sub

    Protected Sub RefreshData()
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            ' Get Recent Activity
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
            parameters.Add(New SqlParameter("@U_GLOBALID", ClsSessionHelper.LogonUser.GlobalID))
            Dim EnvironmentName As String = "NOT APPLICABLE"
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Environment")) Then
                parameters.Add(New SqlParameter("@Environment", ConfigurationManager.AppSettings("Environment")))
            End If
            Dim recentData As DataSet = ClsDataAccessHelper.FillDataSet("Administration.GetRecentActivity", parameters)
            If recentData.Tables.Count = 2 Then
                RecentActionsRepeater.DataSource = recentData.Tables(0)
                RecentActionsRepeater.DataBind()
                recentActivitiesRepeter.DataSource = recentData.Tables(1)
                recentActivitiesRepeter.DataBind()
            End If

            ' Render default chart if defined
            Dim user As ClsUser = ClsSessionHelper.LogonUser
            If user.HomePageChartID > 0 And user.DefaultEnvironmentID > 0 And Not String.IsNullOrEmpty(user.DefaultSOPIDs) Then
                tblDefaultChart.Visible = True
                tblRecentActivities.Attributes.Add("class", "col-lg-5 recent-activities")
                RenderDefaultChart(user)
            Else
                tblDefaultChart.Visible = False
                tblRecentActivities.Attributes.Add("class", "col-lg-12 recent-activities")
            End If
        End If
    End Sub

End Class
