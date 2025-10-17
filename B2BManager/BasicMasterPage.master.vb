Imports System.Security.Principal
Partial Class BasicMasterPage
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim HideHeader As Boolean = False
        Boolean.TryParse(Request.QueryString("HideHeader"), HideHeader)

        If HideHeader Then
            headerPanel.Visible = False
        End If

    End Sub

End Class

