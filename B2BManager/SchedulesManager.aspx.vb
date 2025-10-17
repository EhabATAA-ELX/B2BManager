Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI
Imports ClsHelper

Partial Class SchedulesManager
    Inherits Page

    Protected Function GetActionRights() As String
        Dim scriptTemplate As String = "var ADD_SCHEDULE_B2B = {0};var DELETE_SCHEDULE_B2B = {1};var EDIT_SCHEDULE_B2B = {2};"
        Dim script As String = ""
        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
            Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
            script = String.Format(scriptTemplate,
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.ADD_SCHEDULE_B2B), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.DELETE_SCHEDULE_B2B), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.EDIT_SCHEDULE_B2B), "true", "false"))
        Else
            script = String.Format(scriptTemplate, "false", "false", "false")
        End If
        Return script
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            If (__EVENTTARGET.EndsWith("UpdatePanel1")) Then
                BindData()
                UpdatePanel1.Update()
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
            End If
        Else
            RenderControls()
            BindData()
            UpdatePanel1.Update()
            ClsHelper.Log("Load B2B Translations", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
            ClsHelper.Log("Load Schedules", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Protected Function GetEditTemplate() As String
        Dim EditTemplate As String = ""
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim CanEdit As Boolean = False
        If logonUser.Actions.Count > 0 Then
            If logonUser.Actions.Contains(ActionDesignation.EDIT_LOCAL_TRANSLATION_VALUE) Then
                CanEdit = True
                If Not IsPostBack Then
                    HD_EditLOCALVALUE.Value = "1"
                End If
            End If

            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_COMMENTS_AND_DEFAULT_TRANSLATION_VALUE) Then
                CanEdit = True
                If Not IsPostBack Then
                    HD_EDITALL.Value = "1"
                End If
            End If
        End If

        If CanEdit Then
            EditTemplate = "<img id=""#data#""  src=""Images/Edit.png"" title=""Edit Translation"" class=""MoreInfoImg"" onclick=""GetEditRow(this)""  width=""20"" height=""20"">"
        Else
            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_UNASSIGN_TRANSLATION_AREA) Then
                TableHeaderCell0.Text = "Manage Area(s)"
            Else
                TableHeaderCell0.Text = "Display Area(s)"
            End If
        End If
        Return EditTemplate
    End Function

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)
        GetAreaByEnv()
        GetLanguagesBycountry()
        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
        btnAddSchedules.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_SCHEDULE_B2B)
    End Sub


    Private Sub LoadGrid()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadGrid", "LoadGrid();", True)
    End Sub
    Public Sub GetAreaByEnv()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", ddlEnvironment.SelectedValue))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_GetAreasByEnv", parameters)
        ddlArea.Items.Insert(0, New ListItem("All", "0"))
        For Each row As DataRow In dataTable.Rows
            ddlArea.Items.Add(New ListItem(row("TA_Name").ToString(), row("TA_AreaID").ToString()))
        Next
        ddlArea.SelectedIndex = 0
    End Sub

    Public Sub GetLanguagesBycountry()
        Dim dt = ClsEbusinessHelper.GetLanguages(ddlEnvironment.SelectedValue, Page.Cache)
        If dt IsNot Nothing Then
            Dim dr As DataRow() = dt.Select("S_SOP_ID='" + ddlCountry.SelectedValue + "'")
            If dr.Length = 1 Then
                HdLanguage.Value = dr(0)("LANG_ISOCODE")
                ddlLanguage.Visible = False
                lblLanguage.Visible = False
            Else
                ddlLanguage.Items.Clear()
                ddlLanguage.Visible = True
                lblLanguage.Visible = True
                For Each row As DataRow In dr
                    ddlLanguage.Items.Add(New ListItem(row("LANG_NAME").ToString(), row("LANG_ISOCODE").ToString()))
                Next
            End If
        End If
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        LoadGrid()
    End Sub
    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        GetLanguagesBycountry()
        LoadGrid()
    End Sub
    Protected Sub ddlArea_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        LoadGrid()
    End Sub
    Protected Sub ddlLanguage_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        LoadGrid()
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
                ClsSessionHelper.EbusinessSopID = ddlCountry.SelectedValue
            End If
        Catch ex As Exception
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
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

    Private Sub BindData()
        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[Maintenance].[GetScheduledWarningBySop]", cnx)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
            cmd.Parameters.AddWithValue("@SopName", ddlCountry.SelectedValue)
            cmd.CommandType = CommandType.StoredProcedure
            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")

            Dim i As Integer = 0
            For Each dr In ds.Tables(0).Rows
                Dim trow As New TableRow

                trow.ClientIDMode = ClientIDMode.Static
                trow.ID = "Line_" + dr("GlobalID").ToString()
                For Each dc In ds.Tables(0).Columns
                    Dim tcell As New TableCell
                    If (i = 0) Then
                        Dim toolsCount As Integer = 0
                        Dim actionsHeader As String = ""
                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_SCHEDULE_B2B) Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/edit.png"
                            img.Attributes.Add("onclick", "EditRow('" + trow.ID + "');")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "Edit")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            toolsCount += 1
                            actionsHeader = "Edit"
                        End If

                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SCHEDULE_B2B) Then
                            toolsCount += 1
                            actionsHeader = "Delete"
                        End If
                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SCHEDULE_B2B) Then
                            Dim img2 As New HtmlImage()
                            img2.Src = "Images/delete.png"
                            img2.Attributes.Add("onclick", "Delete('" + dr("GlobalID").ToString() + "','" + ddlCountry.SelectedValue + "','" + ddlEnvironment.SelectedValue + "');")
                            img2.Attributes.Add("class", "width20px LineChartImg")
                            img2.Attributes.Add("title", "Delete schedule")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img2)
                        End If
                        Select Case toolsCount
                        Case 0
                            TableHeaderCell0.Text = ""
                        Case 1
                            TableHeaderCell0.Text = actionsHeader
                    End Select
                    trow.Cells.Add(tcell)
                        End If
                    tcell = New TableCell()
                    tcell.Controls.Add(New LiteralControl(dr(dc.ColumnName).ToString))
                    tcell.Attributes.Add("data", dr(dc.ColumnName).ToString)
                    
                    trow.Cells.Add(tcell)
                    i += 1
                Next
                i = 0
                B2BSchedules.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    Protected Sub btnAdd_ClickSchedule(sender As Object, e As EventArgs)
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
            Return
        Else
            Try
                Dim cnx As SqlConnection
                Dim cmd As SqlCommand

                cnx = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                cnx.Open()
                cmd = New SqlCommand("[Maintenance].[SetScheduledWarningBySop]", cnx)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
                cmd.Parameters.AddWithValue("@SopName", ddlCountry.SelectedValue)
                cmd.Parameters.AddWithValue("@ScheduleStart", Convert.ToDateTime(ScheduleStartTxt.Value))
                cmd.Parameters.AddWithValue("@ScheduleEnd", Convert.ToDateTime(ScheduleEndTxt.Value))
                cmd.ExecuteNonQuery()

            Catch ex As Exception
                Throw ex
            End Try
        End If
        ScheduleStartTxt.Value = Nothing
        ScheduleEndTxt.Value = Nothing
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Finish", "CloseWindow();RefreshTable();", True)
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteLine(EnvironmentID As String, SopId As String, ID As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@ID", ID))
            If ClsDataAccessHelper.ExecuteNonQuery("[Maintenance].[DeleteScheduledWarningBySop]", parameters) Then
                LogEbusinessAction(EnvironmentID, SopId, "Delete schedule field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                LogEbusinessAction(EnvironmentID, SopId, "Delete schedule field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLine(EnvironmentID As String, SopId As String, ID As String, StartSchedule As String, EndSchedule As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SOPID", SopId))
            parameters.Add(New SqlParameter("@ID", ID))
            parameters.Add(New SqlParameter("@ScheduleStart", StartSchedule))
            parameters.Add(New SqlParameter("@ScheduleEnd", EndSchedule))

            If ClsDataAccessHelper.ExecuteNonQuery("[Maintenance].[EditScheduledWarningBySop]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

End Class
