Imports System.Data.SqlClient
Imports System.Data
Imports System.Xml
Imports System.Drawing
Imports SAPRequestsLib
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class ViewXMLFiles
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")


        Dim ApplicationID As Integer = 0
        Dim ActionName As String = ""
        Dim EnvironmentID As Integer = 0
        Dim CorrelID As String = ""
        Dim ID As Integer = 0
        Dim TableName As String = ""
        Dim SopID As String = "ALL"
        Dim GlobalID As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If Not Request.QueryString("ApplicationID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ApplicationID"), ApplicationID)
        End If

        If Not Request.QueryString("ActionName") Is Nothing Then
            ActionName = Request.QueryString("ActionName").Replace("%20", " ")
        End If

        If SAPRequester.GetMessageTypeByMessage(ActionName) = SAPMessageType.NotSpecified _
            Or Not ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.SEND_REQUESTS_TO_SAP) Then
            SpanConfirmSendRequestToSap.Visible = False
        End If

        If SAPRequester.GetMessageTypeByMessage(ActionName) = SAPMessageType.NotSpecified _
            Or SAPRequester.GetMessageTypeByMessage(ActionName) = SAPMessageType.PlaceOrder _
            Or SAPRequester.GetMessageTypeByMessage(ActionName) = SAPMessageType.PlaceOrder_Async _
            Or Not ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_A_MONITORING_MESSAGE) Then
            SpanMonitorThisMessage.Visible = False
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

        Dim RequestSessionName = "Request_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
        Dim ReplySessionName = "Reply_" + ActionName + "_" + CorrelID + "_" + ID.ToString()

        If Session(RequestSessionName) Is Nothing Or Session(ReplySessionName) Is Nothing And ApplicationID > 0 Then
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
                If dTParameters.Rows.Count = 1 And SpanConfirmSendRequestToSap.Visible Then
                    Session("SAPWCFURL" + EnvironmentID.ToString()) = dTParameters.Rows(0)("SAPWCFURL").ToString()
                    Session("SESSIONID_" + CorrelID + "_" + ActionName + "_" + SopID) = IIf(dTParameters.Rows(0)("SESSIONID").ToString().Equals(""), ConfigurationManager.AppSettings("SessionID"), dTParameters.Rows(0)("SESSIONID").ToString())
                    Session("SAPWCFUSERNAME_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("SAPWCFUSERNAME").ToString()
                    Session("SAPWCFPSSWORD_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("SAPWCFPSSWORD").ToString()
                    Session("SAPWCFMETHOD_" + CorrelID + "_" + ActionName + "_" + SopID) = dTParameters.Rows(0)("SAPWCFMETHOD").ToString()
                End If

                If dTRequest.Rows.Count = 1 Then
                    If dTRequest.Rows(0)("MSG_XML").ToString().Length > 0 Then
                        Session(RequestSessionName) = ClsHelper.PrettyXml(dTRequest.Rows(0)("MSG_XML").ToString())
                    End If
                    RadTabStrip1.Tabs(0).Text = ClsDataAccessHelper.GetText(dTRequest.Rows(0), "TAB_NAME", "Request")
                End If

                For i As Integer = 0 To dTReply.Rows.Count - 1
                    If dTReply.Rows(i)("MSG_XML").ToString().Length > 0 Then
                        Dim activeSessionName = ReplySessionName & (IIf(dTReply.Rows.Count = 1, "", "_" & i.ToString()))
                        Session(activeSessionName) = ClsHelper.PrettyXml(dTReply.Rows(i)("MSG_XML").ToString())
                        Dim radTabHeaderText As String = "Reply" & IIf(dTReply.Rows.Count = 1, "", " " & (i + 1).ToString())

                        If dTReply.Columns.Contains("TAB_NAME") Then
                            radTabHeaderText = ClsDataAccessHelper.GetText(dTReply.Rows(i), "TAB_NAME", radTabHeaderText)
                        Else
                            Try
                                Dim doc As XmlDocument = New XmlDocument()
                                doc.LoadXml(ClsHelper.PrettyXml(dTReply.Rows(i)("MSG_XML").ToString()))
                                If doc.GetElementsByTagName("SUBJECT").Count > 0 Then
                                    radTabHeaderText = doc.GetElementsByTagName("SUBJECT")(0).InnerText
                                End If
                            Catch ex As Exception
                                radTabHeaderText = "Reply" & IIf(dTReply.Rows.Count = 1, "", " " & (i + 1).ToString())
                            End Try
                        End If
                        RenderReplyTab(radTabHeaderText, activeSessionName)
                    End If
                Next
            End If
        Else
            RenderReplyTab("Reply", ReplySessionName)
        End If

        divXMLRequest.InnerHtml = ClsHelper.FormatXMLinHTML(Session(RequestSessionName))
        If String.IsNullOrEmpty(Session(RequestSessionName)) Then
            BtnCopyRequest.Visible = False
            BtnViewRequestInBrowser.Visible = False
            BtnDownloadRequestXML.Visible = False
        Else
            BtnViewRequestInBrowser.Attributes.Add("onclick", "popup('GetXMLFile.ashx?file=" + RequestSessionName + "')")
            BtnDownloadRequestXML.Attributes.Add("onclick", "donwloadXML('" + RequestSessionName + "')")
        End If
        watch.Stop()
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            If Not IsPostBack Then
                ClsHelper.Log("View XML Files", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            For Each btn In asynPostBackTriggerBtns
                Dim asynPostBackTrigger As AsyncPostBackTrigger = New AsyncPostBackTrigger()
                asynPostBackTrigger.ControlID = btn.UniqueID
                UpdatePanel1.Triggers.Add(asynPostBackTrigger)
            Next
            ChkBoxSendDailyReport.Checked = True
        End If
    End Sub

    Private Property asynPostBackTriggerBtns As List(Of HtmlButton) = New List(Of HtmlButton)

    Private Sub RenderReplyTab(ByVal radTabText As String, ByVal activeSessionName As String)
        If Not String.IsNullOrEmpty(Session(activeSessionName)) Then
            Dim pageView As RadPageView = New RadPageView()
            Dim radTab As RadTab = New RadTab
            radTab.Width = 200
            radTab.Text = radTabText
            Dim xmlPageView As UserControls_XMLPageView = DirectCast(Page.LoadControl("~/UserControls/XMLPageView.ascx"), UserControls_XMLPageView)
            If Not String.IsNullOrEmpty(Session(activeSessionName)) Then
                xmlPageView.SetParameters(ClsHelper.FormatXMLinHTML(Session(activeSessionName)), activeSessionName)
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnCopyReply"), HtmlButton))
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnViewReplyInBrowser"), HtmlButton))
                asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnDownloadReplyXML"), HtmlButton))
            End If
            pageView.Controls.Add(xmlPageView)
            RadMultiPage1.PageViews.Add(pageView)
            RadTabStrip1.Tabs.Add(radTab)
        End If
    End Sub

    Protected Sub ChkBoxActivatePerformanceDegradationAlerts_CheckedChanged(sender As Object, e As EventArgs)
        PnlAlert.Visible = ChkBoxActivatePerformanceDegradationAlerts.Checked
    End Sub

    Protected Sub ChkBoxSendDailyReport_CheckedChanged(sender As Object, e As EventArgs)
        txtSendReportHour.Enabled = ChkBoxSendDailyReport.Checked
        txtSendReportMinute.Enabled = ChkBoxSendDailyReport.Checked
        txtBoxDailyReportEmailTo.Enabled = ChkBoxSendDailyReport.Checked
    End Sub

    Public Sub SendRequestToSap(sender As Object, e As EventArgs) Handles BtnSendRequestToSap.Click
        Dim ApplicationId As Integer = 0
        Dim EnvironmentID As Integer = 0
        Dim ID As Integer = 0
        Dim GlobalID As String = ""
        Dim ActionName As String = ""
        Dim CorrelID As String = ""
        Dim SopID As String = ""
        Dim TableName As String = ""
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If Not Request.QueryString("ApplicationID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ApplicationID"), ApplicationId)
        End If
        If Not Request.QueryString("EnvironmentID") Is Nothing Then
            Integer.TryParse(Request.QueryString("EnvironmentID"), EnvironmentID)
        End If

        If Not Request.QueryString("ActionName") Is Nothing Then
            ActionName = Request.QueryString("ActionName").Replace("%20", " ")
        End If

        If Not Request.QueryString("TableName") Is Nothing Then
            TableName = Request.QueryString("TableName")
        End If

        If Not Request.QueryString("CorrelID") Is Nothing Then
            CorrelID = Request.QueryString("CorrelID")
        End If

        If Not Request.QueryString("GlobalID") Is Nothing Then
            GlobalID = Request.QueryString("GlobalID")
        Else
            GlobalID = ClsSessionHelper.LogonUser.GlobalID.ToString()
        End If

        If Not Request.QueryString("SopID") Is Nothing Then
            SopID = Request.QueryString("SopID")
        End If

        If Not Request.QueryString("ID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ID"), ID)
        End If

        Dim RequestSessionName = "Request_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
        Dim wcfUrl As String = Session("SAPWCFURL" + EnvironmentID.ToString())
        Dim wcfUserName As String = Session("SAPWCFUSERNAME_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim wcfPassword As String = Session("SAPWCFPSSWORD_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim wcfMethodName As String = Session("SAPWCFMETHOD_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim SessionID As String = Session("SESSIONID_" + CorrelID + "_" + ActionName + "_" + SopID)
        Dim xmlRequest As String = Session(RequestSessionName)
        Dim errorMessage As String = Nothing
        Dim SAPReplyResult As SAPReplyResult = SAPRequester.SendSAPMessage(xmlRequest, wcfUrl, wcfMethodName, wcfUserName, wcfPassword, GlobalID, SessionID, SopID)
        btnViewRequestReplyXML.Visible = False

        If SAPReplyResult.HasError Then
            divSendToSapInfo.InnerHtml = "<h3 style='color:red'>Message not sent please review the error message</h3>"
            If SAPReplyResult.error IsNot Nothing Then
                divXMLReplySAP.InnerHtml = SAPReplyResult.error.Message
                errorMessage = SAPReplyResult.error.Message
            End If
            divXMLReplySAP.Visible = True
        Else
            If String.IsNullOrEmpty(SAPReplyResult.Reply) Then
                divSendToSapInfo.InnerHtml = "<h3 style='color:green'>Message sent with success</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                btnViewRequestReplyXML.Visible = True
                btnViewRequestReplyXML.Attributes.Add("onclick", String.Format("OpenViewXMLFilesWindow('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", ApplicationId, ActionName, EnvironmentID, SAPReplyResult.CorrelID, TableName, SopID, GlobalID, ID))
            Else
                If Not SAPReplyResult.Reply.Contains("<Error Reply>") Then
                    divSendToSapInfo.InnerHtml = "<h3 style='color:green'>Message sent with success</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                    divXMLReplySAP.InnerHtml = ClsHelper.FormatXMLinHTML(ClsHelper.PrettyXml(SAPReplyResult.Reply))
                    divXMLReplySAP.Visible = True
                Else
                    divSendToSapInfo.InnerHtml = "<h3 style='color:orange'>Message sent with Error</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                    divXMLReplySAP.InnerHtml = ClsHelper.FormatXMLinHTML(ClsHelper.PrettyXml(SAPReplyResult.Reply))
                    divXMLReplySAP.Visible = True
                End If
            End If

        End If

        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close dialog", "SendToSapFinish();", True)
        ClsMessageRequesterHelper.LogXMLReply(SAPReplyResult.CorrelID, CorrelID, ClsHelper.PrettyXml(SAPReplyResult.Reply), xmlRequest, EnvironmentID, ActionName, SAPReplyResult.RequestedDate, SAPReplyResult.ReceivedDate, wcfUrl, SAPReplyResult.HasError, errorMessage)
        watch.Stop()
        ClsHelper.Log("Send Request To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>ApplicationID:</b> {0}</br><b>Action Name:</b> {1}</br><b>CorrelID: </b> {2}</br><b>SAP Reply CorrelID:</b> {3}", ApplicationId, ActionName, CorrelID, SAPReplyResult.CorrelID), watch.ElapsedMilliseconds, SAPReplyResult.HasError, errorMessage)
    End Sub

    Protected Sub BtnSaveMointoringMessageForLater_Click(sender As Object, e As EventArgs) Handles BtnSaveMointoringMessageForLater.Click
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim CorrelID As String = ""
        If Not Request.QueryString("CorrelID") Is Nothing Then
            CorrelID = Request.QueryString("CorrelID")
        End If
        If SaveMointoringMessage() Then
            lblInfoSaveMonitoringMessage.ForeColor = Color.Green
            lblInfoSaveMonitoringMessage.Text = "Message saved for later with success"
        Else
            lblInfoSaveMonitoringMessage.ForeColor = Color.Red
            lblInfoSaveMonitoringMessage.Text = "An unhandled exception has occurred please try later"
        End If
        watch.Stop()
        ClsHelper.Log("Save Mointoring Message for later", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>Linked CorrelID: </b>" + CorrelID, watch.ElapsedMilliseconds, False, Nothing)
    End Sub
    Protected Sub BtnSaveMointoringMessageAndEnable_Click(sender As Object, e As EventArgs) Handles BtnSaveMointoringMessageAndEnable.Click
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim CorrelID As String = ""
        If Not Request.QueryString("CorrelID") Is Nothing Then
            CorrelID = Request.QueryString("CorrelID")
        End If
        If SaveMointoringMessage(True) Then
            lblInfoSaveMonitoringMessage.ForeColor = Color.Green
            lblInfoSaveMonitoringMessage.Text = "Message saved and enalbed with success"
        Else
            lblInfoSaveMonitoringMessage.ForeColor = Color.Red
            lblInfoSaveMonitoringMessage.Text = "An unhandled exception has occurred please try later"
        End If
        watch.Stop()
        ClsHelper.Log("Save Mointoring Message and enable it", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>Linked CorrelID: </b>" + CorrelID, watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Function SaveMointoringMessage(Optional ByVal Enabled As Boolean = False) As Boolean
        Try
            Dim EnvironmentID As Integer = 0
            Dim GlobalID As String = ""
            Dim ActionName As String = ""
            Dim CorrelID As String = ""
            Dim SopID As String = ""

            If Not Request.QueryString("EnvironmentID") Is Nothing Then
                Integer.TryParse(Request.QueryString("EnvironmentID"), EnvironmentID)
            End If

            If Not Request.QueryString("ActionName") Is Nothing Then
                ActionName = Request.QueryString("ActionName").Replace("%20", " ")
            End If

            If Not Request.QueryString("CorrelID") Is Nothing Then
                CorrelID = Request.QueryString("CorrelID")
            End If

            If Not Request.QueryString("GlobalID") Is Nothing Then
                GlobalID = Request.QueryString("GlobalID")
            End If

            If Not String.IsNullOrEmpty(GlobalID) Then
                GlobalID = ClsSessionHelper.LogonUser.GlobalID.ToString()
            End If

            If Not Request.QueryString("SopID") Is Nothing Then
                SopID = Request.QueryString("SopID")
            End If

            Dim RequestSessionName = "Request_" + ActionName + "_" + CorrelID + "_" + ID.ToString()
            Dim MessageXML As String = Session(RequestSessionName)
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@MessageType", SAPRequester.GetMessageTypeByMessage(MessageXML).ToString()))
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SOP_ID", SopID))
            parameters.Add(New SqlParameter("@WcfB2BWebServiceURL", Session("SAPWCFURL" + EnvironmentID.ToString())))
            parameters.Add(New SqlParameter("@WcfMethodName", Session("SAPWCFMETHOD_" + CorrelID + "_" + ActionName + "_" + SopID)))
            parameters.Add(New SqlParameter("@WcfUserName", Session("SAPWCFUSERNAME_" + CorrelID + "_" + ActionName + "_" + SopID)))
            parameters.Add(New SqlParameter("@WcfPassword", Session("SAPWCFPSSWORD_" + CorrelID + "_" + ActionName + "_" + SopID)))
            parameters.Add(New SqlParameter("@C_GLOBALID", New Guid(GlobalID)))
            parameters.Add(New SqlParameter("@SessionID", New Guid(Session("SESSIONID_" + CorrelID + "_" + ActionName + "_" + SopID).ToString())))
            parameters.Add(New SqlParameter("@MessageIntervalInSeconds", txtIntervalInSeconds.Value))
            parameters.Add(New SqlParameter("@SendDailyReport", ChkBoxSendDailyReport.Checked))
            If (ChkBoxSendDailyReport.Checked) Then
                parameters.Add(New SqlParameter("@DailyReportHour", txtSendReportHour.Value))
                parameters.Add(New SqlParameter("@DailyReportMinute", txtSendReportMinute.Value))
                parameters.Add(New SqlParameter("@DailyReportEmailTo", txtBoxDailyReportEmailTo.Text))
            End If
            parameters.Add(New SqlParameter("@MessageXML", MessageXML.ToString().Replace("<?xml version=""1.0"" encoding=""UTF-8""?>", "")))
            parameters.Add(New SqlParameter("@IsActive", Enabled))
            parameters.Add(New SqlParameter("@ExpectedResponseTimeInMilliseconds", txtExpectedResponseTimeInMilliseconds.Value))
            parameters.Add(New SqlParameter("@WorstAcceptableResponseTimeInMilliseconds", txtWorstAcceptableResponseTimeInMilliseconds.Value))
            parameters.Add(New SqlParameter("@ActivatePerformanceDegradationAlerts", ChkBoxActivatePerformanceDegradationAlerts.Checked))
            If (ChkBoxActivatePerformanceDegradationAlerts.Checked) Then
                parameters.Add(New SqlParameter("@PerformanceDegradationAlertMessagesCount", txtPerformanceDegradationAlertMessagesCount.Value))
            End If
            parameters.Add(New SqlParameter("@CreatedBy", ClsSessionHelper.LogonUser.ID))
            parameters.Add(New SqlParameter("@LinkedCorrelID", CorrelID))
            ClsDataAccessHelper.ExecuteNonQuery("[Monitoring].[SaveMonitoringMessage]", parameters)
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("Unable to Save Monitoring Message</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                        , exceptionStackTrace
                        )
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try

        Return True
    End Function


End Class
