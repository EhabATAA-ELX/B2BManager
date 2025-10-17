
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class EbusinessInsightsChartProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If Not IsPostBack Then
            Dim chartID As Integer = 0
            Integer.TryParse(Request.QueryString("cid"), chartID)
            Dim chart As ClsInsightsHelper.Chart = Nothing
            If chartID > 0 Then
                chart = ClsInsightsHelper.GetChartByID(chartID)
                CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Chart " & chart.Name
            Else
                Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
                RadDateTimePickerFrom.SelectedDate = fromDate
                Dim toDate = fromDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)
                RadDateTimePickerTo.SelectedDate = toDate
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Insights Chart"
            End If
            If logonUser Is Nothing Then
                Return
            End If
            RenderSections()
            RendGroupByDropDownList()
            Dim hideDataSourceFrom As Boolean? = Nothing
            If logonUser.Applications IsNot Nothing Then
                If logonUser.Applications.Where(Function(fc) fc.Checked).Count > 0 Then
                    ddlSource.Items.Add(New ListItem("Application Logs", "0"))
                    ddlSource.SelectedValue = "0"
                    RenderApplicationDropDownList()
                    hideDataSourceFrom = True
                End If
            End If
            If logonUser.Actions.Count > 0 Then
                If logonUser.Actions.Contains(ClsHelper.ActionDesignation.SETUP_SURVEY_CHARTS) Then
                    If ddlSurveyEnvironment.Items.Count > 0 Then
                        Dim listOfSurveys As List(Of ClsInsightsHelper.Survey) = GetSurveysByEnvironmentID(ddlSurveyEnvironment.SelectedValue)
                        If listOfSurveys IsNot Nothing Then
                            ddlSource.Items.Add(New ListItem("Survey Results", "1"))
                            LoadSurveyLists(listOfSurveys)
                            If hideDataSourceFrom Is Nothing Then
                                ClsHelper.RenderDropDownList(ddlSurveyEnvironment, logonUser.Applications.Where(Function(fc) fc.ID = 1).SingleOrDefault().Environments, True)
                                ddlSource.SelectedValue = "1"
                            End If
                            hideDataSourceFrom = False
                        End If
                    End If
                End If
                If logonUser.Actions.Contains(ClsHelper.ActionDesignation.SETUP_FREE_QUERIES_CHARTS) Then
                    ddlSource.Items.Add(New ListItem("Free Query Records", "2"))
                    If hideDataSourceFrom Is Nothing Then
                        ddlSource.SelectedValue = "2"
                    End If
                    hideDataSourceFrom = False
                End If
            End If
            If hideDataSourceFrom Is Nothing Then
                UpdatePanel1.Visible = False
            Else
                If hideDataSourceFrom Then
                    trDataFrom.Visible = False
                End If
                ChangeControlsDisplayBySource(ddlSource.SelectedValue)
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
                If __EVENTTARGET.Contains("BtnRefreshSections") Then
                    RenderSections()
                End If
            End If
        End If
    End Sub

    Protected Sub ddlApplicationName_SelectedIndexChanged(sender As Object, e As EventArgs)
        RenderApplicationDropDownList(ddlApplicationName.SelectedValue)
    End Sub

    Private Sub RenderSections()
        Dim sections As List(Of ClsInsightsHelper.Area) = ClsInsightsHelper.GetAreas(ClsSessionHelper.LogonUser.GlobalID)
        ddlSections.Items.Clear()
        For Each section As ClsInsightsHelper.Area In sections
            ddlSections.Items.Add(New ListItem(section.Name, section.ID.ToString()))
        Next
    End Sub

    Private Sub RenderApplicationDropDownList(Optional ByVal selectedValue As String = Nothing)
        Dim activeApplications As List(Of ClsHelper.BasicModel) = New List(Of ClsHelper.BasicModel)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If

        For Each application As ClsHelper.Application In applications
            If application.Checked = True Then
                activeApplications.Add(application)
            End If
        Next
        ClsHelper.RenderDropDownList(ddlApplicationName, activeApplications, True)
        ClsHelper.RenderDropDownList(ddlSurveyEnvironment, applications.Where(Function(fc) fc.ID = 1).SingleOrDefault().Environments, True)
        If selectedValue Is Nothing Then
            ddlApplicationName.SelectedIndex = 0
            selectedValue = ddlApplicationName.SelectedValue
        Else
            ddlApplicationName.SelectedValue = selectedValue
        End If
        If Not String.IsNullOrEmpty(selectedValue) Then
            Dim selectedApplication = ClsHelper.FindApplicationByID(applications, selectedValue)
            RenderActionDropDownList(selectedApplication.Actions, True, selectedApplication.AppendAllItemInActions)
        End If
        InitMinDate()
    End Sub

    Private Sub RendGroupByDropDownList()
        ddlGroupBy.Items.Clear()
        If ddlChartType.SelectedValue = "1" Then
            lblGroupBy.Text = "Group by"
            ddlGroupBy.Items.Add(New ListItem("Action Name", "ActionName"))
        Else
            lblGroupBy.Text = "Aggregate by"
            ddlGroupBy.Items.Add(New ListItem("Count of actions (with split)", "CountActionsWithSplit"))
            ddlGroupBy.Items.Add(New ListItem("Count of actions (grouped together)", "CountActionsGroupedTogether"))
            If ddlAction.SelectedValue <> 2 AndAlso ddlChartType.SelectedValue = "2" Then
                ddlGroupBy.Items.Add(New ListItem("Avg. of elapsed time (with action split)", "AverageElapsedTime"))
            End If
        End If
    End Sub

    Private Sub InitMinDate()
        Select Case ddlApplicationName.SelectedValue
            Case 1, 4
                Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
                RadDateTimePickerFrom.MinDate = fromDate.AddDays(-60)
                RadDateTimePickerTo.MinDate = fromDate.AddDays(-60)
            Case 2
                Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
                RadDateTimePickerFrom.MinDate = fromDate.AddYears(-5)
                RadDateTimePickerTo.MinDate = fromDate.AddYears(-5)
            Case Else
                RadDateTimePickerFrom.MinDate = New Date(2018, 1, 30) 'First day of log
                RadDateTimePickerTo.MinDate = New Date(2018, 1, 30) 'First day of log
        End Select
    End Sub

    Private Sub RenderActionDropDownList(ByVal actions As List(Of ClsHelper.Action), Optional clearItems As Boolean = False, Optional appendDataBoundItem As Boolean = False)
        Dim SelectedValue As String = ddlAction.SelectedValue
        If (clearItems) Then
            ddlAction.Items.Clear()
        End If
        Dim index As Integer = 0
        If (appendDataBoundItem) And actions.Count > 0 Then
            ddlAction.Items.Insert(index, New ListItem("All", "0"))
            index = +1
        End If
        For Each action As ClsHelper.Action In actions
            ddlAction.Items.Insert(index, New ListItem(action.Name, action.ID.ToString()))
            If action.CorrespondentActionID > 0 And action.CorrespondentActionID.ToString() = SelectedValue Then
                ddlAction.SelectedIndex = index
            End If
            index = +1
        Next
    End Sub

    Protected Sub ddlDateInterval_SelectedIndexChanged(sender As Object, e As EventArgs)
        trDateIntervalDynamic.Visible = ddlDateInterval.SelectedValue = "Dynamic"
        trDateIntervalStaticFrom.Visible = ddlDateInterval.SelectedValue = "Static"
        trDateIntervalStaticTo.Visible = ddlDateInterval.SelectedValue = "Static"
    End Sub
    Protected Sub ddlDynamicDateInterval_SelectedIndexChanged(sender As Object, e As EventArgs)
        pnlIntervalToSpecify.Visible = ddlDynamicDateInterval.SelectedValue = "InLast"
    End Sub

    Protected Sub ddlChartType_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If
        Dim selectedApplication = ClsHelper.FindApplicationByID(applications, ddlApplicationName.SelectedValue)
        RendGroupByDropDownList()
    End Sub

    Private Function GetChartCreationStoreProcedure() As String
        If ddlSource.SelectedValue = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords Then
            Return txtBoxQueryText.Text
        Else
            Dim CreationStoredProcedureName As String = "[Insights].[GenericChartBuilder] @EnvironmentID = [EnvironmentID],"
            If ddlSource.SelectedValue = ClsInsightsHelper.InsightDataSourceType.SurveyResults Then
                CreationStoredProcedureName = "[Insights].[GenericSuveryChartBuilder]"
            End If
            Dim CreationStoredProcedureScript As String = CreationStoredProcedureName & " @SOPIDs = '[SOPIDs]',@CountrySplit=[CountrySplit] "
            CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "IsDynamicDateInterval", ddlDateInterval.SelectedValue = "Dynamic")
            If ddlDateInterval.SelectedValue = "Dynamic" Then
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "DateIntervalType", ddlDynamicDateInterval.SelectedValue)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "DateIntervalValue", txtDateUnit.Value)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "DateIntervalUnit", ddlDateUnits.SelectedValue)
            Else
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "From", RadDateTimePickerFrom.SelectedDate.Value.ToString("yyyy-MM-dd HH:mm"))
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "To", RadDateTimePickerTo.SelectedDate.Value.ToString("yyyy-MM-dd HH:mm"))
            End If
            CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "ChartType", ddlChartType.SelectedValue)

            If ddlSource.SelectedValue = ClsInsightsHelper.InsightDataSourceType.SurveyResults Then
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "EnvironmentID", ddlSurveyEnvironment.SelectedValue)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "SurveyID", ddlSurvey.SelectedValue)
                If Not ddlSurveyQuestion.SelectedValue = Guid.Empty.ToString() Then
                    CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "QuestionID", ddlSurveyQuestion.SelectedValue)
                End If
            Else
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "ApplicationID", ddlApplicationName.SelectedValue)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "ActionID", ddlAction.SelectedValue)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "ActionsStatus", ddlActionToDisplay.SelectedValue)
                CreationStoredProcedureScript += String.Format(", @{0} = '{1}'", "GroupBy", ddlGroupBy.SelectedValue)
            End If
            Return CreationStoredProcedureScript
        End If
    End Function

    Private Function DoesCacheResultNeeded() As Boolean
        Dim cacheResult As Boolean = False
        If ddlDateInterval.SelectedValue = "Dynamic" Then
            cacheResult = (ddlDynamicDateInterval.SelectedValue <> "Today") And Not (ddlDynamicDateInterval.SelectedValue = "InLast" And ddlDateUnits.SelectedValue = "1" And txtDateUnit.Value = 1)
        Else
            Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
            cacheResult = RadDateTimePickerTo.SelectedDate < fromDate
        End If
        Return cacheResult
    End Function
    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim chart As ClsInsightsHelper.Chart = New ClsInsightsHelper.Chart(0, txtBoxChartTitle.Text, ddlChartType.SelectedValue, txtBoxChartSubTitle.Text, txtBoxComments.Text)
        chart.CreationStoredProcedureName = GetChartCreationStoreProcedure()
        chart.ExcelSheetName = txtBoxExcelSheetName.Text
        Session("PreviewChart") = chart
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Init", "window.parent.PreviewChart();", True)
        ClsHelper.Log("Insights Preview Chart", ClsSessionHelper.LogonUser.GlobalID.ToString(), "CreationStoredProcedureName: " & chart.CreationStoredProcedureName, watch.ElapsedMilliseconds, False, Nothing)
    End Sub
    Protected Sub btnSaveOrUpdateChart_Click(sender As Object, e As EventArgs)
        Dim chartID As Integer = 0
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Integer.TryParse(Request.QueryString("cid"), chartID)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
        parameters.Add(New SqlParameter("@ChartID", chartID))
        parameters.Add(New SqlParameter("@Title", txtBoxChartTitle.Text))
        parameters.Add(New SqlParameter("@SubTitle", txtBoxChartSubTitle.Text))
        parameters.Add(New SqlParameter("@Comments", txtBoxComments.Text))
        parameters.Add(New SqlParameter("@TypeID", ddlChartType.SelectedValue))
        parameters.Add(New SqlParameter("@AreaID", ddlSections.SelectedValue))
        parameters.Add(New SqlParameter("@CreationStoredProcedureName", GetChartCreationStoreProcedure()))
        parameters.Add(New SqlParameter("@ExcelSheetName", txtBoxExcelSheetName.Text))
        parameters.Add(New SqlParameter("@IsShared", ddlSharingType.SelectedValue = "1"))
        parameters.Add(New SqlParameter("@CacheResult", DoesCacheResultNeeded()))
        Dim resultedChartID As Integer = ClsDataAccessHelper.ExecuteScalar("Insights.SaveOrUpdateChart", parameters)
        If resultedChartID > 0 Then
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "window.parent.ShowOrCloseChartWindow(false);", True)
            ClsHelper.Log("Insights " & IIf(chartID = 0, "Save", "Update") & " Chart", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Chart ID: " & resultedChartID, watch.ElapsedMilliseconds, False, Nothing)
        Else
            lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
            ClsHelper.Log("Insights " & IIf(chartID = 0, "Save", "Update") & " Chart", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Chart ID: " & resultedChartID, watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
        End If
    End Sub

    Protected Sub ddlSource_SelectedIndexChanged(sender As Object, e As EventArgs)
        ChangeControlsDisplayBySource(ddlSource.SelectedValue)
    End Sub

    Private Function GetSurveysByEnvironmentID(environmentID As Integer) As List(Of ClsInsightsHelper.Survey)
        Dim cacheObjectName As String = "Surveys_" + environmentID.ToString()
        If Cache(cacheObjectName) Is Nothing Then
            Cache(cacheObjectName) = ClsInsightsHelper.GetSuverys(environmentID)
        End If
        If Cache(cacheObjectName) IsNot Nothing Then
            Return DirectCast(Cache(cacheObjectName), List(Of ClsInsightsHelper.Survey))
        Else
            Return Nothing
        End If
    End Function

    Private Sub LoadSurveyLists(listOfSurveys As List(Of ClsInsightsHelper.Survey))
        Dim index As Integer = 0
        ddlSurvey.Items.Clear()
        For Each item As ClsInsightsHelper.Survey In listOfSurveys
            Dim ddItem As ListItem = New ListItem(item.Name, item.ID)
            If index = 0 Then
                ddItem.Selected = True
            End If
            ddlSurvey.Items.Add(ddItem)
            index += 1
        Next
        LoadQuestions(listOfSurveys)
    End Sub

    Private Sub LoadQuestions(listOfSurveys As List(Of ClsInsightsHelper.Survey))
        ddlSurveyQuestion.Items.Clear()
        ddlSurveyQuestion.Items.Add(New ListItem("All", Guid.Empty.ToString()))
        If ddlSurvey.SelectedValue IsNot Nothing Then
            For Each item As ClsInsightsHelper.SurveyQuestion In listOfSurveys.Where(Function(fc) fc.ID = ddlSurvey.SelectedValue).Single().Questions
                Dim ddItem As ListItem = New ListItem(item.Name, item.ID)
                ddlSurveyQuestion.Items.Add(ddItem)
            Next
        End If
    End Sub


    Private Sub ChangeControlsDisplayBySource(source As ClsInsightsHelper.InsightDataSourceType)
        trLogViewerActions.Visible = (source = ClsInsightsHelper.InsightDataSourceType.LogViewerApplication)
        trLogViewerActionStatus.Visible = (source = ClsInsightsHelper.InsightDataSourceType.LogViewerApplication)
        trLogViewerApplicationName.Visible = (source = ClsInsightsHelper.InsightDataSourceType.LogViewerApplication)
        trLogViewerGroupBy.Visible = (source = ClsInsightsHelper.InsightDataSourceType.LogViewerApplication)
        trSurveyEnvironment.Visible = (source = ClsInsightsHelper.InsightDataSourceType.SurveyResults)
        trSurveyQuestion.Visible = (source = ClsInsightsHelper.InsightDataSourceType.SurveyResults)
        trSurveySelection.Visible = (source = ClsInsightsHelper.InsightDataSourceType.SurveyResults)
        trFreeQuery.Visible = (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        trFreeQueryHelp.Visible = (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        trDateIntervalType.Visible = Not (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        trDateIntervalDynamic.Visible = ddlDateInterval.SelectedValue = "Dynamic" And Not (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        trDateIntervalStaticFrom.Visible = ddlDateInterval.SelectedValue = "Static" And Not (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        trDateIntervalStaticTo.Visible = ddlDateInterval.SelectedValue = "Static" And Not (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        pnlIntervalToSpecify.Visible = ddlDynamicDateInterval.SelectedValue = "InLast" And Not (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
        If ddlChartType.SelectedValue = 4 Then
            ddlChartType.SelectedValue = 1
        End If

        For Each radComboItem As RadComboBoxItem In ddlChartType.Items
            If radComboItem.Value = 4 Then
                radComboItem.Enabled = (source = ClsInsightsHelper.InsightDataSourceType.FreeQueryRecords)
            End If
        Next

    End Sub
    Protected Sub ddlSurveyEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlSurveyEnvironment.SelectedValue IsNot Nothing Then
            Dim listOfSurveys As List(Of ClsInsightsHelper.Survey) = GetSurveysByEnvironmentID(ddlSurveyEnvironment.SelectedValue)
            If listOfSurveys IsNot Nothing Then
                LoadSurveyLists(listOfSurveys)
            End If
        End If
    End Sub
    Protected Sub ddlSurvey_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlSurveyEnvironment.SelectedValue IsNot Nothing Then
            Dim listOfSurveys As List(Of ClsInsightsHelper.Survey) = GetSurveysByEnvironmentID(ddlSurveyEnvironment.SelectedValue)
            If listOfSurveys IsNot Nothing Then
                LoadQuestions(listOfSurveys)
            End If
        End If
    End Sub
End Class
