
Partial Class _404
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        If ClsSessionHelper.LogonUser IsNot Nothing AndAlso Not Request.Url.PathAndQuery.ToLower().Contains("error") Then
            Me.MasterPageFile = "~/MasterPage.master"
        End If
    End Sub
End Class
