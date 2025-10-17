
Imports System.Data
Imports System.Diagnostics
Imports System.Threading
Imports OfficeOpenXml
Imports Telerik.Web.UI

Partial Class EbusinessInsightsChartPreview
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If Not IsPostBack Then
            chkBoxCountrySplit.Checked = False
            RenderControls()
            If Session("PreviewChart") IsNot Nothing Then
                RenderChart(Session("PreviewChart"))
            End If
            CType(Master.FindControl("title"), HtmlTitle).Text = "Chart Preview"
        End If
    End Sub

    Private Sub RenderChart(chart As ClsInsightsHelper.Chart)
        Dim CreationStoredProcedureName As String = Nothing
        If chart IsNot Nothing Then
            If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
                CreationStoredProcedureName = chart.CreationStoredProcedureName.Replace("[SOPIDs]", ddlCountry.SelectedValue) _
                                                                               .Replace("[EnvironmentID]", ddlEnvironment.SelectedValue) _
                                                                               .Replace("[CountrySplit]", IIf(chkBoxCountrySplit.Checked, "1", "0"))
            End If
        End If
        Dim chartType As ClsInsightsHelper.InsightChartType = chart.Type
        Dim uid As String = "TEMP" & chart.ID & "-" & Guid.NewGuid.ToString() & "-" & ClsSessionHelper.LogonUser.GlobalID.ToString()
        ChartUID.Value = uid
        ClsInsightsHelper.ProcessDataAsync(CreationStoredProcedureName, uid)
        ' Change Pie chart type to Horizontal Bar Chart when country split is applicable and applied
        If Not String.IsNullOrEmpty(chart.CreationStoredProcedureName) Then
            If chartType = ClsInsightsHelper.InsightChartType.PieChart And chart.CreationStoredProcedureName.Contains("[CountrySplit]") And chkBoxCountrySplit.Checked Then
                chartType = ClsInsightsHelper.InsightChartType.HorizontalBarChart
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
            chartPnlContainer.Controls.Add(userControl)
        End If
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.LogonUser.DefaultEnvironmentID.ToString())
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.LogonUser.DefaultSOPIDs, True)
        chkBoxCountrySplit.Checked = ClsSessionHelper.LogonUser.DefaultCountrtySplitStatus
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        If Session("PreviewChart") IsNot Nothing Then
            RenderChart(Session("PreviewChart"))
        End If
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim excelDocumentTitle As String = "Chart Preview Data"
        Dim emptyDashboard As Boolean = True
        Using excel As ExcelPackage = New ExcelPackage()
            If Session("PreviewChart") IsNot Nothing Then
                Dim previewChart As ClsInsightsHelper.Chart = DirectCast(Session("PreviewChart"), ClsInsightsHelper.Chart)
                AddDataToExcel(excel, IIf(String.IsNullOrEmpty(previewChart.ExcelSheetName), IIf(String.IsNullOrEmpty(previewChart.Name), "Preview Data", ClsHelper.Truncate(previewChart.Name, 31)), previewChart.ExcelSheetName), ChartUID.Value.ToString(), True)
                emptyDashboard = False
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
        ClsHelper.Log("Insights Export To Excel", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Preview Chart Data", watch.ElapsedMilliseconds, False, Nothing)
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
                    Dim errorMsg As String = String.Format("<b>Methode Name:</b>AddDataToExcel</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                    ClsSendEmailHelper.SendErrorEmail(errorMsg)
                End Try
            End If
        End If
    End Sub
End Class
