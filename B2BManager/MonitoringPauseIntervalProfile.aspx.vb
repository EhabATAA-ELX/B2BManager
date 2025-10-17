
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports SAPRequestsLib
Imports SAPRequestsLib.Models

Partial Class MonitoringPauseIntervalProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim PauseIntervalID As Integer = 0
            Integer.TryParse(Request.QueryString("PauseIntervalID"), PauseIntervalID)

            If PauseIntervalID > 0 Then
                CType(Master.FindControl("title"), HtmlTitle).Text = "Display/edit Pause Interval " & PauseIntervalID.ToString()
                BtnSavePauseInterval.Text = "Update"
                BtnSavePauseInterval.OnClientClick = "ProcessPauseIntervalButton('Update')"
            Else
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Pause Interval"
                BtnSavePauseInterval.Text = "Save"
                BtnSavePauseInterval.OnClientClick = "ProcessPauseIntervalButton('Save')"
            End If
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        End If
        If Not IsPostBack Then
            Dim PauseIntervalID As Integer = 0
            Integer.TryParse(Request.QueryString("PauseIntervalID"), PauseIntervalID)
            RenderPauseIntervalDetails(PauseIntervalID)
        End If

    End Sub

    Protected Sub ChkBoxOccursEveryDay_CheckedChanged(sender As Object, e As EventArgs)
        RadDateTimePickerOccursFrom.Enabled = Not ChkBoxOccursEveryDay.Checked
        RadDateTimePickerOccursTo.Enabled = Not ChkBoxOccursEveryDay.Checked
        If (ChkBoxOccursEveryDay.Checked) Then
            RadDateTimePickerOccursTo.SelectedDate = Nothing
            RadDateTimePickerOccursFrom.SelectedDate = Nothing
        Else
            RadDateTimePickerOccursFrom.SelectedDate = DateTime.Now().AddHours(1)
            RadDateTimePickerOccursTo.SelectedDate = DateTime.Now().AddHours(3)
        End If

    End Sub


    Protected Sub RenderPauseIntervalDetails(pauseIntervalID As Integer)
        If pauseIntervalID > 0 Then
            Dim pauseInterval As MonitoringMessagePauseInterval = MonitoringManager.GetMonitoringMessagePauseInterval(pauseIntervalID, ConfigurationManager.ConnectionStrings("LogDB").ConnectionString,
                                                                                                ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(),
                                                                                                ConfigurationManager.AppSettings("ErrorEmailTo").ToString(),
                                                                                                ConfigurationManager.AppSettings("SMTPServer").ToString())
            ChkBoxOccursEveryDay.Checked = pauseInterval.OccursEveryDay
            txtStartOccurenceHour.Text = pauseInterval.StartPauseTimeHour.ToString()
            txtStartOccurenceMinute.Text = pauseInterval.StartPauseTimeMinute.ToString()
            txtEndOccurenceHour.Text = pauseInterval.EndPauseTimeHour.ToString()
            txtEndOccurenceMinute.Text = pauseInterval.EndPauseTimeMinute.ToString()
            ChkBoxOccursEveryDayEdit_CheckedChanged(Nothing, Nothing)
        End If
    End Sub

    Protected Sub ChkBoxOccursEveryDayEdit_CheckedChanged(sender As Object, e As EventArgs)
        RadDateTimePickerOccursFrom.Enabled = Not ChkBoxOccursEveryDay.Checked
        RadDateTimePickerOccursTo.Enabled = Not ChkBoxOccursEveryDay.Checked
        If (ChkBoxOccursEveryDay.Checked) Then
            RadDateTimePickerOccursTo.SelectedDate = Nothing
            RadDateTimePickerOccursFrom.SelectedDate = Nothing
        Else
            RadDateTimePickerOccursFrom.SelectedDate = DateTime.Now().AddHours(1)
            RadDateTimePickerOccursTo.SelectedDate = DateTime.Now().AddHours(3)
        End If

    End Sub

    Protected Sub BtnSavePauseInterval_Click(sender As Object, e As EventArgs)
        Dim PauseIntervalID As Integer = 0
        Integer.TryParse(Request.QueryString("PauseIntervalID"), PauseIntervalID)
        SaveOrUpdatePauseInterval(PauseIntervalID)
    End Sub
    Private Sub SaveOrUpdatePauseInterval(pauseIntervalID As Integer)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim UID As Guid = Guid.Empty
        Dim IsUIDWorkflowID As Boolean
        Guid.TryParse(Request.QueryString("UID"), UID)
        Boolean.TryParse(Request.QueryString("IsUIDWorkflowID"), IsUIDWorkflowID)

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@PauseIntervalID", pauseIntervalID))
        parameters.Add(New SqlParameter("@MessageID", UID))
        parameters.Add(New SqlParameter("@OccursEveryDay", ChkBoxOccursEveryDay.Checked))
        parameters.Add(New SqlParameter("@StartPauseTimeHour", txtStartOccurenceHour.Value))
        parameters.Add(New SqlParameter("@StartPauseTimeMinute", txtStartOccurenceMinute.Value))
        parameters.Add(New SqlParameter("@EndPauseTimeHour", txtEndOccurenceHour.Value))
        parameters.Add(New SqlParameter("@EndPauseTimeMinute", txtEndOccurenceMinute.Value))
        parameters.Add(New SqlParameter("@IsMessageIDWorkflowID", IsUIDWorkflowID))
        If Not ChkBoxOccursEveryDay.Checked Then
            parameters.Add(New SqlParameter("@StartDayOfOccurence", RadDateTimePickerOccursFrom.SelectedDate))
            parameters.Add(New SqlParameter("@EndDayOfOccurence", RadDateTimePickerOccursTo.SelectedDate))
        End If

        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Monitoring.SaveOrUpdatePauseInterval", parameters)
        Dim TempPauseIntervals = New List(Of MonitoringMessagePauseInterval)
        For Each row As DataRow In dataTable.Rows
            TempPauseIntervals.Add(MonitoringManager.GetMonitoringMessagePauseIntervalFromDataRow(row))
        Next
        HttpContext.Current.Session("pauseIntervals_" + UID.ToString()) = TempPauseIntervals

        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close dialog", "FinishChange();", True)
        watch.Stop()
        If pauseIntervalID = 0 Then
            ClsHelper.Log(String.Format("Save{0}Pause Interval", IIf(IsUIDWorkflowID, " Workflow ", " ")), ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
        Else
            ClsHelper.Log(String.Format("Update{0}Pause Interval", IIf(IsUIDWorkflowID, " Workflow ", " ")), ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>{1} ID: </b>{0}", UID, IIf(IsUIDWorkflowID, "Workflow", "Message")), watch.ElapsedMilliseconds, False, Nothing)
        End If

    End Sub

End Class
