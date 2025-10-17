
Partial Class EbusinessUsersList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
            If clsUser Is Nothing Then
                Return
            End If
            DeleteUserScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SUPER_USER) Or clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_USER)
        End If
    End Sub

End Class
