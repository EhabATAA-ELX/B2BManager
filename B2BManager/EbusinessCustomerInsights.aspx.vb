
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading

Partial Class EbusinessCustomerInsights
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim cid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
                Guid.TryParse(Request.QueryString("cid"), cid)
            End If

            If EnvironmentID > 0 AndAlso cid <> Guid.Empty Then
                RenderChart(cid, EnvironmentID)
            End If
        End If
    End Sub

    Private Sub RenderChart(cid As Guid, environmentID As Integer)
        Dim chartType As ClsInsightsHelper.InsightChartType = ClsInsightsHelper.InsightChartType.VerticalBarChart
        Dim uid As String = "History_Chart_" + environmentID.ToString() + "_" & cid.ToString()
        Dim userControlPath As String = ClsInsightsHelper.LoadControlByType(chartType)
        If Not String.IsNullOrEmpty(userControlPath) Then
            Dim userControl As ClsChartUserControl = LoadControl(userControlPath)
            userControl.Name = "Activity overview in last 30 days"
            userControl.SubTitle = String.Empty
            userControl.Type = chartType
            userControl.ChartUID = Guid.NewGuid
            userControl.ChartID = 0
            userControl.UID = uid
            chartPnlContainer.Controls.Add(userControl)
        End If
    End Sub
End Class
