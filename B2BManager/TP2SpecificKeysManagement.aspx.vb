
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Web.Script.Services
Imports System.Web.Services
Imports Telerik.Web.UI

Partial Class TP2SpecificKeysManagement
    Inherits System.Web.UI.Page


    Protected Function GetActionRights() As String
        Dim scriptTemplate As String = "var EDIT_SMS_LOCAL_VALUE = {0};var EDIT_SMS_DEFAULT_AND_COMMENT_VALUES = {1};var CHANGE_SMS_TYPE = {2};var DELETE_SMS_KEY = {3};var ADD_SMS_KEY = {4};var DISPLAY_ALL_SMS_TYPES = {5}"
        Dim script As String = ""
        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
            Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
            script = String.Format(scriptTemplate,
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.EDIT_TP2_SMS_LOCAL_VALUE), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.EDIT_TP2_SMS_DEFAULT_AND_COMMENT_VALUES), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.CHANGE_TP2_SMS_TYPE), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.DELETE_TP2_SMS_KEY), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.ADD_TP2_SMS_KEY), "true", "false"),
                                   IIf(actions.Contains(ClsHelper.ActionDesignation.DISPLAY_ALL_TP2_SMS_TYPES), "true", "false"))
        Else
            script = String.Format(scriptTemplate, "false", "false", "false", "false", "false", "false")
        End If
        Return script
    End Function


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            If (__EVENTTARGET.EndsWith("SmsKeyUpdatePanel")) Then
                BindData()
                SmsKeyUpdatePanel.Update()
                ScriptManager.RegisterStartupScript(SmsKeyUpdatePanel, SmsKeyUpdatePanel.GetType(), "BindDataTable", "BindDataTable();", True)
            End If
        Else
            RenderControls()
            BindData()
            SmsKeyUpdatePanel.Update()
            ClsHelper.Log("Load TP2 Specific Management Keys", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)
        RenderDropDownListType()
        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
        btnAddSmsKey.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_TP2_SMS_KEY)
        'I didn't have the good right so I put true in prod we use top line
        'btnAddSmsKey.Visible = True

    End Sub

    Private Sub RenderDropDownListType()

        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions

        If (actions.Contains(ClsHelper.ActionDesignation.DISPLAY_ALL_TP2_SMS_TYPES)) Then
            'If (True) Then
            ddlType.Items.Insert(0, New ListItem("All", "All"))
            ddlType.Items.Insert(1, New ListItem("Functional (F)", "F"))
            ddlType.Items.Insert(2, New ListItem("Technical (T)", "T"))
        Else
            ddlType.Items.Insert(0, New ListItem("Functional (F)", "F"))
            lblTypeSearch.Visible = False
            ddlType.Visible = False
        End If

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
            cmd = New SqlClient.SqlCommand("[Ebusiness].[SmsMgmt_GetKeys]", cnx)
            cmd.Parameters.AddWithValue("@SOPID", ddlCountry.SelectedValue)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
            If (ddlType.SelectedValue <> "" And ddlType.SelectedValue <> "All") Then
                cmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
            End If
            cmd.CommandType = CommandType.StoredProcedure
            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")

            Dim i As Integer = 0
            For Each dr In ds.Tables(0).Rows
                Dim trow As New TableRow

                trow.ClientIDMode = ClientIDMode.Static
                trow.ID = "Line_" + dr("KeyName").ToString()
                For Each dc In ds.Tables(0).Columns
                    Dim tcell As New TableCell
                    If (dc.ColumnName.ToString <> "IsBoolean") Then
                        If (i = 0) Then
                            Dim toolsCount As Integer = 0
                            Dim actionsHeader As String = ""
                            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_TP2_SMS_DEFAULT_AND_COMMENT_VALUES) Or logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_TP2_SMS_LOCAL_VALUE) Then
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
                            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_ALL_TP2_SMS_TYPES) Then
                                Dim img As New HtmlImage()
                                img.Src = "Images/magnifyingglass.png"
                                img.Attributes.Add("onclick", "ViewAllCountry('" + dr("KeyName").ToString() + "');")
                                img.Attributes.Add("class", "width20px LineChartImg")
                                img.Attributes.Add("title", "View values in all countries")
                                tcell.Attributes.Add("class", "TextAlignCenter")
                                tcell.Controls.Add(img)
                                toolsCount += 1
                                actionsHeader = "Display"
                            End If

                            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_TP2_SMS_KEY) Then
                                toolsCount += 1
                                actionsHeader = "Delete"
                            End If
                            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_TP2_SMS_KEY) And dr("CountryValue").ToString <> "" Then
                                Dim img As New HtmlImage()
                                img.Src = "Images/delete.png"
                                img.Attributes.Add("onclick", "Delete('" + dr("KeyName").ToString() + "','" + ddlCountry.SelectedValue + "','" + ddlEnvironment.SelectedValue + "');")
                                img.Attributes.Add("class", "width20px LineChartImg")
                                img.Attributes.Add("title", "Delete country value")
                                tcell.Attributes.Add("class", "TextAlignCenter")
                                tcell.Controls.Add(img)
                            Else
                                If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_TP2_SMS_KEY) Then
                                    Dim img As New HtmlImage()
                                    img.Src = "Images/delete.png"
                                    img.Attributes.Add("class", "width20px ImgDisabled")
                                    img.Attributes.Add("title", "Delete country value")
                                    tcell.Attributes.Add("class", "TextAlignCenter")
                                    tcell.Controls.Add(img)
                                End If
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

                        If (dc.ColumnName.ToString = "CountryValue") Then
                            tcell.ClientIDMode = ClientIDMode.Static
                            tcell.ID = dr("KeyName").ToString() + "_CountryValue"
                        End If
                        tcell.Controls.Add(New LiteralControl(dr(dc.ColumnName).ToString))
                        tcell.Attributes.Add("data", dr(dc.ColumnName).ToString)
                        trow.Cells.Add(tcell)
                        i += 1
                    End If
                Next
                i = 0
                JQueryDataTable.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLine(EnvironmentID As String, SopId As String, KeyName As String, Comment As String, DefaultValue As String, CountryValue As String, Type As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@SOPID", SopId))
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@Key", KeyName))
            parameters.Add(New SqlParameter("@Value", CountryValue))
            parameters.Add(New SqlParameter("@DefaultValue", DefaultValue))
            parameters.Add(New SqlParameter("@Comment", Comment))
            parameters.Add(New SqlParameter("@Type", Type))

            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[SmsMgmt_UpdateKeyFields]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save TP2 Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save TP2 Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteLine(EnvironmentID As String, SopId As String, KeyName As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@SOPID", SopId))
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@Key", KeyName))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[SmsMgmt_DeleteCountryValue]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Delete TP2 Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Delete TP2 Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    Protected Sub ddl_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        SmsKeyUpdatePanel.Update()
        ScriptManager.RegisterStartupScript(SmsKeyUpdatePanel, SmsKeyUpdatePanel.GetType(), "BindDataTable", "BindDataTable();", True)
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

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
            Return
        Else
            Try
                Dim cnx As SqlConnection
                Dim cmd As SqlCommand

                cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                cnx.Open()
                cmd = New SqlClient.SqlCommand("[Ebusiness].[SmsMgmt_InsertKeyFields]", cnx)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
                cmd.Parameters.AddWithValue("@Key", KeyNameTxt.Value)
                cmd.Parameters.AddWithValue("@DefaultValue", DefautValueTxt.Value)
                cmd.Parameters.AddWithValue("@Comment", CommentTxt.Value)
                cmd.Parameters.AddWithValue("@Type", type.Value)
                cmd.ExecuteNonQuery()
                BindData()
                KeyNameTxt.Value = ""
                DefautValueTxt.Value = ""
                CommentTxt.Value = ""
                type.SelectedIndex = 0
                SmsKeyUpdatePanel.Update()
                ScriptManager.RegisterStartupScript(SmsKeyUpdatePanel, SmsKeyUpdatePanel.GetType(), "Finish", "CloseWindow();BindDataTable();", True)
            Catch ex As Exception
                Throw ex
            End Try
        End If
    End Sub


End Class
