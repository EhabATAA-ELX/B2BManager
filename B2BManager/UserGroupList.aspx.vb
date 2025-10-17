
Partial Class UserGroupList
    Inherits System.Web.UI.Page
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim Uid As Integer = 0
            If Not String.IsNullOrEmpty(Request.QueryString("Uid")) Then
                Integer.TryParse(Request.QueryString("Uid"), Uid)
            End If
            If Uid > 0 Then
                hdfSelectedGroup.Value = Uid
            End If
        End If
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            userglobalid.Value = ClsSessionHelper.LogonUser.GlobalID.ToString()
        End If
    End Sub
End Class
