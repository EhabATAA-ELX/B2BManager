Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Reflection
Imports InsightsDataService
Imports Microsoft.VisualBasic

Public Class ClsInsightsHelper

    <Serializable>
    Public Class InsightEntity
        Public ID As Integer
        Public Name As String

        Public Sub New(iD As Integer, name As String)
            Me.ID = iD
            Me.Name = name
        End Sub
    End Class

    <Serializable>
    Public Class Dashobard
        Inherits InsightEntity
        Public IsShared As Boolean
        Public Editable As Boolean
        Public Areas As List(Of Area)

        Public Sub New(iD As Integer, name As String, isShared As Boolean, editable As Boolean)
            MyBase.New(iD, name)
            Me.Editable = editable
            Me.IsShared = isShared
        End Sub
    End Class

    <Serializable>
    Public Class Area
        Inherits InsightEntity
        Public ShowHelpTooltip As Boolean
        Public Included As Boolean
        Public Editable As Boolean
        Public Deletable As Boolean
        Public TooltipText As String
        Public Charts As List(Of Chart)
        Public Sub New(iD As Integer, name As String, showHelpTooltip As Boolean, tooltipText As String)
            MyBase.New(iD, name)
            Me.ShowHelpTooltip = showHelpTooltip
            Me.TooltipText = tooltipText
        End Sub
    End Class

    <Serializable>
    Public Class Chart
        Inherits InsightEntity
        Public Type As InsightChartType
        Public CacheResult As Boolean
        Public Comments As String
        Public SubTitle As String
        Public ProductionExclusive As Boolean
        Public CreationStoredProcedureName As String
        Public ExcelPrintHeaders As Boolean
        Public ExcelSheetName As String
        Public data_gs_x As Integer
        Public data_gs_y As Integer
        Public data_gs_height As Integer
        Public data_gs_width As Integer
        Public TemplateHTML As String
        Public Included As Boolean
        Public Editable As Boolean
        Public [Shared] As Boolean
        Public Sub New(iD As Integer, name As String, type As InsightChartType, subTitle As String, comments As String)
            MyBase.New(iD, name)
            Me.Type = type
            Me.SubTitle = subTitle
            Me.Comments = comments
        End Sub
    End Class

    Public Shared Function CanUserAddChart(logonUser As ClsUser) As Boolean
        Dim canAddChart = False
        If logonUser.Applications IsNot Nothing Then
            canAddChart = logonUser.Applications.Where(Function(fc) fc.Checked).Count > 0
        End If
        If logonUser.Actions.Count > 0 Then
            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.SETUP_SURVEY_CHARTS) Then
                canAddChart = True
            End If
            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.SETUP_FREE_QUERIES_CHARTS) Then
                canAddChart = True
            End If
        End If
        Return canAddChart
    End Function

    <Serializable>
    Public Class Survey
        Public ID As String
        Public Name As String
        Public Questions As List(Of SurveyQuestion)

        Public Sub New(name As String, iD As String)
            Me.ID = iD
            Me.Name = name
        End Sub
    End Class

    <Serializable>
    Public Class SurveyQuestion
        Public ID As String
        Public Name As String

        Public Sub New(name As String, iD As String)
            Me.ID = iD
            Me.Name = name
        End Sub
    End Class

    Public Shared Function FormatTag(TagName As String, BackgroundColor As String, Optional title As String = "") As String
        Return String.Format("<span style='background-color:{0};' {2} class='sharingTag'>{1}</span>", BackgroundColor, TagName, IIf(String.IsNullOrEmpty(title), "", "title='" & title & "'"))
    End Function

    Public Shared Function GetImageUrlFromChartType(type As InsightChartType) As String
        Dim imageUrl As String = "Images/Insights/"
        Select Case type
            Case InsightChartType.CustomDataTable
                imageUrl += "datatable.png"
            Case InsightChartType.PieChart
                imageUrl += "Pie.png"
            Case InsightChartType.HorizontalBarChart
                imageUrl += "Bar.png"
            Case InsightChartType.LineChart
                imageUrl += "Line.png"
            Case InsightChartType.VerticalBarChart
                imageUrl += "VerticalBar.png"
        End Select
        Return imageUrl
    End Function

    Public Enum InsightChartType
        PieChart = 1
        LineChart = 2
        HorizontalBarChart = 3
        CustomDataTable = 4
        VerticalBarChart = 5
    End Enum

    Public Enum InsightDataSourceType
        LogViewerApplication = 0
        SurveyResults = 1
        FreeQueryRecords = 2
    End Enum

    Public Shared Async Function ProcessDataAsync(CreationStoredProcedureName As String, uid As String, Optional GetDataOnly As Boolean = False) As Threading.Tasks.Task(Of DataTable)
        Dim data As DataTable = New DataTable()
        Try
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If GetDataOnly Then
                    If client.CheckItemInCache(uid) Then
                        data = Await client.GetDataAsync(uid, CreationStoredProcedureName)
                    End If
                Else
                    data = Await client.GetDataAsync(uid, CreationStoredProcedureName)
                End If
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>ProcessDataAsync</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return data
    End Function

    Public Shared Function ProcessData(CreationStoredProcedureName As String, uid As String, Optional GetDataOnly As Boolean = False) As DataTable
        Dim data As DataTable = New DataTable()
        Try
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If GetDataOnly Then
                    If client.CheckItemInCache(uid) Then
                        data = client.GetData(uid, CreationStoredProcedureName)
                    End If
                Else
                    data = client.GetData(uid, CreationStoredProcedureName)
                End If
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>ProcessData</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return data
    End Function

    Public Shared Function Preload() As Boolean
        Dim preloaded As Boolean = False
        Try
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                preloaded = Not client.CheckItemInCache("PRELOAD")
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>PreLoad</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return preloaded
    End Function

    Public Shared Function LoadControlByType(chartType As InsightChartType) As String
        Dim userControlPath As String = ""
        Select Case chartType
            Case InsightChartType.CustomDataTable
                userControlPath = "~/UserControls/InsightsCustomDataTable.ascx"
                Exit Select
            Case InsightChartType.HorizontalBarChart
                userControlPath = "~/UserControls/InsightsHorizontalBarChart.ascx"
                Exit Select
            Case InsightChartType.LineChart
                userControlPath = "~/UserControls/InsightsLineChart.ascx"
                Exit Select
            Case InsightChartType.PieChart
                userControlPath = "~/UserControls/InsightsPieChart.ascx"
                Exit Select
            Case InsightChartType.VerticalBarChart
                userControlPath = "~/UserControls/InsightsBarChart.ascx"
                Exit Select
        End Select
        Return userControlPath
    End Function

    Friend Shared Function ConvertDataTableToHTMLString(dataTable As DataTable, chartID As Integer) As String
        If dataTable Is Nothing Then
            Return String.Empty
        End If

        Dim HtmlString As String = String.Empty
        Dim TemplateHTML As String = String.Empty
        Dim TitleHTML As String = String.Empty
        If chartID = 0 Then
            TitleHTML = "<div class='custom-chart-title'>Title is not available in chart creation preview</div><div class='custom-chart-subtitle'>You can customize/add your sepearte template by amending the <i>TemplateHTML</i> column in the <i>Insight.charts</i> table</div>"
        Else
            Dim chart As Chart = GetChartByID(chartID)
            If chart IsNot Nothing Then
                TitleHTML = "<div class='custom-chart-title'>" & chart.Name & "</div><div class='custom-chart-subtitle'>" & chart.SubTitle & "</div>"
                TemplateHTML = chart.TemplateHTML
            End If
        End If

        If String.IsNullOrEmpty(TemplateHTML) Then
            TemplateHTML = "<tr>"
            For Each column As DataColumn In dataTable.Columns
                TemplateHTML = TemplateHTML + "<td>[" + column.ColumnName + "]</td>"
            Next
            TemplateHTML = TemplateHTML + "</tr>"
        End If

        HtmlString = TitleHTML + "<table border='0' cellspacing='5' cellpadding='5' align='center' class='custom-chart-table'>"
        For Each row As DataRow In dataTable.Rows
            Dim rowHtml As String = TemplateHTML
            For Each column As DataColumn In dataTable.Columns
                rowHtml = rowHtml.Replace("[" + column.ColumnName + "]", ClsDataAccessHelper.GetText(row, column.ColumnName))
            Next
            HtmlString += rowHtml
        Next

        Return HtmlString
    End Function

    Public Shared Function ImportDataTableWithBulkAndApplyDesign(ByRef Dt As DataTable, ByVal dashboardID As Integer) As Integer

        Dim IsInError As Boolean = False
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@DashboardID", dashboardID))
        ClsDataAccessHelper.FillDataTable("[Insights].[ArchiveDashboardDesign]", parameters)

        Dim userGuid As Guid = Guid.Empty
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            userGuid = ClsSessionHelper.LogonUser.GlobalID
        End If

        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
        Try
            cn.Open()
            Dt.AcceptChanges()
            Dim dashboardIDColumn As DataColumn = New DataColumn("DashboardID", GetType(System.Int32))
            dashboardIDColumn.AllowDBNull = False
            dashboardIDColumn.DefaultValue = dashboardID
            dashboardIDColumn.ColumnName = "DashboardID"
            Dt.Columns.Add(dashboardIDColumn)
            Dim copy As SqlBulkCopy = New SqlBulkCopy(cn)
            copy.BulkCopyTimeout = 0
            copy.DestinationTableName = "Insights.Dashboards_Charts"
            copy.ColumnMappings.Add("DashboardID", "DashboardID")
            copy.ColumnMappings.Add("ChartID", "ChartID")
            copy.ColumnMappings.Add("x", "data-gs-x")
            copy.ColumnMappings.Add("y", "data-gs-y")
            copy.ColumnMappings.Add("h", "data-gs-height")
            copy.ColumnMappings.Add("w", "data-gs-width")
            copy.WriteToServer(Dt)
            copy.Close()
            cn.Close()
            ClsHelper.Log("Insights Change Design", userGuid.ToString(), "Dashobard ID: " & dashboardID, watch.ElapsedMilliseconds, False, Nothing)
        Catch ex As Exception
            IsInError = True
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsHelper.Log("Insights Change Design", userGuid.ToString(), "Change Design</br>Dashobard ID: " & dashboardID, watch.ElapsedMilliseconds, True, errorMsg)
        End Try

        Return Not IsInError
    End Function



    Public Shared Function GetDashboardByID(DashboardID As Integer, UserGlobalID As Guid) As Dashobard
        Dim dashobard = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@DashobardID", DashboardID))
        parameters.Add(New SqlParameter("@UserGlobalID", UserGlobalID))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Insights].[GetDashobardMasterData]", parameters)

        If dataSet IsNot Nothing Then
            If dataSet.Tables.Count = 3 Then
                If dataSet.Tables(0).Rows.Count = 1 Then
                    dashobard = New Dashobard(DashboardID, ClsDataAccessHelper.GetText(dataSet.Tables(0).Rows(0), "Name"), dataSet.Tables(0).Rows(0)("IsShared"), dataSet.Tables(0).Rows(0)("Editable"))
                Else
                    If DashboardID = 0 Then
                        dashobard = New Dashobard(0, "Template", True, True)
                    End If
                End If
                If dataSet.Tables(1).Rows.Count > 0 Then
                    dashobard.Areas = New List(Of Area)
                    For Each areaRow As DataRow In dataSet.Tables(1).Rows
                        Dim area As Area = New Area(areaRow("ID"), ClsDataAccessHelper.GetText(areaRow, "Name"), areaRow("ShowHelpTooltip"), ClsDataAccessHelper.GetText(areaRow, "TooltipText"))
                        area.Included = areaRow("Included")
                        area.Charts = New List(Of Chart)()
                        For Each chartRow In dataSet.Tables(2).Select(String.Format("AreaID={0}", area.ID)).ToList()
                            Dim chart As New Chart(chartRow("ID"), ClsDataAccessHelper.GetText(chartRow, "Title"), chartRow("TypeID"), ClsDataAccessHelper.GetText(chartRow, "SubTitle"), ClsDataAccessHelper.GetText(chartRow, "Comments"))
                            chart.CacheResult = chartRow("CacheResult")
                            chart.CreationStoredProcedureName = ClsDataAccessHelper.GetText(chartRow, "CreationStoredProcedureName")
                            chart.ProductionExclusive = chartRow("ProductionExclusive")
                            chart.ExcelPrintHeaders = chartRow("ExcelPrintHeaders")
                            chart.ExcelSheetName = ClsDataAccessHelper.GetText(chartRow, "ExcelSheetName")
                            chart.Included = chartRow("Included")
                            chart.TemplateHTML = ClsDataAccessHelper.GetText(chartRow, "TemplateHTML")
                            If chart.Included Then
                                chart.data_gs_x = chartRow("data-gs-x")
                                chart.data_gs_y = chartRow("data-gs-y")
                                chart.data_gs_height = chartRow("data-gs-height")
                                chart.data_gs_width = chartRow("data-gs-width")
                            End If
                            chart.Editable = chartRow("Editable")
                            chart.Shared = chartRow("IsShared")
                            area.Charts.Add(chart)
                        Next
                        dashobard.Areas.Add(area)
                    Next
                End If
            End If
        End If

        Return dashobard
    End Function

    Public Shared Function GetDashboards(UserGlobalID As Guid) As DataTable
        Dim dashobard = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserGlobalID", UserGlobalID))
        Return ClsDataAccessHelper.FillDataTable("[Insights].[GetAvailableDashboards]", parameters)
    End Function

    Public Shared Function GetSuverys(environmentID As Integer) As List(Of Survey)
        Dim surveys As List(Of Survey) = New List(Of Survey)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("Insights.GetSurveysInformation", parameters)
        If dataSet.Tables.Count = 2 Then
            For Each dataRow As DataRow In dataSet.Tables(0).Rows
                Dim survey As Survey = New Survey(ClsDataAccessHelper.GetText(dataRow, "Title"), dataRow("ID").ToString())
                Dim questions As List(Of SurveyQuestion) = New List(Of SurveyQuestion)
                For Each questionDataRow As DataRow In dataSet.Tables(1).Select("SurveyID='" + dataRow("ID").ToString() + "'")
                    questions.Add(New SurveyQuestion(ClsDataAccessHelper.GetText(questionDataRow, "Question"), questionDataRow("ID").ToString()))
                Next
                If questions.Count > 0 Then
                    survey.Questions = questions
                    surveys.Add(survey)
                End If
            Next
        End If
        Return surveys
    End Function

    Public Shared Function GetChartByID(ChartID As Integer) As Chart
        Dim chart = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ChartID", ChartID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Insights].[GetChartByID]", parameters)
        If dataTable IsNot Nothing Then
            If dataTable.Rows.Count = 1 Then
                Dim chartRow As DataRow = dataTable.Rows(0)
                chart = New Chart(chartRow("ID"), ClsDataAccessHelper.GetText(chartRow, "Title"), chartRow("TypeID"), ClsDataAccessHelper.GetText(chartRow, "SubTitle"), ClsDataAccessHelper.GetText(chartRow, "Comments"))
                chart.CacheResult = chartRow("CacheResult")
                chart.CreationStoredProcedureName = ClsDataAccessHelper.GetText(chartRow, "CreationStoredProcedureName")
                chart.ProductionExclusive = chartRow("ProductionExclusive")
                chart.ExcelPrintHeaders = chartRow("ExcelPrintHeaders")
                chart.ExcelSheetName = ClsDataAccessHelper.GetText(chartRow, "ExcelSheetName")
                chart.TemplateHTML = ClsDataAccessHelper.GetText(chartRow, "TemplateHTML")
            End If
        End If
        Return chart
    End Function

    Public Shared Function GetAreas(userGlobalID As Guid) As List(Of Area)
        Dim resultList As List(Of Area) = New List(Of Area)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserGlobalID", userGlobalID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Insights].[GetAreas]", parameters)
        If dataTable IsNot Nothing Then
            For Each areaRow As DataRow In dataTable.Rows
                Dim area As Area = New Area(areaRow("ID"), ClsDataAccessHelper.GetText(areaRow, "Name"), areaRow("ShowHelpTooltip"), ClsDataAccessHelper.GetText(areaRow, "TooltipText"))
                area.Deletable = areaRow("Deletable")
                area.Editable = areaRow("Editable")
                resultList.Add(area)
            Next
        End If
        Return resultList
    End Function

    Public Shared Function GetAreaByID(areaID As Integer, userGlobalID As Guid) As Area
        Dim resultArea As Area = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserGlobalID", userGlobalID))
        parameters.Add(New SqlParameter("@AreaID", areaID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Insights].[GetAreaByID]", parameters)
        If dataTable IsNot Nothing Then
            If dataTable.Rows.Count = 1 Then
                Dim areaRow As DataRow = dataTable.Rows(0)
                resultArea = New Area(areaRow("ID"), ClsDataAccessHelper.GetText(areaRow, "Name"), areaRow("ShowHelpTooltip"), ClsDataAccessHelper.GetText(areaRow, "TooltipText"))
                resultArea.Deletable = areaRow("Deletable")
                resultArea.Editable = areaRow("Editable")
            End If
        End If
        Return resultArea
    End Function

    Public Shared Function ConvertDataTableToClsInsightChartDataList(ByVal dataTable As DataTable) As List(Of ClsInsightLineChartData)
        Dim resultList As List(Of ClsInsightLineChartData) = New List(Of ClsInsightLineChartData)()
        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim YValue As Decimal? = Nothing
                If row.Table.Columns.Contains("Y Value") Then
                    If row("Y Value") IsNot DBNull.Value Then
                        YValue = Decimal.Parse(row("Y Value").ToString())
                    End If
                End If
                If row.Table.Columns.Contains("X Value") Then
                    If row.Table.Columns.Contains("Series Name") Then
                        resultList.Add(New ClsInsightLineChartData(ClsDataAccessHelper.GetText(row, "Series Name"), ClsDataAccessHelper.GetText(row, "X Value"), YValue))
                    ElseIf row.Table.Columns.Contains("Name") Then
                        resultList.Add(New ClsInsightLineChartData(ClsDataAccessHelper.GetText(row, "Name"), ClsDataAccessHelper.GetText(row, "X Value"), YValue))
                    End If
                End If
            Next
        End If
        Return resultList
    End Function

    Public Shared Function ConvertDataTableToClsInsightPieChartData(ByVal dataTable As DataTable) As List(Of ClsInsightPieChartData)
        Dim resultList As List(Of ClsInsightPieChartData) = New List(Of ClsInsightPieChartData)()
        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim Value As Decimal = 0
                If row.Table.Columns.Contains("Value") Then
                    If row("Value") IsNot DBNull.Value Then
                        Value = Decimal.Parse(row("Value").ToString())
                    End If
                    resultList.Add(New ClsInsightPieChartData(ClsDataAccessHelper.GetText(row, "Name"), Value))
                End If
            Next
        End If
        Return resultList
    End Function

    Public Shared Function ConvertDataTableToClsInsightBarStackChartData(ByVal dataTable As DataTable) As List(Of ClsInsightBarStackChartData)
        Dim resultList As List(Of ClsInsightBarStackChartData) = New List(Of ClsInsightBarStackChartData)()
        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim Value As Decimal = 0
                If row.Table.Columns.Contains("Value") Then
                    If row("Value") IsNot DBNull.Value Then
                        Value = Decimal.Parse(row("Value").ToString())
                    End If
                    resultList.Add(New ClsInsightBarStackChartData(ClsDataAccessHelper.GetText(row, "Name"), Value, ClsDataAccessHelper.GetText(row, "Country"), ClsDataAccessHelper.GetText(row, "Stack", "Default")))
                End If
            Next
        End If
        Return resultList
    End Function
End Class

