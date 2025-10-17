
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Xml
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SAPRequestsLib
Imports Telerik.Web.UI

Partial Class ViewJsonFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ID As Integer = 0
        Dim ApplicationID As Integer = 0
        Dim ActionName As String = ""
        Dim EnvironmentID As Integer = 0
        Dim CorrelID As String = ""
        Dim TableName As String = ""
        Dim SopID As String = "ALL"
        Dim GlobalID As String = Nothing
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If Not Request.QueryString("ID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ID"), ID)
        End If
        If Not Request.QueryString("ApplicationID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ApplicationID"), ApplicationID)
        End If

        If Not Request.QueryString("ActionName") Is Nothing Then
            ActionName = Request.QueryString("ActionName").Replace("%20", " ")
        End If

        If Not Request.QueryString("EnvironmentID") Is Nothing Then
            Integer.TryParse(Request.QueryString("EnvironmentID"), EnvironmentID)
        End If

        If Not Request.QueryString("CorrelID") Is Nothing Then
            CorrelID = Request.QueryString("CorrelID")
        End If

        If Not Request.QueryString("ID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ID"), ID)
        End If

        If Not Request.QueryString("TableName") Is Nothing Then
            TableName = Request.QueryString("TableName")
        End If

        If Not Request.QueryString("SopID") Is Nothing Then
            SopID = Request.QueryString("SopID")
        End If

        If Not Request.QueryString("GlobalID") Is Nothing Then
            GlobalID = Request.QueryString("GlobalID")
        End If

        If SAPRequester.GetMessageTypeByMessage(ActionName) = SAPMessageType.NotSpecified _
            Or Not ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.SEND_REQUESTS_TO_SAP) Then
            SpanConfirmSendRequestToScoopOS.Visible = False
        End If
        Dim JsonFileSessionName = "Json_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
        Dim ReplySessionName = "Reply_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()

        If String.IsNullOrEmpty(Session(JsonFileSessionName)) And ApplicationID > 0 Then
            parameters.Add(New SqlParameter("ActionName", ActionName))
            parameters.Add(New SqlParameter("EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("CorrelID", CorrelID))
            parameters.Add(New SqlParameter("TableName", TableName))
            parameters.Add(New SqlParameter("SopID", SopID))
            parameters.Add(New SqlParameter("ID", ID))
            parameters.Add(New SqlParameter("GlobalID", GlobalID))
            Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("Logger.GetXMLInformation", parameters)
            If dataSet.Tables.Count = 3 Then
                Dim dTParameters As DataTable = dataSet.Tables(0)
                Dim dTRequest As DataTable = dataSet.Tables(1)
                Dim dTReply As DataTable = dataSet.Tables(2)
                If dTParameters.Rows.Count = 1 And SpanConfirmSendRequestToScoopOS.Visible Then
                    Session("MediaAuthenticationClientID_" + EnvironmentID.ToString()) = dTParameters.Rows(0)("MediaAuthenticationClientID").ToString()
                    Session("MediaAuthenticationClientSecret_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("MediaAuthenticationClientSecret").ToString()
                    Session("MediaRequestURL_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("MediaRequestURL").ToString()
                    Session("MediaNotifyEndpointURL_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("MediaNotifyEndpointURL").ToString()
                    Session("MediaAuthenticationURL_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("MediaAuthenticationURL").ToString()
                    Session("LanguageId_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("LanguageId").ToString()
                End If
                If dTRequest.Rows.Count = 1 Then
                    If dTRequest.Rows(0)("MSG_XML").ToString().Length > 0 Then
                        Session(JsonFileSessionName) = JValue.Parse(dTRequest.Rows(0)("MSG_XML").ToString()).ToString(Newtonsoft.Json.Formatting.Indented)
                    End If
                    RadTabStrip1.Tabs(0).Text = ClsDataAccessHelper.GetText(dTRequest.Rows(0), "TAB_NAME", "Request")
                End If
                For i As Integer = 0 To dTReply.Rows.Count - 1
                    If dTReply.Rows(i)("MSG_XML").ToString().Length > 0 Then
                        Dim activeSessionName = ReplySessionName & (IIf(dTReply.Rows.Count = 1, "", "_" & i.ToString()))
                        Session(activeSessionName) = JValue.Parse(dTReply.Rows(i)("MSG_XML").ToString()).ToString(Newtonsoft.Json.Formatting.Indented)
                        Dim radTabHeaderText As String = "Reply" & IIf(dTReply.Rows.Count = 1, "", " " & (i + 1).ToString())
                        RenderReplyTab(radTabHeaderText, activeSessionName)
                    End If
                Next
            End If
        Else
            RenderReplyTab("Reply", ReplySessionName)
        End If
        If String.IsNullOrEmpty(Session(JsonFileSessionName)) Or ID = 0 Then
            divJSONFile.InnerHtml = "No data found"
            BtnCopyJSON.Visible = False
            BtnViewJSONInBrowser.Visible = False
            BtnDownloadJSON.Visible = False
        Else
            divJSONFile.InnerHtml = ClsHelper.PrettyJsonInHtml(Session(JsonFileSessionName))
            BtnViewJSONInBrowser.Attributes.Add("onclick", "popup('GetJSONFile.ashx?file=" + JsonFileSessionName + "')")
            BtnDownloadJSON.Attributes.Add("onclick", "donwloadJSON('" + JsonFileSessionName + "')")
        End If

        watch.Stop()
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            If Not IsPostBack Then
                ClsHelper.Log("View Single JSON File", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            End If
        End If


    End Sub
    Private Property asynPostBackTriggerBtns As List(Of HtmlButton) = New List(Of HtmlButton)
    Private Sub RenderReplyTab(ByVal radTabText As String, ByVal activeSessionName As String)
        If Not String.IsNullOrEmpty(Session(activeSessionName)) Then
            Dim pageView As RadPageView = New RadPageView()
            Dim radTab As RadTab = New RadTab
            radTab.Width = 200
            radTab.Text = radTabText
            Dim xmlPageView As UserControls_JsonPageView = DirectCast(Page.LoadControl("~/UserControls/JsonPageView.ascx"), UserControls_JsonPageView)
            If Not String.IsNullOrEmpty(Session(activeSessionName)) Then
                xmlPageView.SetParameters(ClsHelper.FormatJsoninHTML(Session(activeSessionName)), activeSessionName)
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnCopyReply"), HtmlButton))
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnViewReplyInBrowser"), HtmlButton))
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnDownloadReplyXML"), HtmlButton))
            End If
            pageView.Controls.Add(xmlPageView)
            RadMultiPage1.PageViews.Add(pageView)
            RadTabStrip1.Tabs.Add(radTab)
        End If
    End Sub
    Public Sub SendRequestToScoopOS(sender As Object, e As EventArgs) Handles BtnSendRequestToScoopOS.Click
        Dim ActionName As String = ""
        Dim CorrelID As String = ""
        Dim SopID As String = ""
        Dim TableName As String = ""
        Dim EnvironmentID As Integer = 0
        Dim ApplicationId As Integer = 0
        Dim GlobalID As String = ""
        Dim ID As Integer = 0
        If Not Request.QueryString("ApplicationID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ApplicationID"), ApplicationId)
        End If
        If Not Request.QueryString("EnvironmentID") Is Nothing Then
            Integer.TryParse(Request.QueryString("EnvironmentID"), EnvironmentID)
        End If

        If Not Request.QueryString("ActionName") Is Nothing Then
            ActionName = Request.QueryString("ActionName").Replace("%20", " ")
        End If
        If Not Request.QueryString("SopID") Is Nothing Then
            SopID = Request.QueryString("SopID")
        End If
        If Not Request.QueryString("GlobalID") Is Nothing Then
            GlobalID = Request.QueryString("GlobalID")
        Else
            GlobalID = ClsSessionHelper.LogonUser.GlobalID.ToString()
        End If
        If Not Request.QueryString("ID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ID"), ID)
        End If
        If Not Request.QueryString("TableName") Is Nothing Then
            TableName = Request.QueryString("TableName")
        End If

        If Not Request.QueryString("CorrelID") Is Nothing Then
            CorrelID = Request.QueryString("CorrelID")
        End If

        Dim JsonFileSessionName = "Json_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
        Dim MediaAuthenticationClientID As String = Session("MediaAuthenticationClientID_" + EnvironmentID.ToString())
        Dim MediaAuthenticationClientSecret As String = Session("MediaAuthenticationClientSecret_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim MediaRequestURL As String = Session("MediaRequestURL_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim MediaNotifyEndpointURL As String = Session("MediaNotifyEndpointURL_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim MediaAuthenticationURL As String = Session("MediaAuthenticationURL_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim LanguageId As String = Session("LanguageId_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim Content As String = Session(JsonFileSessionName)
        Try
            btnViewRequestReplyScoopOS.Visible = False
            Dim Token As String = MediaRequester.GetToKenMedia(MediaAuthenticationURL, MediaAuthenticationClientID, MediaAuthenticationClientSecret)
            If Token <> "" Then
                Dim newRequest As String = New_Request(Content)
                Dim result As String = MediaRequester.PostMessage_media(Token, newRequest, MediaNotifyEndpointURL, MediaRequestURL)
                If result <> "" Then
                    ' Dim dynamicObject = JsonConvert.DeserializeObject(Of Object)(result)
                    Dim dynamicObject = JsonConvert.DeserializeObject(Of System.Dynamic.ExpandoObject)(result)
                    Dim List As IDictionary(Of String, Object) = CType(dynamicObject, IDictionary(Of String, Object))
                    Dim id1 = List.Values(0)
                    divSendToScoopOSInfo.InnerHtml = "<h3 style='color:green'>Message sent with success</h3><h4>Correl ID: <b>" + id1 + "</b></h4>"
                    btnViewRequestReplyScoopOS.Visible = True
                    btnViewRequestReplyScoopOS.Attributes.Add("onclick", String.Format("OpenViewJsonFileWindow('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", ApplicationId, ActionName, EnvironmentID, id1, TableName, SopID, GlobalID, ID))
                    Dim status As String = "INPROGRESS"
                    If CType(dynamicObject, IDictionary(Of String, Object)).ContainsKey("errors") Then
                        status = "ERROR"
                    End If
                    InsertMediaRequest(EnvironmentID, id1, GlobalID, SopID, LanguageId, newRequest, result, status)
                Else
                    divSendToScoopOSInfo.InnerHtml = "<h3 style='color:red'>Message not sent please review the error message</h3>"
                End If
            Else
                divSendToScoopOSInfo.InnerHtml = "<h3 style='color:red'>Message not sent please review the error message</h3>"
            End If
        Catch ex As Exception
            divSendToScoopOSInfo.InnerHtml = "<h3 style='color:red'>Message not sent please review the error message</h3>"
        End Try
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close dialog", "SendToScoopOSFinish();", True)
    End Sub

    Public Function InsertMediaRequest(ByVal EnvironmentID As String, ByVal CORREL_ID As String, ByVal U_GLOBALID As String, ByVal SOP As String, ByVal LanguageId As String, ByVal REQUEST As String, ByVal Reply As String, ByVal Status As String) As Boolean
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("CORREL_ID", CORREL_ID))
        parameters.Add(New SqlParameter("U_GLOBALID", U_GLOBALID))
        parameters.Add(New SqlParameter("LanguageId", LanguageId))
        parameters.Add(New SqlParameter("REQUEST", REQUEST))
        parameters.Add(New SqlParameter("SOP", SOP))
        parameters.Add(New SqlParameter("Reply", Reply))
        parameters.Add(New SqlParameter("Status", Status))
        Dim result = ClsDataAccessHelper.ExecuteNonQuery("Logger.Insert_Media_Request", parameters)
        Return result
    End Function

    Public Function New_Request(ByVal Content As String) As String
        Dim jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(Content)
        jsonObj("id") = Str_GenerateCorrelID()
        Dim Newcontent As String = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj)
        Return Newcontent
    End Function

    Public Function Str_GenerateCorrelID() As String
        Return "B2" & DateTime.Now.ToString("ddMMyyyyHHmmss") + (Guid.NewGuid().ToString().Replace("-", ""))
    End Function
End Class
