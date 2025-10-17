
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Newtonsoft.Json
Imports SAPRequestsLib

Partial Class B2BPendingOrdersResend
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Resend Pending Order Confirmation"
        End If
    End Sub

    Public Sub SendRequestToSap(sender As Object, e As EventArgs) Handles BtnSendRequestToSap.Click

        Dim EnvironmentID As Integer
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Context.Request.QueryString("List")) AndAlso EnvironmentID > 0 Then
            Dim Collection = JsonConvert.DeserializeObject(Context.Request.QueryString("List"))
            For index As Integer = 0 To Collection.Count - 1
                Dim item = Collection(index)
                Dim res = ResendOrder(EnvironmentID, item.GetValue("Correl_ID").ToString(), item.GetValue("SOP").ToString(), item.GetValue("U_GLOBALID").ToString())
                If Not res Then
                    lblInfo.Text = "An unexpected error has occurred, please retry later"
                    Return
                End If
            Next
            ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow('WindowSendToSAP');", True)
        Else
            If Not String.IsNullOrEmpty(Page.Request.QueryString("Correl_ID")) AndAlso EnvironmentID > 0 Then
                Dim res = ResendOrder(EnvironmentID, Page.Request.QueryString("Correl_ID"), Page.Request.QueryString("SopID"), Page.Request.QueryString("GlobalID"))
                If res Then
                    ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow('WindowSendToSAP');", True)
                Else
                    lblInfo.Text = "An unexpected error has occurred, please retry later"
                End If
            End If
        End If
    End Sub


    Protected Function ResendOrder(ByVal EnvironmentID As Integer, ByVal Correl_ID As String, ByVal SopID As String, ByVal GlobalID As String) As Boolean
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("ActionName", "PLACE_SALESORDER_ASYNC"))
        parameters.Add(New SqlParameter("EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("CorrelID", Correl_ID))
        parameters.Add(New SqlParameter("TableName", "T_CONFIRM_ORDER_ASYNC"))
        parameters.Add(New SqlParameter("SopID", SopID))
        parameters.Add(New SqlParameter("GlobalID", GlobalID))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("Logger.GetXMLInformation", parameters)
        If dataSet.Tables.Count = 3 Then
            Dim dTParameters As DataTable = dataSet.Tables(0)
            Dim dTRequest As DataTable = dataSet.Tables(1)
            If dTParameters.Rows.Count = 1 AndAlso dTRequest.Rows.Count > 0 Then
                Dim wcfUrl As String = dTParameters.Rows(0)("SAPWCFURL").ToString()
                Dim wcfUserName As String = dTParameters.Rows(0)("SAPWCFUSERNAME").ToString()
                Dim wcfPassword As String = dTParameters.Rows(0)("SAPWCFPSSWORD").ToString()
                Dim wcfMethodName As String = dTParameters.Rows(0)("SAPWCFMETHOD").ToString()
                Dim SessionID As String = IIf(dTParameters.Rows(0)("SESSIONID").ToString().Equals(""), ConfigurationManager.AppSettings("SessionID"), dTParameters.Rows(0)("SESSIONID").ToString())
                Dim xmlRequest As String = ClsHelper.PrettyXml(dTRequest.Rows(0)("MSG_XML").ToString())
                Dim SAPReplyResult As SAPReplyResult = SAPRequester.SendSAPMessage(xmlRequest, wcfUrl, wcfMethodName, wcfUserName, wcfPassword, GlobalID, SessionID, SopID)
                If SAPReplyResult.HasError Then
                    ClsHelper.Log("Resend Order Async To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                    Return False
                Else
                    ClsHelper.Log("Resend Order Async To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
                    LogResend(Correl_ID, EnvironmentID)
                    Return True
                End If
                ClsHelper.Log("Resend Order Async To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                Return False
            End If
        End If
        Return False
    End Function
    Protected Sub LogResend(ByVal Correl_ID As String, ByVal EnvironmentID As Integer)
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@Correl_ID", Correl_ID))
        parameters.Add(New SqlParameter("@U_GLOBALID", ClsSessionHelper.LogonUser.GlobalID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        ClsDataAccessHelper.ExecuteNonQuery("[B2BOrders].[LogResendOrder]", parameters)
    End Sub
End Class
