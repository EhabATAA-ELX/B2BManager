
Imports System.Data
Imports System.Diagnostics
Imports System.Threading
Imports System.Xml

Partial Class Maintenance
    Inherits System.Web.UI.Page

    Private ReadOnly B2B_TEMPLATE_FILE_PATH As String = Server.MapPath("~/App_Data/Maintenance/app_offline{0}.htm")
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        If Not IsPostBack Or "RenderControls".Equals(__EVENTARGUMENT) Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
            If clsUser Is Nothing Then
                Return
            End If
            RenderControls(clsUser)
            ClsHelper.Log("Access Maintenance Page", clsUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        Else
            If Not String.IsNullOrEmpty(target) Then
                If target.ToLower().Contains("btn") Then
                    Exit Sub
                End If
            End If
            LoadInstances()
        End If
    End Sub

    Private Sub RenderControls(clsUser As ClsUser)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, clsUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, Nothing, True)
    End Sub

    Private countryHtmlSortedList As SortedList
    Private HasBiliInstances As Boolean
    Private HasB2BInstances As Boolean
    Private HasAEGInstances As Boolean
    Private HasElectroluxInstances As Boolean
    Private Sub LoadInstances(Optional B2BMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                              Optional BiliMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                              Optional AEGMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                              Optional ElectroluxMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read)
        Dim dataset As DataSet = ClsMaintenanceHelper.GetInstancesByEnvironmentID(ddlEnvironment.SelectedValue, ddlCountry.SelectedValue, Cache)
        Dim LogonUser As ClsUser = ClsSessionHelper.LogonUser
        If LogonUser Is Nothing Then
            Exit Sub
        End If
        Dim html As String = ""
        Dim UserGlobalID As String = LogonUser.GlobalID.ToString()
        countryHtmlSortedList = New SortedList()
        HasBiliInstances = False
        HasB2BInstances = False
        HasAEGInstances = False
        HasElectroluxInstances = False
        Dim childrenThreads As List(Of Thread) = New List(Of Thread)()
        If dataset.Tables.Count = 2 Then
            If (dataset.Tables(0).Rows.Count > 0) Then
                html += "<ul class=""storeapp-list"">"
                If dataset.Tables(0).Rows.Count = 1 Then
                    GetCountryInstances(dataset.Tables(0).Rows(0), dataset, ddlMaintenanceMode.SelectedValue, B2B_TEMPLATE_FILE_PATH, UserGlobalID, ddlEnvironment.SelectedValue, B2BMode, BiliMode, AEGMode, ElectroluxMode)
                Else 'use multithreading only when more than 1 instance
                    For Each row In dataset.Tables(0).Rows
                        Dim actionThread As Thread = New Thread(Sub(p) GetCountryInstances(row, dataset, ddlMaintenanceMode.SelectedValue, B2B_TEMPLATE_FILE_PATH, UserGlobalID, ddlEnvironment.SelectedValue, B2BMode, BiliMode, AEGMode, ElectroluxMode))
                        actionThread.Start()
                        childrenThreads.Add(actionThread)
                    Next
                    For Each thread As Thread In childrenThreads
                        thread.Join()
                    Next
                End If
                If countryHtmlSortedList.Count > 0 Then
                    For Each value In countryHtmlSortedList.Values
                        html += value
                    Next
                End If
                html += "</ul></br>"
            End If
        End If
        tdB2BInMaintenance.Visible = HasB2BInstances
        tdB2BUp.Visible = HasB2BInstances
        tdBiliInMaintenance.Visible = HasBiliInstances
        tdBiliUp.Visible = HasBiliInstances

        tdAEGInMaintenance.Visible = HasAEGInstances
        tdAEGUp.Visible = HasAEGInstances
        tdElectroluxInMaintenance.Visible = HasElectroluxInstances
        tdElectroluxUp.Visible = HasElectroluxInstances

        If HasBiliInstances Then
            Dim CountNBPending As Integer = ClsMaintenanceHelper.GetNBPendingOrders(ddlEnvironment.SelectedValue, ddlCountry.SelectedValue)
            tdNBPendingOrdersToResend.Visible = True
            If CountNBPending > 0 Then
                LblNBOrdersToResend.Text = CountNBPending.ToString() + " Order(s) to Process"
                tdBiliResendOrderMaintenance.Visible = True
            Else
                LblNBOrdersToResend.Text = ""
                tdBiliResendOrderMaintenance.Visible = False
            End If
        Else
            tdNBPendingOrdersToResend.Visible = False
        End If
        htmlInfo.InnerHtml = html
        ddlCountry.Enabled = True
        ddlEnvironment.Enabled = True
    End Sub

    Private Sub GetCountryInstances(row As DataRow,
                                    dataset As DataSet,
                                    mode As String,
                                    B2bTemplateFilePath As String,
                                    UserGlobalID As String,
                                    EnvironmentID As Integer,
                                    Optional B2BMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                                    Optional BiliMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                                    Optional AEGMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read,
                                    Optional ElectroluxMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read)
        Dim htmlResult As String = ClsMaintenanceHelper.GetInstancesHtml(dataset, row("SOP_ID"), mode, row("Name"), UserGlobalID, EnvironmentID, HasB2BInstances, HasBiliInstances, HasAEGInstances, HasElectroluxInstances, B2BMode, BiliMode, AEGMode, ElectroluxMode, Nothing, B2bTemplateFilePath)
        SyncLock countryHtmlSortedList
            countryHtmlSortedList.Add(row("Name"), htmlResult)
        End SyncLock
    End Sub

    Protected Sub TriggerChange_Click(sender As Object, e As EventArgs)
        Dim CommandName As String = DirectCast(sender, LinkButton).CommandName
        Dim CommandArgument As String = DirectCast(sender, LinkButton).CommandArgument
        Dim mode As ClsMaintenanceHelper.Mode = IIf(CommandName.Equals("Start"), ClsMaintenanceHelper.Mode.StartInstance, ClsMaintenanceHelper.Mode.StopInstance)
        LoadInstances(
            IIf(CommandArgument.Equals("B2B"), mode, ClsMaintenanceHelper.Mode.Read),
            IIf(CommandArgument.Equals("Bili"), mode, ClsMaintenanceHelper.Mode.Read),
            IIf(CommandArgument.Equals("AEG"), mode, ClsMaintenanceHelper.Mode.Read),
            IIf(CommandArgument.Equals("Electrolux"), mode, ClsMaintenanceHelper.Mode.Read)
            )
    End Sub
End Class
