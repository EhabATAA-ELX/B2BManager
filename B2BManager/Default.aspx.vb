Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Data
Imports Telerik.Web.UI
Imports OfficeOpenXml
Imports OfficeOpenXml.Table.PivotTable

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Then
            If RenderApplicationDropDownList() Then
                RunSearch()
            End If
        Else
            InitAjaxSeetings()
        End If
        If chkBoxAutoSearch.Checked Then
            If Not String.IsNullOrEmpty(target) Then
                If Not target.ToLower().Contains("chkbox") And Not target.ToLower().Contains("gridsearch") And Not target.ToLower().Contains("btn") Then
                    RunSearch()
                Else
                    PopulateSearchGrid()
                End If
            Else
                PopulateSearchGrid()
            End If
        Else
            If target IsNot Nothing Then
                If target.Equals("") Then
                    PopulateSearchGrid()
                End If
            End If
        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            uniqueGeneratedKey.Value = DateTime.Now.GetHashCode().ToString("x") + "_" + Session.SessionID
            Me.Form.DefaultButton = btnSearch.UniqueID
            Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
            RadDateTimePickerFrom.SelectedDate = fromDate
            Dim toDate = fromDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)
            RadDateTimePickerTo.SelectedDate = toDate
            chkBoxAutoRefresh.Attributes.Add("onchange", "AutoRefresh(" & GetRefreshTimeInSeconds() & ")")
            If ConfigurationManager.AppSettings("GridAutoRefreshCheckedByDefault") IsNot Nothing Then
                If CBool(ConfigurationManager.AppSettings("GridAutoRefreshCheckedByDefault").ToString()) Then
                    chkBoxAutoRefresh.Attributes.Add("checked", "")
                End If
            End If
            InitAjaxSeetings()

            If ClsSessionHelper.LogonUser Is Nothing Then
                Response.Redirect("Login.aspx", True)
            Else
                btnExport.Visible = False
                If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
                    If ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.EXPORT_LOGS_IN_EXCEL) Then
                        btnExport.Visible = True
                    End If
                End If

                If ClsSessionHelper.LogonUser.Tools.Where(Function(fc) fc.ToolID = 15 And fc.TypeID = 1).Count() = 1 Then
                    btnCreateChart.Visible = True
                Else
                    btnCreateChart.Visible = False
                End If
            End If

        End If

        ShowElapsedTime = False
        ShowHasError = False
    End Sub

    Private Sub InitMinDate()
        Select Case ddlApplicationName.SelectedValue
            Case 1, 4
                Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
                RadDateTimePickerFrom.MinDate = fromDate.AddDays(-90)
                RadDateTimePickerTo.MinDate = fromDate.AddDays(-90)
            Case 2
                Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
                RadDateTimePickerFrom.MinDate = fromDate.AddYears(-5)
                RadDateTimePickerTo.MinDate = fromDate.AddYears(-5)
            Case Else
                RadDateTimePickerFrom.MinDate = New Date(2018, 1, 30) 'First day of log
                RadDateTimePickerTo.MinDate = New Date(2018, 1, 30) 'First day of log
        End Select
    End Sub

    Public Function GetDefaultAutoSearchLabel() As String
        Dim defaultAutoSearchLabel As String = "Auto Refresh:"
        If ConfigurationManager.AppSettings("GridAutoRefreshCheckedByDefault") IsNot Nothing Then
            If CBool(ConfigurationManager.AppSettings("GridAutoRefreshCheckedByDefault").ToString()) Then
                defaultAutoSearchLabel = "Auto Refresh after " & GetRefreshTimeInSeconds() & " seconds"
            End If
        End If
        Return defaultAutoSearchLabel
    End Function

    Protected Sub ddlApplicationName_SelectedIndexChanged(sender As Object, e As EventArgs)
        RenderApplicationDropDownList(ddlApplicationName.SelectedValue)
        If chkBoxAutoSearch.Checked Then
            RunSearch()
        End If
    End Sub

    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim parameter As SqlParameter
        If ddlApplicationName.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("ApplicationID", CInt(ddlApplicationName.SelectedValue.ToString())))
        End If

        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("EnvironementID", CInt(ddlEnvironment.SelectedValue)))
        End If

        If ddlCountry.SelectedValue <> "All" Then
            parameters.Add(New SqlParameter("SOP_IDs", ddlCountry.SelectedValue))
        End If

        If ddlAction.SelectedValue <> "0" Then
            parameters.Add(New SqlParameter("@ActionName", ddlAction.SelectedItem.Text))
        End If

        parameter = New SqlParameter("From", RadDateTimePickerFrom.SelectedDate)
        parameter.DbType = DbType.DateTime
        parameters.Add(parameter)

        parameter = New SqlParameter("To", RadDateTimePickerTo.SelectedDate)
        parameter.DbType = DbType.DateTime
        parameters.Add(parameter)

        If Not String.IsNullOrWhiteSpace(txtBoxSearchInDetails.Text) Then
            parameters.Add(New SqlParameter("SearchText", txtBoxSearchInDetails.Text.Trim()))
        End If

        If Not String.IsNullOrWhiteSpace(txtRowsCount.Text) Then
            parameters.Add(New SqlParameter("RowsCount", Integer.Parse(txtRowsCount.Text.Trim())))
        End If

        If ddlActionToDisplay.SelectedValue <> "0" Then
            parameters.Add(New SqlParameter("ActionsDisplay", Integer.Parse(ddlActionToDisplay.SelectedValue)))
        End If

        Return parameters
    End Function

    Private Function GetConnectionString() As String
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim selectedApplication As ClsHelper.Application = ClsHelper.FindApplicationByID(logonUser.Applications, CInt(ddlApplicationName.SelectedValue))
        Dim connectionString As String = Nothing
        If selectedApplication IsNot Nothing Then
            If selectedApplication.UseEnvironmentConnectionString Then
                connectionString = selectedApplication.Environments.Where(Function(f) f.ID = CInt(ddlEnvironment.SelectedValue)).First.ConnectionString
            End If
        End If
        Return connectionString
    End Function

    Public Property searchDs As DataTable
        Get

            If "true".Equals(ConfigurationManager.AppSettings("UseCacheForDataSearchInLogViewer")) Then
                If Cache("searchDs_" + uniqueGeneratedKey.Value) Is Nothing Then
                    Cache("searchDs_" + uniqueGeneratedKey.Value) = ClsDataAccessHelper.FillDataTable("Logger.GetActivityLogs", GetSearchParameters(), CommandType.StoredProcedure, GetConnectionString())
                End If
                Return CType(Cache("searchDs_" + uniqueGeneratedKey.Value), DataTable)
            Else
                If Session("searchDs_" + uniqueGeneratedKey.Value) Is Nothing Then
                    Session("searchDs_" + uniqueGeneratedKey.Value) = ClsDataAccessHelper.FillDataTable("Logger.GetActivityLogs", GetSearchParameters(), CommandType.StoredProcedure, GetConnectionString())
                End If
                Return CType(Session("searchDs_" + uniqueGeneratedKey.Value), DataTable)
            End If
        End Get
        Set(value As DataTable)
            If "true".Equals(ConfigurationManager.AppSettings("UseCacheForDataSearchInLogViewer")) Then
                If value Is Nothing Then
                    If Cache.Get("searchDs_" + uniqueGeneratedKey.Value) IsNot Nothing Then
                        Cache.Remove("searchDs_" + uniqueGeneratedKey.Value)
                    End If
                End If
            Else
                Session("searchDs_" + uniqueGeneratedKey.Value) = value
            End If
        End Set
    End Property


    Private Sub RefershSearchDataSource()
        searchDs = Nothing
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If
        Dim CanViewXMLFiles As Boolean = False

        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
            If ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.VIEW_XML_FILES) Then
                CanViewXMLFiles = True
            End If
        End If

        Dim selectedApplication As ClsHelper.Application = ClsHelper.FindApplicationByID(applications, CInt(ddlApplicationName.SelectedValue))
        gridSearch.MasterTableView.GetColumn("CustomerCode").Visible = selectedApplication.ShowCustomerCodeColumn
        gridSearch.MasterTableView.GetColumn("CORREL_ID").Visible = selectedApplication.ShowCorrelIDColumn
        CheckPOIDColumnDisplay = selectedApplication.ShowPOIDColumn
        gridSearch.MasterTableView.GetColumn("POID").Visible = selectedApplication.ShowPOIDColumn
        gridSearch.MasterTableView.GetColumn("SALESORDERID").Visible = selectedApplication.ShowSalesOrderIDColumn
        gridSearch.MasterTableView.GetColumn("HubspanId").Visible = selectedApplication.ShowHubspanIdColumn
        gridSearch.MasterTableView.GetColumn("ReceivedDate").Visible = selectedApplication.ShowDateReceivedColumn
        CheckSalesOrderIDColumnDisplay = selectedApplication.ShowSalesOrderIDColumn
        gridSearch.MasterTableView.GetColumn("ViewXMLWithCorrelID").Visible = selectedApplication.ShowViewMessageXMLColumn AndAlso selectedApplication.ShowCorrelIDColumn AndAlso selectedApplication.IsMessageXML AndAlso CanViewXMLFiles
        gridSearch.MasterTableView.GetColumn("ViewJsonWithCorrelID").Visible = selectedApplication.ShowViewMessageXMLColumn AndAlso selectedApplication.ShowCorrelIDColumn AndAlso selectedApplication.IsMessageXML AndAlso CanViewXMLFiles
        gridSearch.MasterTableView.GetColumn("ViewXML").Visible = selectedApplication.ShowViewMessageXMLColumn AndAlso Not selectedApplication.ShowCorrelIDColumn AndAlso selectedApplication.IsMessageXML AndAlso CanViewXMLFiles
        gridSearch.MasterTableView.GetColumn("ViewJson").Visible = selectedApplication.ShowViewMessageXMLColumn AndAlso Not selectedApplication.ShowCorrelIDColumn AndAlso Not selectedApplication.IsMessageXML AndAlso CanViewXMLFiles
    End Sub

    Private Sub PopulateSearchGrid()
        gridSearch.DataSource = searchDs
        gridSearch.DataBind()
        If searchDs IsNot Nothing AndAlso ddlApplicationName.SelectedItem IsNot Nothing AndAlso ddlEnvironment.SelectedItem IsNot Nothing Then
            If searchDs.Rows.Count = 0 Then
                lblInformation.InnerHtml = "No results found, please check your filters"
            Else
                lblInformation.InnerHtml = "Showing <span class='information-label-text'>" & IIf(searchDs.Rows.Count < txtRowsCount.Value, searchDs.Rows.Count, "top " & txtRowsCount.Text.Trim()) &
                                        "</span> row" & IIf(searchDs.Rows.Count > 1, "s", "") & " for <span class='information-label-text'>" & ddlApplicationName.SelectedItem.Text & "</span>" &
                                        IIf(ddlEnvironment.SelectedItem.Text.ToLower().Contains("applicable"), "", " in <span class='information-label-text'>" + ddlEnvironment.SelectedItem.Text + "</span>")
            End If

        Else
            lblInformation.InnerHtml = ""
        End If
        gridSearch.MasterTableView.GetColumn("ElapsedTime").Visible = ShowElapsedTime
        gridSearch.MasterTableView.GetColumn("HasError").Visible = ShowHasError
        gridSearch.MasterTableView.GetColumn("ID").Visible = ShowIDColumn
        gridSearch.MasterTableView.GetColumn("U_Login").Visible = ShowLoginColumn
        If CheckPOIDColumnDisplay Then
            gridSearch.MasterTableView.GetColumn("POID").Visible = ShowPOIDColumn
        End If
        If CheckSalesOrderIDColumnDisplay Then
            gridSearch.MasterTableView.GetColumn("SALESORDERID").Visible = ShowSalesOrderIDColumn
        End If
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


    Protected ShowElapsedTime As Boolean
    Protected ShowIDColumn As Boolean
    Protected ShowHasError As Boolean
    Protected ShowPOIDColumn As Boolean
    Protected ShowSalesOrderIDColumn As Boolean
    Protected ShowLoginColumn As Boolean
    Protected CheckPOIDColumnDisplay As Boolean
    Protected CheckSalesOrderIDColumnDisplay As Boolean

    Protected Sub gridSearch_ItemDataBound(sender As Object, e As GridItemEventArgs)
        Try
            If TypeOf e.Item Is GridDataItem Then
                Dim item As GridDataItem = e.Item
                ShowElapsedTime = ShowElapsedTime Or item.DataItem.row.Item("ElapsedTime") IsNot DBNull.Value
                ShowHasError = ShowHasError Or item.DataItem.row.Item("HasError") IsNot DBNull.Value
                ShowIDColumn = ShowIDColumn Or item.DataItem.row.Item("ID") IsNot DBNull.Value
                ShowLoginColumn = ShowLoginColumn Or item.DataItem.row.Item("UserID") IsNot DBNull.Value Or item.DataItem.row.Item("U_LOGIN") IsNot DBNull.Value
                If CheckPOIDColumnDisplay Then
                    ShowPOIDColumn = ShowPOIDColumn Or item.DataItem.row.Item("POID") IsNot DBNull.Value
                End If
                If CheckSalesOrderIDColumnDisplay Then
                    ShowSalesOrderIDColumn = ShowSalesOrderIDColumn Or item.DataItem.row.Item("SALESORDERID") IsNot DBNull.Value
                End If
                Me.RadToolTipManager1.TargetControls.Add("UserImgTooltip_" + item.DataItem.row.Item("TootlipID").ToString(), True)
                Me.RadToolTipManager1.TargetControls.Add("MachineImgTooltip_" + item.DataItem.row.Item("TootlipID").ToString(), True)
                Me.RadToolTipManager1.TargetControls.Add("ActionImgTooltip_" + item.DataItem.row.Item("TootlipID").ToString(), True)
                If Not item.DataItem.row.Item("HasError") Is DBNull.Value Then
                    If CBool(item.DataItem.row.Item("HasError")) Then
                        Me.RadToolTipManager1.TargetControls.Add("StatusImgTooltip_" + item.DataItem.row.Item("TootlipID").ToString(), True)
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle ItemDataBound exception
        End Try
    End Sub

    Private Sub RunSearch()
        Try
            Dim watch As Stopwatch = Stopwatch.StartNew()
            RefershSearchDataSource()
            PopulateSearchGrid()
            ConfigureAutoRefresh()
            watch.Stop()
            ClsHelper.Log("Run search", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)

        Catch ex As Exception
            Dim errorMessage As String = ex.Message
        End Try
    End Sub

    Public Function GetRefreshTimeInSeconds() As Integer
        Dim TimeToRefresh As Integer = 120
        If ConfigurationManager.AppSettings("GridAutoRefreshTimeInSeconds") IsNot Nothing Then
            Integer.TryParse(ConfigurationManager.AppSettings("GridAutoRefreshTimeInSeconds").ToString(), TimeToRefresh)
        End If
        Return TimeToRefresh
    End Function

    Private Sub ConfigureAutoRefresh()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Start AutoRefresh", "AutoRefresh(" & GetRefreshTimeInSeconds() & ");", True)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        RunSearch()
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Using excel As ExcelPackage = New ExcelPackage()
            AddDataToExcel(excel, "Activity Logs")
            Response.BinaryWrite(excel.GetAsByteArray())
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("content-disposition", String.Format("attachment;  filename=ActivityLog_{0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy HH:mm")))
            Response.[End]()
        End Using
        watch.Stop()
        ClsHelper.Log("Export Results", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Private Sub AddDataToExcel(ByRef excel As ExcelPackage, workSheetName As String)
        Try
            ' Add data worksheet
            Dim ws As ExcelWorksheet = excel.Workbook.Worksheets.Add(workSheetName)
            Dim applications As List(Of ClsHelper.Application) = Nothing
            If ClsSessionHelper.LogonUser Is Nothing Then
                Response.Redirect("Login.aspx", True)
                Return
            Else
                applications = ClsSessionHelper.LogonUser.Applications
            End If
            Dim selectedApplication As ClsHelper.Application = ClsHelper.FindApplicationByID(applications, CInt(ddlApplicationName.SelectedValue))
            Dim clonedSearchDt As DataTable = searchDs.Copy()
            Dim exportTemplateColumns As List(Of ClsHelper.ExportTemplate) = GetExportTemplate()
            Dim loggedOnIndex As Integer = 0
            Dim actionNameIndex As Integer = 0
            Dim currentIndex As Integer = 0

            RemoveColumn(clonedSearchDt, "CORREL_ID", Not selectedApplication.ShowCorrelIDColumn)
            RemoveColumn(clonedSearchDt, "POID", Not selectedApplication.ShowPOIDColumn)
            RemoveColumn(clonedSearchDt, "SALESORDERID", Not selectedApplication.ShowSalesOrderIDColumn)
            RemoveColumn(clonedSearchDt, "CustomerCode", Not selectedApplication.ShowCustomerCodeColumn)

            For Each column As DataColumn In searchDs.Columns
                If clonedSearchDt.Columns.Contains(column.ColumnName) Then
                    Dim exportColumnTemplate As ClsHelper.ExportTemplate = exportTemplateColumns.Where(Function(fn) (fn.ColumnFieldName = column.ColumnName)).SingleOrDefault()
                    If exportColumnTemplate IsNot Nothing Then
                        clonedSearchDt.Columns(column.ColumnName).ColumnName = exportColumnTemplate.ColumnName
                        currentIndex += 1
                        If column.ColumnName.StartsWith("LoggedOn") Then
                            loggedOnIndex = currentIndex
                        End If
                        If column.ColumnName.StartsWith("ActionName") Then
                            actionNameIndex = currentIndex
                        End If
                    Else
                        clonedSearchDt.Columns.Remove(column.ColumnName)
                    End If
                End If
            Next
            ws.Cells("A1").LoadFromDataTable(clonedSearchDt, True)
            ws.Cells("A1:" & ClsHelper.GetExcelColumnName(clonedSearchDt.Columns.Count) & "1").Style.Font.Bold = True
            ws.Cells("A1:" & ClsHelper.GetExcelColumnName(clonedSearchDt.Columns.Count) & "1").AutoFilter = True
            ws.Cells.AutoFitColumns()
            ' Add pivot worksheet
            Dim wsPivot As ExcelWorksheet = excel.Workbook.Worksheets.Add("Pivot")
            Dim pivotTable As ExcelPivotTable = wsPivot.PivotTables.Add(wsPivot.Cells("B2"), ws.Cells("A1:" & ClsHelper.GetExcelColumnName(clonedSearchDt.Columns.Count) & clonedSearchDt.Rows.Count + 1), "PivotTable")
            If actionNameIndex > 0 AndAlso loggedOnIndex > 0 Then
                pivotTable.RowFields.Add(pivotTable.Fields("Action Name"))
                pivotTable.DataOnRows = False

                Dim field = pivotTable.DataFields.Add(pivotTable.Fields("Logged On"))
                field.Name = "Count of Actions"
                field.Function = DataFieldFunctions.Count
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>AddDataToExcel (main)</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

    Private Sub RemoveColumn(ByRef dataTable As DataTable, columnName As String, RemoveColumn As Boolean)
        If RemoveColumn Then
            If dataTable.Columns.Contains(columnName) Then
                dataTable.Columns.Remove(columnName)
            End If
        End If
    End Sub

    Private Function GetExportTemplate() As List(Of ClsHelper.ExportTemplate)
        If Cache("ExportTemplate") Is Nothing Then
            Dim exportTemplateList As List(Of ClsHelper.ExportTemplate) = New List(Of ClsHelper.ExportTemplate)()
            Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Logger.GetExportTemplate")
            If dataTable IsNot Nothing Then
                For Each row As DataRow In dataTable.Rows
                    exportTemplateList.Add(New ClsHelper.ExportTemplate(row("ColumnFieldName"), row("ColumnName")))
                Next
            End If
            Cache.Insert("ExportTemplate", exportTemplateList)
        End If

        Return DirectCast(Cache("ExportTemplate"), List(Of ClsHelper.ExportTemplate))
    End Function


    Private Function RenderApplicationDropDownList(Optional ByVal selectedValue As String = Nothing) As Boolean
        Dim activeApplications As List(Of ClsHelper.BasicModel) = New List(Of ClsHelper.BasicModel)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If

        If applications IsNot Nothing Then
            If applications.Where(Function(fn) (fn.Checked)).Count > 0 Then
                For Each application As ClsHelper.Application In applications
                    If application.Checked = True Then
                        activeApplications.Add(application)
                    End If
                Next
                ClsHelper.RenderDropDownList(ddlApplicationName, activeApplications, True)
                If selectedValue Is Nothing Then
                    ddlApplicationName.SelectedIndex = 0
                    selectedValue = ddlApplicationName.SelectedValue
                Else
                    ddlApplicationName.SelectedValue = selectedValue
                End If
                Dim selectedApplication = ClsHelper.FindApplicationByID(applications, selectedValue)
                ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True)
                RenderActionDropDownList(selectedApplication.Actions, True, selectedApplication.AppendAllItemInActions)
                ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ddlCountry.SelectedValue, selectedApplication.SelectAllCountriesByDefault)
                InitMinDate()
                Return True
            End If
        End If

        infoLabel.Visible = True
        gridSearch.Visible = False
        filtersTable.Visible = False
        Return False
    End Function

    Private Sub AddAjaxSetting(ByVal controlID As String)
        Dim ajaxSettings As AjaxSetting = New AjaxSetting()
        ajaxSettings.AjaxControlID = controlID
        ajaxSettings.UpdatedControls.Add(New AjaxUpdatedControl("gridSearch", "RadAjaxLoadingPanel1"))
        RadAjaxManager1.AjaxSettings.Add(ajaxSettings)
    End Sub

    Private Sub InitAjaxSeetings()
        RadAjaxManager1.AjaxSettings.Clear()
        AddAjaxSetting("btnSearch")
        AddAjaxSetting("chkBoxAutoSearch")
        AddAjaxSetting("gridSearch")
        If chkBoxAutoSearch.Checked Then
            AddAjaxSetting("ddlApplicationName")
            AddAjaxSetting("ddlEnvironment")
            AddAjaxSetting("ddlCountry")
            AddAjaxSetting("ddlAction")
            AddAjaxSetting("ddlActionToDisplay")
            AddAjaxSetting("txtRowsCount")
            AddAjaxSetting("RadDateTimePickerFrom")
            AddAjaxSetting("RadDateTimePickerTo")
            AddAjaxSetting("txtBoxSearchInDetails")
        End If
    End Sub

    Protected Sub chkBoxAutoSearch_CheckedChanged(sender As Object, e As EventArgs)
        ddlEnvironment.AutoPostBack = chkBoxAutoSearch.Checked
        ddlCountry.AutoPostBack = chkBoxAutoSearch.Checked
        ddlAction.AutoPostBack = chkBoxAutoSearch.Checked
        ddlActionToDisplay.AutoPostBack = chkBoxAutoSearch.Checked
        txtRowsCount.AutoPostBack = chkBoxAutoSearch.Checked
        RadDateTimePickerFrom.AutoPostBack = chkBoxAutoSearch.Checked
        RadDateTimePickerTo.AutoPostBack = chkBoxAutoSearch.Checked
        txtBoxSearchInDetails.AutoPostBack = chkBoxAutoSearch.Checked
        InitAjaxSeetings()
    End Sub
    Protected Sub gridSearch_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        gridSearch.DataSource = searchDs
    End Sub
End Class
