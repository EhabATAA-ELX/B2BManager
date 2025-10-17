
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class EbusinessDeleteCustomerRange
    Inherits System.Web.UI.Page


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim CustomerName As String = ""
        If Not IsPostBack Then
            If Context.Request.QueryString("CustomerName") IsNot Nothing Then
                CustomerName = Context.Request.QueryString("CustomerName")
            End If
            CType(Master.FindControl("title"), HtmlTitle).Text = "Reset Customer Range Timestamp"
            Lbl_Title.Text = "Are you sure you wish to reset Customer Range Timestamp for this company :   <span class=""name"">" + CustomerName + "</span>"
        End If
    End Sub

    Protected Sub BtnDeleteimeStampCusRange_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        Dim C_GlobalID As String = Context.Request.QueryString("C_GlobalID")
        Dim res As Boolean = DeleteTimeStampCustomerRange(EnvironmentID, C_GlobalID)
        If res Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow();", True)
        Else
            If res = False Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopupCustomerRange('An error occurred during the customer range timestamp deletion process.</br>Please contact your B2B support.');", True)
            Else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Your session has expired please login again');", True)
            End If
        End If
    End Sub

    Protected Function DeleteTimeStampCustomerRange(ByVal EnvironmentID As Integer, ByVal C_GlobalID As String) As Boolean?
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim SOPNAME As String = Context.Request.QueryString("SOPNAME")
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If logonUser Is Nothing Then
            Return Nothing
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@C_GLOBALID", C_GlobalID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[PU_COMPANY_TIMESTAMP_CR]", parameters) Then
            ClsHelper.Log("Delete TimeStamp CustomerRange", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            ClsHelper.LogEbusinessAction(EnvironmentID, SOPNAME, "Delete TimeStamp CustomerRange", logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Return True
        Else
            ClsHelper.Log("Delete TimeStamp CustomerRange", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            ClsHelper.LogEbusinessAction(EnvironmentID, SOPNAME, "Delete TimeStamp CustomerRange", logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            Return False
        End If
    End Function
End Class
