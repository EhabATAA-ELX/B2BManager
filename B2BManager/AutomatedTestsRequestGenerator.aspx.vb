
Partial Class AutomatedTestsRequestGenerator
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Request(s) Generator"
        End If
    End Sub
End Class
