
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class EbusinessInsightsDashboardProfile
    Inherits System.Web.UI.Page

    Private Const EDIT_ACTION_STRING As String = "<span class='action-button' title='Edit Chart' onclick='EditChart(""{0}"")'>Edit</span>"
    Private Const DELETE_ACTION_STRING As String = "<span class='action-button' title='Delete chart' onclick='DeleteChart(""{0}"")'>Delete</span>"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            RenderDashboardChartsAndSections()
        End If
    End Sub

    Private Sub RenderDashboardChartsAndSections()
        Dim DashboardID As Integer = 0
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        btnNewChartPlaceHolder.Visible = ClsInsightsHelper.CanUserAddChart(logonUser)
        Integer.TryParse(Request.QueryString("did"), DashboardID)
        Dim dashboard As ClsInsightsHelper.Dashobard = Nothing
        If DashboardID > 0 Then
            dashboard = ClsInsightsHelper.GetDashboardByID(DashboardID, logonUser.GlobalID)
            If Not IsPostBack Then
                CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Dashboard " & dashboard.Name
                txtBoxDashboardName.Text = dashboard.Name
                ddlSharingType.SelectedValue = IIf(dashboard.IsShared, "1", "0")
                txtBoxDashboardName.Enabled = dashboard.Editable
                ddlSharingType.Enabled = dashboard.Editable
                btnExecute.Visible = dashboard.Editable
            End If
        Else
            dashboard = ClsInsightsHelper.GetDashboardByID(0, logonUser.GlobalID)
            If Not IsPostBack Then
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Dashboard"
            End If
        End If
        tvCharts.Nodes.Clear()
        If dashboard IsNot Nothing Then
            If dashboard.Areas IsNot Nothing Then
                For Each area As ClsInsightsHelper.Area In dashboard.Areas
                    Dim radAreaTreeNode As RadTreeNode = New RadTreeNode(area.Name, "Area" & area.ID)
                    radAreaTreeNode.ImageUrl = "Images/Insights/Section.png"
                    radAreaTreeNode.Enabled = dashboard.Editable
                    If area.Charts IsNot Nothing Then
                        If area.Charts.Where(Function(fc) fc.Included).Count > 0 Then
                            radAreaTreeNode.Expanded = True
                            If area.Charts.Count = area.Charts.Where(Function(fc) fc.Included).Count Then
                                radAreaTreeNode.Checked = True
                            End If
                        End If

                        For Each chart As ClsInsightsHelper.Chart In area.Charts.OrderBy(Function(fc) fc.ID)
                            Dim sharingTag As String = ClsInsightsHelper.FormatTag(IIf(chart.Shared, "Shared", "Private"), IIf(chart.Shared, "#00b0f0", "#767171")) + " "
                            Dim chartTreeNode As RadTreeNode = New RadTreeNode(sharingTag + chart.Name, chart.ID)
                            chartTreeNode.Checked = chart.Included
                            chartTreeNode.Enabled = dashboard.Editable
                            'If chart.Editable Or logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_ALL_SHARED_CHARTS) Then
                            '    chartTreeNode.Text += " " + String.Format(EDIT_ACTION_STRING, chart.ID)
                            'End If
                            'If chart.Editable Or logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_ALL_SHARED_CHARTS) Then
                            '    chartTreeNode.Text += "|" + String.Format(DELETE_ACTION_STRING, chart.ID)
                            'End If
                            chartTreeNode.ImageUrl = ClsInsightsHelper.GetImageUrlFromChartType(chart.Type)
                            radAreaTreeNode.Nodes.Add(chartTreeNode)
                        Next
                    End If
                    tvCharts.Nodes.Add(radAreaTreeNode)
                Next
            End If
        End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("BtnRefreshChartsAndSections") Then
                    RenderDashboardChartsAndSections()
                End If
            End If
        End If
    End Sub

    Protected Sub btnExecute_Click(sender As Object, e As EventArgs)
        Dim validated As Boolean = True
        Dim infoMessage As String = ""
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If String.IsNullOrWhiteSpace(txtBoxDashboardName.Text) Then
            infoMessage = "Dashboard name is required"
            validated = False
        End If

        Dim DashboardID As Integer = 0
        Integer.TryParse(Request.QueryString("did"), DashboardID)

        If Not validated Then
            lblInfoMessage.ForeColor = System.Drawing.Color.Red
            lblInfoMessage.Text = infoMessage
        Else
            If tvCharts.CheckedNodes.Count = 0 Then
                lblInfoMessage.ForeColor = System.Drawing.Color.Red
                lblInfoMessage.Text = "Please select at least one chart"
            Else
                Try
                    lblInfoMessage.Text = " "

                    Dim isShared As Boolean = ddlSharingType.SelectedValue = "1"
                    Dim checkedNodes As String = String.Join(",", tvCharts.CheckedNodes.ToList().Select(Function(fc) fc.Value))
                    Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parameters.Add(New SqlParameter("@DashboardID", DashboardID))
                    parameters.Add(New SqlParameter("@DashboardName", txtBoxDashboardName.Text))
                    parameters.Add(New SqlParameter("@IsShared", isShared))
                    parameters.Add(New SqlParameter("@SelectedCharts", checkedNodes))
                    parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
                    Dim resultedDashboard As Integer = ClsDataAccessHelper.ExecuteScalar("Insights.SaveOrUpdateDashboard", parameters)
                    If (resultedDashboard > 0) Then
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadDashboard", "window.parent.LoadDashboard(" & resultedDashboard & ");", True)
                        ClsHelper.Log(IIf(DashboardID > 0, "Edit Dashboard", "Create Dashboard"), ClsSessionHelper.LogonUser.GlobalID.ToString(), "Dashboard name: " & txtBoxDashboardName.Text + "</br>Dashboard ID: " & resultedDashboard, watch.ElapsedMilliseconds, False, Nothing)
                    Else
                        lblInfoMessage.ForeColor = System.Drawing.Color.Red
                        lblInfoMessage.Text = "An unexpected error has occurred. please try again later."
                    End If
                Catch ex As Exception
                    lblInfoMessage.ForeColor = System.Drawing.Color.Red
                    lblInfoMessage.Text = "An unexpected error has occurred. please try again later."
                    Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                    If Not ex.Message Is Nothing Then
                        exceptionMessage = ex.Message
                    End If
                    If Not ex.StackTrace Is Nothing Then
                        exceptionStackTrace = ex.StackTrace
                    End If
                    Dim errorMsg As String = String.Format("Unable to Save Dashboard</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                        , exceptionStackTrace
                        )
                    ClsHelper.Log(IIf(DashboardID > 0, "Edit Dashboard", "Create Dashboard"), ClsSessionHelper.LogonUser.GlobalID.ToString(), "Dashboard name: " & txtBoxDashboardName.Text + IIf(DashboardID > 0, "</br>Dashboard ID: " & DashboardID, ""), watch.ElapsedMilliseconds, True, errorMsg)
                End Try
            End If
        End If
    End Sub
End Class
