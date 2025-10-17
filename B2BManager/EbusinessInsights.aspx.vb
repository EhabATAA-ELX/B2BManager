
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
Imports OfficeOpenXml
Imports Telerik.Web.UI

Partial Class EbusinessInsights
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If Not IsPostBack Then
            chkBoxCountrySplit.Checked = False
            chkBoxDesignMode.Checked = False
            RenderControls()
        End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            If ClsSessionHelper.ActiveDashboard Is Nothing Then
                ClsSessionHelper.ActiveDashboard = ClsInsightsHelper.GetDashboardByID(ClsSessionHelper.LogonUser.DefaultDashboardID, ClsSessionHelper.LogonUser.GlobalID)
            Else 'Get the latest update each time
                ClsSessionHelper.ActiveDashboard = ClsInsightsHelper.GetDashboardByID(ClsSessionHelper.ActiveDashboard.ID, ClsSessionHelper.LogonUser.GlobalID)
            End If
            RenderDashboard(Guid.NewGuid.ToString())
            watch.Stop()
            ClsHelper.Log("Ebusiness Insights", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Load", watch.ElapsedMilliseconds, False, Nothing)
        Else
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("CurrentDashboardID") Then
                    Dim watch As Stopwatch = Stopwatch.StartNew()
                    If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                        ClsSessionHelper.ActiveDashboard = ClsInsightsHelper.GetDashboardByID(CInt(__EVENTARGUMENT), ClsSessionHelper.LogonUser.GlobalID)
                    Else
                        If ClsSessionHelper.ActiveDashboard IsNot Nothing Then 'Get the latest update each time
                            ClsSessionHelper.ActiveDashboard = ClsInsightsHelper.GetDashboardByID(ClsSessionHelper.ActiveDashboard.ID, ClsSessionHelper.LogonUser.GlobalID)
                        End If
                    End If
                    RenderDashboard(Guid.NewGuid.ToString(), True)
                    watch.Stop()
                    ClsHelper.Log("Ebusiness Insights", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Open dashobard</br>Dashobard ID:" & __EVENTARGUMENT, watch.ElapsedMilliseconds, False, Nothing)
                End If

                If __EVENTTARGET.Contains("chkBoxDesignMode") Then
                    RenderDashboard(CurrentDashobardUID.Value, True)
                End If
            End If
        End If
    End Sub

    Private Sub CheckDesignMode()
        If chkBoxDesignMode.Checked Then
            imgBtnSave.Attributes.Add("class", "MoreInfoImg vertical-align-bottom")
            imgBtnSave.Attributes.Add("onclick", "saveMainPositions()")
        Else
            imgBtnSave.Attributes.Add("class", "MoreInfoImg vertical-align-bottom ImgDisabled")
            imgBtnSave.Attributes.Remove("onclick")
        End If
    End Sub
    Private Sub RenderDashboard(dashobardCacheUniqueIdentifier As String, Optional initChartContainer As Boolean = False)
        Dim editable As Boolean = False
        Dim dashboardContainsAtLeastOneChart As Boolean = False
        If ClsSessionHelper.ActiveDashboard Is Nothing Then
            imgBtnEdit.Attributes.Add("class", "MoreInfoImg vertical-align-bottom ImgDisabled")
            imgBtnEdit.Attributes.Remove("onclick")
        Else
            CurrentDashboardID.Value = ClsSessionHelper.ActiveDashboard.ID
            PnlDashobardCharts.Controls.Clear()
            editable = ClsSessionHelper.ActiveDashboard.Editable
            If Not editable Then
                chkBoxDesignMode.Checked = False
                imgBtnEdit.Attributes.Add("class", "MoreInfoImg vertical-align-bottom ImgDisabled")
                imgBtnEdit.Attributes.Remove("onclick")
            Else
                imgBtnEdit.Attributes.Add("onclick", "EditDashboard(" + ClsSessionHelper.ActiveDashboard.ID.ToString() + ")")
                imgBtnEdit.Attributes.Add("class", "MoreInfoImg vertical-align-bottom")
            End If
            Dim oldDashboardUIDValue As String = CurrentDashobardUID.Value
            CurrentDashobardUID.Value = dashobardCacheUniqueIdentifier.ToString()
            If ClsSessionHelper.ActiveDashboard IsNot Nothing Then
                DashobardTitle.InnerHtml = ClsSessionHelper.ActiveDashboard.Name
                If ClsSessionHelper.ActiveDashboard.Areas IsNot Nothing Then
                    For Each area As ClsInsightsHelper.Area In ClsSessionHelper.ActiveDashboard.Areas.Where(Function(fc) fc.Included)
                        Dim areaTitleContainer As HtmlTable = New HtmlTable
                        Dim areaContainsAtLeastOneChart As Boolean = False
                        Dim areaTitleRow As HtmlTableRow = New HtmlTableRow()
                        areaTitleContainer.Attributes.Add("class", "dashobard-area-title-container")
                        Dim areaTitle As HtmlTableCell = New HtmlTableCell()
                        areaTitle.Attributes.Add("class", "dashobard-area-title")
                        areaTitle.InnerHtml = area.Name
                        areaTitleRow.Cells.Add(areaTitle)
                        If area.ShowHelpTooltip And Not String.IsNullOrWhiteSpace(area.TooltipText) Then
                            Dim areaHelpTooltip As HtmlTableCell = New HtmlTableCell
                            areaHelpTooltip.InnerHtml = String.Format("<img src='Images/Info.png' class='MoreInfoImg' id='ImgTooltipHelp_{0}' width='18' height='18' alt='More details' /><div class='hidden' style='margin:25px;' id=""TooltipContentHelp_{0}"" >{1}</div>", area.ID, area.TooltipText)
                            Me.RadToolTipManager1.TargetControls.Add(String.Format("ImgTooltipHelp_{0}", area.ID), True)
                            areaTitleRow.Cells.Add(areaHelpTooltip)
                        End If
                        areaTitleContainer.Rows.Add(areaTitleRow)
                        Dim gridStackContainer As HtmlGenericControl = New HtmlGenericControl("div")
                        gridStackContainer.Attributes.Add("class", "grid-stack")
                        For Each chart As ClsInsightsHelper.Chart In area.Charts.Where(Function(fc) fc.Included And (Not fc.ProductionExclusive Or ddlEnvironment.SelectedItem.Text.ToLower().Contains("prod")))
                            Dim uid As String = "S" & chart.ID & "-" & dashobardCacheUniqueIdentifier.ToString() & "-" & ClsSessionHelper.LogonUser.GlobalID.ToString()
                            areaContainsAtLeastOneChart = True
                            dashboardContainsAtLeastOneChart = True
                            If chart.CacheResult Then
                                uid = "C" & chart.ID & ddlCountry.SelectedValue.Replace(",", "").GetHashCode() & ddlEnvironment.SelectedValue & IIf(chkBoxCountrySplit.Checked, "1", "0")
                            End If
                            Dim CreationStoredProcedureName As String = Nothing
                            If chart IsNot Nothing Then
                                If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
                                    CreationStoredProcedureName = chart.CreationStoredProcedureName.Replace("[SOPIDs]", ddlCountry.SelectedValue) _
                                                                                                   .Replace("[EnvironmentID]", ddlEnvironment.SelectedValue) _
                                                                                                   .Replace("[CountrySplit]", IIf(chkBoxCountrySplit.Checked, "1", "0"))
                                End If
                            End If
                            Dim chartType As ClsInsightsHelper.InsightChartType = chart.Type
                            Dim data_gs_height As Integer = chart.data_gs_height
                            Dim data_gs_y As Integer = chart.data_gs_y
                            Dim thread As Thread = New Thread(Function(t) ClsInsightsHelper.ProcessData(CreationStoredProcedureName, uid))
                            thread.Start()
                            ' Change Pie chart type to Horizontal Bar Chart when country split is applicable and applied
                            If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
                                If chartType = ClsInsightsHelper.InsightChartType.PieChart And chart.CreationStoredProcedureName.Contains("[CountrySplit]") And chkBoxCountrySplit.Checked Then
                                    chartType = ClsInsightsHelper.InsightChartType.HorizontalBarChart
                                    data_gs_height += 2
                                    If chart.Type = ClsInsightsHelper.InsightChartType.PieChart Then
                                        data_gs_y += 2
                                    End If
                                End If
                            End If
                            Dim userControlPath As String = ClsInsightsHelper.LoadControlByType(chartType)
                            If Not String.IsNullOrEmpty(userControlPath) Then
                                Dim gridStackItem As HtmlGenericControl = New HtmlGenericControl("div")
                                gridStackItem.Attributes.Add("class", "grid-stack-item")
                                gridStackItem.Attributes.Add("data-gs-x", chart.data_gs_x)
                                gridStackItem.Attributes.Add("data-gs-y", data_gs_y)
                                gridStackItem.Attributes.Add("data-gs-width", chart.data_gs_width)
                                gridStackItem.Attributes.Add("data-gs-height", data_gs_height)
                                gridStackItem.Attributes.Add("data-gs-no-resize", IIf(chkBoxDesignMode.Checked, "no", "yes"))
                                gridStackItem.Attributes.Add("data-gs-no-move", IIf(chkBoxDesignMode.Checked, "no", "yes"))
                                gridStackItem.Attributes.Add("data-gs-id", chart.ID)
                                Dim userControl As ClsChartUserControl = LoadControl(userControlPath)
                                userControl.Name = chart.Name
                                userControl.Type = chartType
                                userControl.ChartUID = Guid.NewGuid
                                userControl.SubTitle = chart.SubTitle
                                userControl.ChartID = chart.ID
                                userControl.UID = uid
                                gridStackItem.Controls.Add(userControl)
                                gridStackContainer.Controls.Add(gridStackItem)
                            End If
                        Next
                        If areaContainsAtLeastOneChart Then
                            PnlDashobardCharts.Controls.Add(areaTitleContainer)
                            PnlDashobardCharts.Controls.Add(gridStackContainer)
                        End If
                    Next
                End If
            End If
            If initChartContainer Then
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Init", "initChartContainer();", True)
            End If
        End If

        If Not dashboardContainsAtLeastOneChart Then
            Dim htmlGenericMessages As HtmlGenericControl = New HtmlGenericControl("div")
            htmlGenericMessages.Attributes.Add("class", "generic-message")
            htmlGenericMessages.InnerHtml = "<h2>The dashboard you are trying to visualize has no chart available in your selected environment. Please ensure to change it or load another dashboard.</h2>"
            PnlDashobardCharts.Controls.Add(htmlGenericMessages)
        End If

        chkBoxDesignMode.Enabled = editable
        imgBtnEdit.Disabled = Not editable
        CheckDesignMode()
    End Sub


    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.LogonUser.DefaultEnvironmentID.ToString())
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.LogonUser.DefaultSOPIDs, True)
        chkBoxCountrySplit.Checked = ClsSessionHelper.LogonUser.DefaultCountrtySplitStatus
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        RenderDashboard(Guid.NewGuid.ToString(), True)
    End Sub
    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim excelDocumentTitle As String = "Dashboard"
        Dim emptyDashboard As Boolean = True
        Using excel As ExcelPackage = New ExcelPackage()
            If ClsSessionHelper.ActiveDashboard IsNot Nothing Then
                excelDocumentTitle = ClsSessionHelper.ActiveDashboard.Name
                If ClsSessionHelper.ActiveDashboard.Areas IsNot Nothing Then
                    For Each area As ClsInsightsHelper.Area In ClsSessionHelper.ActiveDashboard.Areas.Where(Function(fc) fc.Included)
                        For Each chart As ClsInsightsHelper.Chart In area.Charts.Where(Function(fc) fc.Included)
                            Dim uid As String = "S" & chart.ID & "-" & CurrentDashobardUID.Value.ToString() & "-" & ClsSessionHelper.LogonUser.GlobalID.ToString()
                            If chart.CacheResult Then
                                uid = "C" & chart.ID & ddlCountry.SelectedValue.Replace(",", "").GetHashCode() & ddlEnvironment.SelectedValue & IIf(chkBoxCountrySplit.Checked, "1", "0")
                            End If
                            AddDataToExcel(excel, chart.ExcelSheetName, uid, chart.ExcelPrintHeaders)
                            If emptyDashboard Then
                                emptyDashboard = False
                            End If
                        Next
                    Next
                End If
            End If

            If emptyDashboard Then
                AddDataToExcel(excel, "Data", Guid.NewGuid().ToString(), True)
            End If

            Response.BinaryWrite(excel.GetAsByteArray())
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            excelDocumentTitle = "attachment;  filename=""" & ClsHelper.RemoveOddCharachters(excelDocumentTitle) & ".xlsx"""
            Response.AddHeader("content-disposition", excelDocumentTitle)
            Response.[End]()
        End Using
        watch.Stop()
        ClsHelper.Log("Insights Export To Excel", ClsSessionHelper.LogonUser.GlobalID.ToString(), "DashboardID : " & CurrentDashobardUID.Value.ToString(), watch.ElapsedMilliseconds, False, Nothing)
    End Sub


    Private Sub AddDataToExcel(ByRef excel As ExcelPackage, workSheetName As String, uid As String, Optional PrintHeaders As Boolean = True)
        Dim dataTable As DataTable = ClsInsightsHelper.ProcessData(Nothing, uid, True)
        If dataTable IsNot Nothing Then
            Dim clonedDt As DataTable = dataTable.Copy()
            For Each column As DataColumn In dataTable.Columns
                If column.ColumnName.ToLower().StartsWith("exlude_") Then
                    clonedDt.Columns.Remove(column.ColumnName)
                End If
            Next
            Dim ws As ExcelWorksheet = excel.Workbook.Worksheets.Add(workSheetName)
            If clonedDt.Rows.Count > 0 Then
                Try
                    ws.Cells("A1").LoadFromDataTable(clonedDt, PrintHeaders)
                    ws.Cells("A1:" & ClsHelper.GetExcelColumnName(clonedDt.Columns.Count) & "1").Style.Font.Bold = True
                    ws.Cells.AutoFitColumns()
                Catch ex As Exception
                    Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                    If Not ex.Message Is Nothing Then
                        exceptionMessage = ex.Message
                    End If
                    If Not ex.StackTrace Is Nothing Then
                        exceptionStackTrace = ex.StackTrace
                    End If
                    Dim errorMsg As String = String.Format("<b>Methode Name:</b>AddDataToExcel with headers</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                    ClsSendEmailHelper.SendErrorEmail(errorMsg)
                End Try
            End If
        End If
    End Sub

End Class
