Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI
Imports ClsHelper
Partial Class SurveyAnswers
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

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            If (__EVENTTARGET.EndsWith("UpdatePanel1")) Then
                BindData()
                UpdatePanel1.Update()
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
                BindDataQuestions()
                UpdatePanel2.Update()
                ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "BindDataTableQuestions", "BindDataTableQuestions();", True)
            End If
            If (__EVENTTARGET.EndsWith("UpdatePanel2")) Then
                BindDataQuestions()
                UpdatePanel2.Update()
                ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "BindDataTableQuestions", "BindDataTableQuestions();", True)
            End If
        Else
            RenderControls()
            BindData()
            UpdatePanel1.Update()
            ClsHelper.Log("Load Survey", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
            BindDataQuestions()
            UpdatePanel2.Update()
            ClsHelper.Log("Load Questions", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
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
        GetLanguagesBycountry()
        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
        btnAddAnswers.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_SURVEY)
    End Sub

    Public Sub GetLanguagesBycountry()
        Dim dt = ClsEbusinessHelper.GetLanguages(ddlEnvironment.SelectedValue, Page.Cache)
        If dt IsNot Nothing Then
            Dim dr As DataRow() = dt.Select("S_SOP_ID='" + ddlCountry.SelectedValue + "'")
            'If dr.Length = 1 Then
            '    ddlLanguage.Visible = False
            '    lblLanguage.Visible = False
            'Else
            ddlLanguage.Visible = True
            lblLanguage.Visible = True
            'End If
            HdLanguage.Value = dr(0)("LANG_ISOCODE")
            ddlLanguage.Items.Clear()

            For Each row As DataRow In dr
                ddlLanguage.Items.Add(New ListItem(row("LANG_NAME").ToString(), row("LANG_ISOCODE").ToString()))
            Next
        End If
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        BindDataQuestions()
        UpdatePanel2.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "BindDataTableQuestions", "BindDataTableQuestions();", True)
    End Sub
    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        GetLanguagesBycountry()
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        BindDataQuestions()
        UpdatePanel2.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "BindDataTableQuestions", "BindDataTableQuestions();", True)
    End Sub
    Protected Sub ddlLanguage_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData()
        UpdatePanel1.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "BindDataTable", "BindDataTable();", True)
        BindDataQuestions()
        UpdatePanel2.Update()
        ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "BindDataTableQuestions", "BindDataTableQuestions();", True)
    End Sub

    Private Sub BindData()
        Try
            Dim QuestionID = Request.QueryString("QuestionID")
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[dbo].[GetAllAnswer]", cnx)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
            cmd.Parameters.AddWithValue("@QuestionID", QuestionID)
            ''cmd.Parameters.AddWithValue("@IsoCodeLanguage", HdLanguage.Value)
            cmd.Parameters.AddWithValue("@IsoCodeLanguage", ddlLanguage.SelectedValue)
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
                            img.Attributes.Add("onclick", "EditRowAnswer('" + trow.ID + "');")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "Edit")
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


                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_SURVEY Or ActionDesignation.TRANSLATE_SURVEY) Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/CircularTools/B2BTranslations.png"
                            img.Attributes.Add("onclick", "EditRowAnswerTranslation('" + trow.ID + "');")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "Edit Translation")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            toolsCount += 1
                            actionsHeader = "Edit Translation"
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
                B2BAnswers.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    Private Sub BindDataQuestions()
        Try
            Dim QuestionID = Request.QueryString("QuestionID")
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[dbo].[GetQuestion]", cnx)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
            cmd.Parameters.AddWithValue("@QuestionID", QuestionID)
            '''cmd.Parameters.AddWithValue("@IsoCodeLanguage", HdLanguage.Value)
            cmd.Parameters.AddWithValue("@IsoCodeLanguage", ddlLanguage.SelectedValue)
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
                            img.Attributes.Add("onclick", "EditRowQuestion('" + trow.ID + "');")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "Edit")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            toolsCount += 1
                            actionsHeader = "Edit"
                        End If

                        If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_SURVEY Or ClsHelper.ActionDesignation.TRANSLATE_SURVEY) Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/CircularTools/B2BTranslations.png"
                            img.Attributes.Add("onclick", "EditTranslation('" + trow.ID + "');;")
                            img.Attributes.Add("class", "width20px LineChartImg")
                            img.Attributes.Add("title", "Edit Translation")
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            toolsCount += 1
                            actionsHeader = "Edit"
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
                B2BQuestionTranslation.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    Protected Sub btnAdd_ClickAnswer(sender As Object, e As EventArgs)
        Dim QuestionID = Request.QueryString("QuestionID")
        Dim SurveyID = Request.QueryString("SurveyID")
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
            Return
        Else
            Try
                Dim cnx As SqlConnection
                Dim cmd As SqlCommand

                cnx = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                cnx.Open()
                cmd = New SqlCommand("[dbo].[InsertAnswer]", cnx)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
                cmd.Parameters.AddWithValue("@QuestionID", QuestionID)
                cmd.Parameters.AddWithValue("@Answer", AnswerTxt.Value)
                cmd.Parameters.AddWithValue("@Rank", RankTxt.Value)
                cmd.ExecuteNonQuery()

            Catch ex As Exception
                Throw ex
            End Try
        End If
        AnswerTxt.Value = ""
        RankTxt.Value = ""
        Response.Redirect("SurveyAnswers.aspx?QuestionID=" + QuestionID + "&SurveyID=" + SurveyID)
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Finish", "CloseWindow();RefreshTable();", True)
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLineQuestion(EnvironmentID As String, SopId As String, ID As String, SurveyID As String, Question As String, Rank As String, Mandatory As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@QuestionID", ID))
            parameters.Add(New SqlParameter("@SurveyID", SurveyID))
            parameters.Add(New SqlParameter("@Question", Question))
            parameters.Add(New SqlParameter("@Rank", Rank))
            parameters.Add(New SqlParameter("@Mandatory", Mandatory))

            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[EditQuestion]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLineAnswer(EnvironmentID As String, SopId As String, AnswerID As String, QuestionID As String, Answer As String, Rank As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@AnswerID", AnswerID))
            parameters.Add(New SqlParameter("@QuestionID", QuestionID))
            parameters.Add(New SqlParameter("@Answer", Answer))
            parameters.Add(New SqlParameter("@Rank", Rank))

            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[EditAnswer]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLineQuestionTranslation(EnvironmentID As String, SopId As String, QuestionID As String, LANG_ISOCODE As String, Translation As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@QuestionID", QuestionID))
            parameters.Add(New SqlParameter("@LANG_ISOCODE", LANG_ISOCODE))
            parameters.Add(New SqlParameter("@Translation", Translation))

            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[InsertUpdateQuestionTranslation]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub UpdateLineAnswerTranslation(EnvironmentID As String, SopId As String, QuestionID As String, AnswerID As String, LANG_ISOCODE As String, Translation As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@QuestionID", QuestionID))
            parameters.Add(New SqlParameter("@AnswerID", AnswerID))
            parameters.Add(New SqlParameter("@IsoCodeLanguage", LANG_ISOCODE))
            parameters.Add(New SqlParameter("@Translation", Translation))

            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[InsertUpdateAnswerTranslation]", parameters) Then
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.LogEbusinessAction(EnvironmentID, SopId, "Save Specific Management field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteLine(EnvironmentID As String, SopId As String, ID As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@AnswerID", ID))
            If ClsDataAccessHelper.ExecuteNonQuery("[dbo].[DeleteAnswer]", parameters) Then
                LogEbusinessAction(EnvironmentID, SopId, "Delete question field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                LogEbusinessAction(EnvironmentID, SopId, "Delete question field", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
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

End Class
