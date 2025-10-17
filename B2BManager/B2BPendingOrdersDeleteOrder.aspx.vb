
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Newtonsoft.Json

Partial Class B2BPendingOrdersDeleteOrder
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Delete Pending Order Confirmation"
        End If

    End Sub
    Protected Sub BtnDeleteOrder_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Context.Request.QueryString("List")) AndAlso EnvironmentID > 0 Then
            Dim Collection = JsonConvert.DeserializeObject(Context.Request.QueryString("List"))
            For index As Integer = 0 To Collection.Count - 1
                Dim item = Collection(index)
                Dim res = DeleteOrder(item.GetValue("Correl_ID").ToString(), EnvironmentID, txtBoxDeletionReason.Text)
                If Not res Then
                    lblInfo.Text = "An unexpected error has occurred, please retry later"
                    Return
                End If
            Next
            ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow('WindowDeleteOrder');", True)
        Else
            If Not String.IsNullOrEmpty(Page.Request.QueryString("Correl_ID")) AndAlso EnvironmentID > 0 Then
                Dim res As Boolean = DeleteOrder(Page.Request.QueryString("Correl_ID"), EnvironmentID, txtBoxDeletionReason.Text)
                If res Then
                    ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow('WindowDeleteOrder');", True)
                Else
                    lblInfo.Text = "An unexpected error has occurred, please retry later"
                End If
            End If
        End If


    End Sub

    Protected Function DeleteOrder(ByVal Correl_ID As String, ByVal EnvironmentID As String, ByVal txtBoxDeletionReason As String) As Boolean
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@Correl_ID", Correl_ID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@REASON_Remove", txtBoxDeletionReason))
        If ClsDataAccessHelper.ExecuteNonQuery("[B2BOrders].[RemovePendingOrder]", parameters) Then
            ClsHelper.Log("Remove Pending Order", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Return True
        Else
            ClsHelper.Log("Remove Pending Order", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            Return False
        End If
    End Function
End Class
