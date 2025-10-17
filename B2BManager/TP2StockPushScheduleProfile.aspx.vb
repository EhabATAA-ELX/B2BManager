
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TP2StockPushScheduleProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim globalid As Guid = Guid.Empty
            Dim tpcid As Integer = 0
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("tpcid")) Then
                Integer.TryParse(Request.QueryString("tpcid"), tpcid)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), globalid)
            End If

            If EnvironmentID > 0 Then
                RenderControls(EnvironmentID)
                If tpcid > 0 AndAlso globalid <> Guid.Empty Then
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Push Stock Schedule Setting"
                    btnSubmit.Text = "Update"
                    btnSubmit.OnClientClick = "ProcessButton('Update')"
                    LoadPushStockData(EnvironmentID, globalid, tpcid)
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "New Push Stock Schedule Setting"
                    btnSubmit.Text = "Save"
                    btnSubmit.OnClientClick = "ProcessButton('Add')"
                End If
            End If

        End If
    End Sub

    Private Sub RenderControls(environmentID As Integer)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.EbusinessSopID, False, False)
    End Sub
    Private Sub LoadPushStockData(environmentID As Integer, globalid As Guid, tpcid As Integer)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@globalid", globalid))
        parameters.Add(New SqlParameter("@tpcid", tpcid))
        Dim data As DataTable = ClsDataAccessHelper.FillDataTable("TP2StockPush.GetPushStockData", parameters)
        If data IsNot Nothing Then
            If data.Rows.Count = 1 Then
                txtBoxTradeplaceID.Text = data.Rows(0)("TPID")
                txtBoxCustomerCode.Text = data.Rows(0)("C_CUID")
                chkBoxScheduleActive.Checked = data.Rows(0)("IS_ACTIF")
                ddlCountry.SelectedValue = data.Rows(0)("S_SOP_ID")
                txtBoxTradeplaceID.Enabled = False
                txtBoxCustomerCode.Enabled = False
                ddlCountry.Enabled = False
                If data.Rows(0)("SCHEDULECONFIGID") IsNot DBNull.Value Then
                    ScheduleTime.SelectedDate = DateTime.ParseExact(data.Rows(0)("TIME").ToString(), ScheduleTime.TimeView.TimeFormat, Nothing)
                    applyTimeChangeToSchedule.Visible = True
                    applyTimeChangeToSchedule.Text = "Apply time change to all schedules with time " + ScheduleTime.SelectedDate.ToString()
                End If
                statusInfo.InnerHtml = "<span style='font-size:9pt'>" + data.Rows(0)("TPC_Name") + "</span> <img src='images/ok.gif' />"
            Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        lblErrorInfo.Text = " "
        Dim EnvironmentID As Integer = 0
        Dim globalid As Guid = Guid.Empty
        Dim tpcid As Integer = 0
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("tpcid")) Then
            Integer.TryParse(Request.QueryString("tpcid"), tpcid)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            Guid.TryParse(Request.QueryString("id"), globalid)
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If tpcid > 0 AndAlso globalid <> Guid.Empty Then
            parameters.Add(New SqlParameter("@tpcid", tpcid))
            parameters.Add(New SqlParameter("@globalid", globalid))
        End If
        parameters.Add(New SqlParameter("@SOPID", ddlCountry.SelectedValue))
        parameters.Add(New SqlParameter("@tradeplaceid", txtBoxTradeplaceID.Text))
        parameters.Add(New SqlParameter("@customercode", txtBoxCustomerCode.Text))
        parameters.Add(New SqlParameter("@IsActive", chkBoxScheduleActive.Checked))
        If ScheduleTime.SelectedDate IsNot Nothing Then
            parameters.Add(New SqlParameter("@ScheduleTime", ScheduleTime.SelectedDate.Value.ToString("HH:mm")))
            parameters.Add(New SqlParameter("@ApplyChangeToSchedule", applyTimeChangeToSchedule.Checked))
        End If
        Dim result As String = ClsDataAccessHelper.ExecuteScalar("TP2StockPush.SaveOrUpdatePushStockSchedule", parameters)
        Select Case result
            Case "Success"
                ClsHelper.Log(IIf(tpcid > 0 AndAlso globalid <> Guid.Empty, "Update", "Save") + " Push Stock Schedule", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.Finish(" + IIf(tpcid > 0 AndAlso globalid <> Guid.Empty, "'Update'", "'Save'") + ");", True)
            Case Nothing
                ClsHelper.Log(IIf(tpcid > 0 AndAlso globalid <> Guid.Empty, "Update", "Save") + "  Push Stock Schedule", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                lblErrorInfo.Text = "An unexpected error has occurred. Please try again later."
            Case "Unkown Tradeplace ID"
                lblErrorInfo.Text = "The Tradeplace ID you entred is not in the system"
            Case "Unkown Customer ID"
                lblErrorInfo.Text = "The customer code you entred is not in the system"
            Case Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.ConfirmExistingMapping('" + result.Split("|")(0) + "','" + result.Split("|")(1) + "');", True)
        End Select
    End Sub
End Class
