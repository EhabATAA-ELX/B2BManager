
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class Chatbot_ChatBotConversationsViewer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=ChatBotConversationsViewer.aspx", True)
        End If

        If Not IsPostBack Then
            PopulateDropDowns(True)
            RunSearch(True)
        Else
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If ("selectedUser".Equals(__EVENTTARGET)) Then
                    If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        If (arguments.Length = 2) Then
                            Dim chatbotID As Integer = 0
                            Dim userGlobalID As Guid = Guid.Empty

                            Integer.TryParse(arguments(0), chatbotID)
                            Guid.TryParse(arguments(1), userGlobalID)

                            If chatbotID > 0 And Not userGlobalID = Guid.Empty Then
                                SelectUser(userGlobalID, chatbotID, True)
                            End If
                        End If
                    End If
                End If

                If ("selectedConversation".Equals(__EVENTTARGET)) Then
                    If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        If (arguments.Length = 2) Then
                            Dim chatbotID As Integer = 0

                            Integer.TryParse(arguments(0), chatbotID)

                            If chatbotID > 0 And Not String.IsNullOrEmpty(arguments(1)) Then
                                SelectConversation(arguments(1), chatbotID)
                            End If
                        End If
                    End If
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(selectedUser.Value) Then
            SelectFinish(selectedUser.Value)
        End If
        If Not String.IsNullOrEmpty(selectedConversation.Value) Then
            SelectFinish(selectedConversation.Value, "Conversation")
        End If

    End Sub

    Private Sub SelectUser(userGlobalID As Guid, chatbotID As Integer, Optional selectFirstConversaton As Boolean = False)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        selectedUser.Value = userGlobalID.ToString()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ChatbotID", chatbotID))
        parameters.Add(New SqlParameter("@U_GLOBALID", userGlobalID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[ChatBot].[GetConversationByUserID]", parameters)
        conversationsRepeter.DataSource = dataTable
        conversationsRepeter.DataBind()
        If selectFirstConversaton AndAlso dataTable.Rows.Count > 0 Then
            SelectConversation(dataTable.Rows(0)("ConversationID").ToString(), chatbotID)
        End If
        watch.Stop()
        ClsHelper.Log("Select Chatbot User", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>User global ID:</b> " + userGlobalID.ToString() + "</br><b>Chatbot ID:</b> " + chatbotID.ToString(), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Private Sub SelectFinish(value As String, Optional type As String = "User")
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "select" & type & "Finish", "select" & type & "Finish(""" + value + """);", True)
    End Sub

    Private Sub SelectConversation(conversationID As String, chatbotID As Integer)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        selectedConversation.Value = conversationID
        Dim chatbots As List(Of ClsHelper.Chatbot) = ClsSessionHelper.chatBots
        Dim developperToken As String = String.Empty
        For Each chatbot As ClsHelper.Chatbot In chatbots
            If chatbot.ID = chatbotID Then
                developperToken = chatbot.DeveloperToken
                Exit For
            End If
        Next
        Dim errorMessage = String.Empty
        If Not String.IsNullOrEmpty(developperToken) Then
            Try
                Dim eluxBotService As EluxBotService.EluxBotService1 = New EluxBotService.EluxBotService1
                conversationContent.InnerHtml = eluxBotService.GetFullConversation(conversationID, developperToken, False, False)
            Catch ex As Exception
                Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                If Not ex.Message Is Nothing Then
                    exceptionMessage = ex.Message
                End If
                If Not ex.StackTrace Is Nothing Then
                    exceptionStackTrace = ex.StackTrace
                End If
                errorMessage = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            End Try
        Else
            conversationContent.InnerHtml = "It looks Like your chatbot wasn't well configured or the devlopper token wasn't specified. Please check its configuration and try again."
        End If
        watch.Stop()
        ClsHelper.Log("Select Chatbot Conversation", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>ConversationID:</b> " + conversationID, watch.ElapsedMilliseconds, Not String.IsNullOrEmpty(errorMessage), errorMessage)
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID
            Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString()).AddDays(-7)
            RadDateTimePickerFrom.SelectedDate = fromDate
            Dim toDate = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString()).AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)
            RadDateTimePickerTo.SelectedDate = toDate
            RadDateTimePickerFrom.MinDate = fromDate.AddDays(-60)
            RadDateTimePickerTo.MinDate = toDate.AddDays(-60)
        End If
    End Sub

    Protected Sub ddlChatbots_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateDropDowns(False, ddlChatbots.SelectedValue)
    End Sub

    Private Sub PopulateDropDowns(Optional forceLoad As Boolean = False, Optional ByVal selectedValue As String = Nothing)
        Dim activeChatbots As List(Of ClsHelper.BasicModel) = New List(Of ClsHelper.BasicModel)
        Dim chatbots As List(Of ClsHelper.Chatbot) = ClsSessionHelper.chatBots
        For Each chatbot As ClsHelper.Chatbot In chatbots
            If chatbot.Checked = True Then
                activeChatbots.Add(chatbot)
            End If
        Next
        If selectedValue Is Nothing Then
            ClsHelper.RenderDropDownList(ddlChatbots, activeChatbots, True)
            ddlChatbots.SelectedIndex = 0
            selectedValue = ddlChatbots.SelectedValue
        End If
        Dim selectedChatbot = ClsHelper.FindChatbotByID(chatbots, selectedValue)
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedChatbot.Environments, True)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedChatbot.Countries, ddlCountry.SelectedValue, True)
    End Sub

    Private Sub RunSearch(Optional selectFirstByDefault As Boolean = False)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        selectedUser.Value = ""
        selectedConversation.Value = ""
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[ChatBot].[GetUserConversations]", GetSearchParameters())
        usersRepeter.DataSource = dataTable
        usersRepeter.DataBind()

        If selectFirstByDefault Then
            If dataTable.Rows.Count > 0 Then
                Dim userGlobalID As Guid = Guid.Empty
                Guid.TryParse(dataTable.Rows(0)("U_GLOBALID").ToString(), userGlobalID)
                If Not userGlobalID = Guid.Empty Then
                    SelectUser(userGlobalID, CInt(ddlChatbots.SelectedValue.ToString()), True)
                End If
            End If
        End If
        watch.Stop()
        ClsHelper.Log("Run Chatbot Conversation Search", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim parameter As SqlParameter
        If ddlChatbots.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("@ChatbotID", CInt(ddlChatbots.SelectedValue.ToString())))
        End If

        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("@EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
        End If

        If ddlCountry.SelectedValue <> "All" Then
            parameters.Add(New SqlParameter("@SopIDs", ddlCountry.SelectedValue))
        End If

        parameter = New SqlParameter("@FromDate", RadDateTimePickerFrom.SelectedDate)
        parameter.DbType = DbType.DateTime
        parameters.Add(parameter)

        parameter = New SqlParameter("@ToDate", RadDateTimePickerTo.SelectedDate)
        parameter.DbType = DbType.DateTime
        parameters.Add(parameter)

        Return parameters
    End Function

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        RunSearch(True)
        If Not String.IsNullOrEmpty(selectedUser.Value) Then
            SelectFinish(selectedUser.Value)
        End If
        If Not String.IsNullOrEmpty(selectedConversation.Value) Then
            SelectFinish(selectedConversation.Value, "Conversation")
        End If
    End Sub
End Class
