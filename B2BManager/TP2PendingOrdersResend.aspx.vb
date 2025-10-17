
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports SAPRequestsLib

Partial Class TP2PendingOrdersResend
    Inherits System.Web.UI.Page
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Resend Pending Order Confirmation"
            Dim NbOrders As Integer
            If Context.Request.QueryString("NBORDERS") IsNot Nothing Then
                Integer.TryParse(Context.Request.QueryString("NBORDERS"), NbOrders)
            End If
            Nb_PendingOrders.Value = NbOrders
            ScriptManager.GetCurrent(Me.Page).AsyncPostBackTimeout = 600000
        End If
    End Sub

    Public Sub SendRequestToSap(sender As Object, e As EventArgs) Handles BtnSendRequestToSap.Click
        Dim EnvironmentID As Integer
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        Dim sopID = Context.Request.QueryString("SOPID")
        Dim res = ResendOrder(EnvironmentID, sopID)
        If Not res Then
            lblInfo.Text = "An unexpected error has occurred, please retry later"
            Return
        End If
        ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow();", True)
    End Sub


    Protected Function GetXMLParameters(ByVal EnvironmentID As Integer) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("ActionName", "PLACE_SALESORDER_ASYNC"))
        parameters.Add(New SqlParameter("EnvironmentID", EnvironmentID))
        Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("Logger.GetXMLParameters", parameters)
        Return dt
    End Function


    Protected Function ResendOrder(ByVal EnvironmentID As Integer, ByVal SOPID As String) As Boolean
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim DtParameters As DataTable = GetXMLParameters(EnvironmentID)
        If DtParameters IsNot Nothing And DtParameters.Rows.Count > 0 Then
            Dim wcfUrl As String = DtParameters.Rows(0)("SAPWCFURL").ToString()
            Dim wcfUserName As String = DtParameters.Rows(0)("SAPWCFUSERNAME").ToString()
            Dim wcfPassword As String = DtParameters.Rows(0)("SAPWCFPSSWORD").ToString()
            Dim wcfMethodName As String = DtParameters.Rows(0)("SAPWCFMETHOD").ToString()
            Dim PendingOrders As DataTable = ClsMaintenanceHelper.GetPendingOrders(EnvironmentID, SOPID)
            For Each row In PendingOrders.Rows
                Dim xmlRequest As String = ClsHelper.PrettyXml(row("XmlB2B").ToString())
                Dim SAPReplyResult As SAPReplyResult = SAPRequester.SendSAPMessage(xmlRequest, wcfUrl, wcfMethodName, wcfUserName, wcfPassword, sopname:=SOPID)
                If SAPReplyResult.HasError Then
                    ClsHelper.Log("Resend TP2 Order Async To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                    Return False
                Else
                    ClsHelper.Log("Resend TP2Order Async To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watcher.ElapsedMilliseconds, False, Nothing)
                    LogResend(EnvironmentID, row("CorID"), row("Country"), row("CustCode"), row("CustName"))
                End If
                Threading.Thread.Sleep(ConfigurationManager.AppSettings("IntervalTimeToSendOrder").ToString())
            Next
        End If
        Return True
    End Function

    Protected Sub LogResend(ByVal EnvironmentID As Integer, ByVal Correl_ID As String, ByVal Country As String, ByVal CustomerCode As String, ByVal CustomerName As String)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@CorrelID", Correl_ID))
        parameters.Add(New SqlParameter("@Country", Country))
        parameters.Add(New SqlParameter("@CustomerCode", CustomerCode))
        parameters.Add(New SqlParameter("@CustomerName", CustomerName))
        ClsDataAccessHelper.ExecuteNonQuery("[Maintenance].[LogResendOrderTP2]", parameters)
    End Sub
End Class
