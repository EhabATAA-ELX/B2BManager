
Partial Class EbusinessNewContact
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim cid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
                Guid.TryParse(Request.QueryString("cid"), cid)
            End If
            If cid <> Guid.Empty Then
                CType(Master.FindControl("title"), HtmlTitle).Text = "Duplicate Contact"
            Else
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Contact"
            End If
        End If
    End Sub

End Class
