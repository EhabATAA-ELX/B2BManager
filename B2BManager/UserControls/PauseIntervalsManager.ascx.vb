
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports SAPRequestsLib
Imports SAPRequestsLib.Models

Partial Class UserControls_PauseIntervalsManager
    Inherits System.Web.UI.UserControl

    Private _UID As Guid
    Private _IsUIDWorkflowID As Boolean

    Public Property pauseIntervals As List(Of MonitoringMessagePauseInterval)
        Get
            Dim value As Object = HttpContext.Current.Session("pauseIntervals_" + UID.ToString())

            If Not value Is Nothing Then
                Return CType(value, List(Of MonitoringMessagePauseInterval))
            Else
                Return New List(Of MonitoringMessagePauseInterval)
            End If
        End Get
        Set(value As List(Of MonitoringMessagePauseInterval))
            HttpContext.Current.Session("pauseIntervals_" + UID.ToString()) = value
        End Set
    End Property

    Public Property UID As Guid
        Get
            Return _UID
        End Get
        Set(value As Guid)
            _UID = value
        End Set
    End Property

    Public Property IsUIDWorkflowID As Boolean
        Get
            Return _IsUIDWorkflowID
        End Get
        Set(value As Boolean)
            _IsUIDWorkflowID = value
        End Set
    End Property


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        End If

        CurrentUID.Value = UID.ToString()
        CurrentIsUIDWorkflowID.Value = IsUIDWorkflowID.ToString()

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        WindowDeletePauseInterval.VisibleOnPageLoad = False

        If Not IsPostBack Then
            BindPauseIntervals()
        ElseIf "DeletePauseInterval".Equals(__EVENTTARGET) And __EVENTARGUMENT IsNot Nothing Then
            Dim pauseIntervalID As Integer = 0
            Integer.TryParse(__EVENTARGUMENT, pauseIntervalID)
            WindowDeletePauseInterval.VisibleOnPageLoad = True
            lblPauseIntervalIDToDelete.Text = pauseIntervalID.ToString()
        Else
            BindPauseIntervals()
        End If
    End Sub

    Private Sub BindPauseIntervals()
        monitoringPauseIntervalsGrid.DataSource = pauseIntervals
        monitoringPauseIntervalsGrid.DataBind()
    End Sub

    Protected Sub BtnDeletePauseInterval_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@PauseIntervalID", Integer.Parse(lblPauseIntervalIDToDelete.Text)))
        parameters.Add(New SqlParameter("@MessageID", UID))

        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Monitoring.DeletePauseInterval", parameters)

        Dim TempPauseIntervals = New List(Of MonitoringMessagePauseInterval)
        For Each row As DataRow In dataTable.Rows
            TempPauseIntervals.Add(MonitoringManager.GetMonitoringMessagePauseIntervalFromDataRow(row))
        Next
        pauseIntervals = TempPauseIntervals

        BindPauseIntervals()

        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "ShowOrClosePauseIntervalWindow('DeletePauseInterval',false);", True)
        watch.Stop()
        ClsHelper.Log(String.Format("Delete{0}Pause Interval", IIf(IsUIDWorkflowID, " Workflow ", " ")), ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>{1} ID: </b>{0}", UID, IIf(IsUIDWorkflowID, "Workflow", "Message")), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

End Class
