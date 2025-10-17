Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI
Imports ClsHelper
Partial Public Class SurveyManager
    Inherits System.Web.UI.Page

    Protected Function GetActionRights() As String
        Dim scriptTemplate As String = "var ADD_SURVEY = {0};var DELETE_SURVEY = {1};var EDIT_SURVEY = {2};var TRANSLATE_SURVEY = {3};"
        Dim script As String = ""
        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
            Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
            script = String.Format(scriptTemplate,
                                       IIf(actions.Contains(ClsHelper.ActionDesignation.ADD_SURVEY), "true", "false"),
                                       IIf(actions.Contains(ClsHelper.ActionDesignation.DELETE_SURVEY), "true", "false"),
                                       IIf(actions.Contains(ClsHelper.ActionDesignation.EDIT_SURVEY), "true", "false"),
                                       IIf(actions.Contains(ClsHelper.ActionDesignation.TRANSLATE_SURVEY), "true", "false"))
        Else
            script = String.Format(scriptTemplate, "false", "false", "false")
        End If
        Return script
    End Function
    Private Sub BindData()
        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[dbo].[GetAllSurvey]", cnx)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
            cmd.CommandType = CommandType.StoredProcedure
            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")

            Dim i As Integer = 0
            For Each dr In ds.Tables(0).Rows
                Dim trow As New TableRow

                trow.ClientIDMode = ClientIDMode.Static
                trow.ID = "Line_" + dr("ID").ToString()
                For Each dc In ds.Tables(0).Columns
                    Dim tcell As New TableCell
                    If (i = 0) Then
                        Dim toolsCount As Integer = 0
                        Dim actionsHeader As String = ""
                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_SURVEY) Then
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

                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_SURVEY Or ClsHelper.ActionDesignation.TRANSLATE_SURVEY) Then
                            Dim stringTest As String = "<form method='post' action='QuestionAndAnswerSurveyManager'"
                            Dim img As New HtmlImage()
                            img.Src = "Images/magnifyingglass.png"
                            img.Attributes.Add("value", ID)
                            img.Attributes.Add("onclick", "ViewDetail('" + trow.ID + "');;")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "View")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            toolsCount += 1
                            actionsHeader = "Edit"
                        End If

                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SURVEY) Then
                            toolsCount += 1
                            actionsHeader = "Delete"
                        End If
                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SURVEY) Then
                            Dim img2 As New HtmlImage()
                            img2.Src = "Images/delete.png"
                            img2.Attributes.Add("onclick", "Delete('" + dr("ID").ToString() + "','" + ddlCountry.SelectedValue + "','" + ddlEnvironment.SelectedValue + "');")
                            img2.Attributes.Add("class", "width20px LineChartImg")
                            img2.Attributes.Add("title", "Delete survey")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img2)
                        Else
                            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SURVEY) Then
                                Dim img3 As New HtmlImage()
                                img3.Src = "Images/delete.png"
                                img3.Attributes.Add("class", "width20px ImgDisabled")
                                img3.Attributes.Add("title", "Delete country value")
                                tcell.Attributes.Add("class", "TextAlignCenter")
                                tcell.Controls.Add(img3)
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

                    tcell.Controls.Add(New LiteralControl(dr(dc.ColumnName).ToString))
                    tcell.Attributes.Add("data", dr(dc.ColumnName).ToString)


                    trow.Cells.Add(tcell)
                    i += 1
                Next
                i = 0
                B2BSurveys.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)

        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
        btnAddSurveys.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_SURVEY)
    End Sub

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
            ClsHelper.Log("Load Surveys", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
    End Sub
    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
    End Sub
    Protected Sub ddlLanguage_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
    End Sub

    Protected Sub btnAdd_ClickSurvey(sender As Object, e As EventArgs)
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
            Return
        Else
            Try
                Dim cnx As SqlConnection
                Dim cmd As SqlCommand
                Dim clsUser As ClsUser = ClsSessionHelper.LogonUser

                cnx = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                cnx.Open()
                cmd = New SqlCommand("[dbo].[InsertSurvey]", cnx)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
                cmd.Parameters.AddWithValue("@Title", TitleTxt.Value)
                cmd.Parameters.AddWithValue("@Description", DescriptionTxt.Value)
                cmd.Parameters.AddWithValue("@WelcomeMsg", WelcomeMsgTxt.Value)
                cmd.Parameters.AddWithValue("@EndMsg", EndMsgTxt.Value)
                cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(StartDateTxt.Value))
                cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(EndDateTxt.Value))
                If DeployedBool.Value = "" Then
                    DeployedBool.Value = 0
                End If
                cmd.Parameters.AddWithValue("@Deployed", DeployedBool.Value)
                cmd.Parameters.AddWithValue("@Author", clsUser.FullName)
                cmd.ExecuteNonQuery()

            Catch ex As Exception
                Throw ex
            End Try
        End If
        TitleTxt.Value = ""
        DescriptionTxt.Value = ""
        WelcomeMsgTxt.Value = ""
        EndMsgTxt.Value = ""
        StartDateTxt.Value = Nothing
        EndDateTxt.Value = Nothing
        DeployedBool.Value = ""
        Response.Redirect("SurveyManager.apsx")
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Finish", "CloseWindow();RefreshTable();", True)
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

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteLine(EnvironmentID As String, SopId As String, ID As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SurveyID", ID))
            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[DeleteSurvey]", parameters) Then
                LogEbusinessAction(EnvironmentID, SopId, "Delete survey field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                LogEbusinessAction(EnvironmentID, SopId, "Delete survey field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLine(EnvironmentID As String, SopId As String, ID As String, Title As String, Description As String, WelcomeMsg As String, EndMsg As String, StartDate As String, EndDate As String, Deployed As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SurveyID", ID))
            parameters.Add(New SqlParameter("@Title", Title))
            parameters.Add(New SqlParameter("@Description", Description))
            parameters.Add(New SqlParameter("@WelcomeMsg", WelcomeMsg))
            parameters.Add(New SqlParameter("@EndMsg", EndMsg))
            parameters.Add(New SqlParameter("@StartDate", StartDate))
            parameters.Add(New SqlParameter("@EndDate", EndDate))
            parameters.Add(New SqlParameter("@Deployed", Deployed))

            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[EditSurvey]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

End Class
