
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class EbusinessDeleteCachePrice
    Inherits System.Web.UI.Page


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim CustomerName As String = ""
        If Not IsPostBack Then
            If Context.Request.QueryString("CustomerName") IsNot Nothing Then
                CustomerName = Context.Request.QueryString("CustomerName")
            End If
            CType(Master.FindControl("title"), HtmlTitle).Text = "Delete Price Cache"
            Lbl_Title.Text = "Are you sure you wish to empty price cache for this company :   <span class=""name"">" + CustomerName + "</span>"
        End If
    End Sub
    Protected Sub BtnPriceCache_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        Dim SOPNAME As String = Context.Request.QueryString("SOPNAME")
        Dim CUSTOMERCODE As String = Context.Request.QueryString("CUSTOMERCODE")
        Dim res As Boolean = DeletePriceCache(SOPNAME, CUSTOMERCODE, EnvironmentID)
        If res Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow();", True)
        Else
            If res = False Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopupPrice('An error occurred during the price cache deletion process.</br>Please contact your B2B support.');", True)
            Else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Your session has expired please login again');", True)
            End If
        End If
    End Sub

    Protected Function DeletePriceCache(ByVal SOPNAME As String, ByVal CUSTOMERCODE As String, ByVal EnvironmentID As Integer) As Boolean?
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If logonUser Is Nothing Then
            Return Nothing
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@SOPNAME", SOPNAME))
        parameters.Add(New SqlParameter("@CUSTOMERCODE", CUSTOMERCODE))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_Remove_PriceCache]", parameters) Then
            ClsHelper.Log("Delete price cache", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            ClsHelper.LogEbusinessAction(EnvironmentID, SOPNAME, "Delete price cache", logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Return True
        Else
            ClsHelper.Log("Delete price cache", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            ClsHelper.LogEbusinessAction(EnvironmentID, SOPNAME, "Delete price cache", logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            Return False
        End If
    End Function

End Class
