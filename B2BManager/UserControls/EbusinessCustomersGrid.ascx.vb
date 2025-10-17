
Partial Class UserControls_EbusinessCustomersGrid
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
    End Sub
End Class
