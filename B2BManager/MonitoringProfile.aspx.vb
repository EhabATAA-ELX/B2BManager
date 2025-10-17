
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports SAPRequestsLib
Imports SAPRequestsLib.Models

Partial Class MonitoringProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        End If

        Dim MessageID As Guid
        If Not Request.QueryString("MessageID") Is Nothing Then
            Guid.TryParse(Request.QueryString("MessageID"), MessageID)
        End If

        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim monitoringMessage As MonitoringMessage = MonitoringManager.GetMonitoringMessage(ConfigurationManager.ConnectionStrings("LogDB").ConnectionString,
                                                                                                ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(),
                                                                                                ConfigurationManager.AppSettings("ErrorEmailTo").ToString(),
                                                                                                ConfigurationManager.AppSettings("SMTPServer").ToString(),
                                                                                                MessageID
                                                                                                )
            If monitoringMessage IsNot Nothing Then
                lblMessage.Text = monitoringMessage.MessageID.ToString()
                lblWcfB2BWebServiceURL.Text = monitoringMessage.WcfB2BWebServiceURL
                lblWcfMethodName.Text = monitoringMessage.WcfMethodName
                ChkBoxSendDailyReport.Checked = monitoringMessage.SendDailyReport
                ChkBoxSendDailyReport_CheckedChanged(Nothing, Nothing)
                If monitoringMessage.SendDailyReport Then
                    txtSendReportHour.Text = monitoringMessage.DailyReportHour.ToString()
                    txtSendReportMinute.Text = monitoringMessage.DailyReportMinute.ToString()
                    txtBoxDailyReportEmailTo.Text = monitoringMessage.DailyReportEmailTo
                End If
                txtExpectedResponseTimeInMilliseconds.Text = monitoringMessage.ExpectedResponseTimeInMilliseconds.ToString()
                txtIntervalInSeconds.Text = monitoringMessage.MessageIntervalInSeconds
                ChkBoxActivatePerformanceDegradationAlerts.Checked = monitoringMessage.ActivatePerformanceDegradationAlerts
                ChkBoxActivatePerformanceDegradationAlerts_CheckedChanged(Nothing, Nothing)
                If monitoringMessage.ActivatePerformanceDegradationAlerts Then
                    txtPerformanceDegradationAlertMessagesCount.Text = monitoringMessage.PerformanceDegradationAlertMessagesCount.ToString()
                End If
                txtWorstAcceptableResponseTimeInMilliseconds.Text = monitoringMessage.WorstAcceptableResponseTimeInMilliseconds
                If Not String.IsNullOrEmpty(monitoringMessage.MessageXML) Then
                    Dim sessionName As String = "MessageXML_" + MessageID.ToString()
                    Session(sessionName) = monitoringMessage.MessageXML
                    Dim asynPostBackTriggerBtns As List(Of HtmlButton) = New List(Of HtmlButton)
                    xmlPageView.SetParameters(ClsHelper.FormatXMLinHTML(monitoringMessage.MessageXML), sessionName)
                    asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnCopyReply"), HtmlButton))
                    asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnViewReplyInBrowser"), HtmlButton))
                    asynPostBackTriggerBtns.Add(DirectCast(xmlPageView.FindControl("BtnDownloadReplyXML"), HtmlButton))
                    DirectCast(xmlPageView.FindControl("BtnDownloadReplyXML"), HtmlButton).InnerText = "Download XML Request"
                    DirectCast(xmlPageView.FindControl("BtnCopyReply"), HtmlButton).InnerText = "Copy XML Request"
                    DirectCast(xmlPageView.FindControl("BtnViewReplyInBrowser"), HtmlButton).InnerText = "View XML Request In Browser"
                    For Each btn In asynPostBackTriggerBtns
                        Dim asynPostBackTrigger As AsyncPostBackTrigger = New AsyncPostBackTrigger()
                        asynPostBackTrigger.ControlID = btn.UniqueID
                        UpdatePanel1.Triggers.Add(asynPostBackTrigger)
                    Next
                End If
                PauseIntervalsManagerUC.UID = MessageID
                PauseIntervalsManagerUC.pauseIntervals = monitoringMessage.pauseIntervals
                watch.Stop()
                ClsHelper.Log("View Monitoring Profile", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>MessageID:</b> " + MessageID.ToString(), watch.ElapsedMilliseconds, False, Nothing)
            End If
        Else
            PauseIntervalsManagerUC.UID = MessageID
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

End Class
