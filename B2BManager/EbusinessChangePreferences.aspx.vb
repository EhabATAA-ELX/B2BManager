
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class EbusinessChangePreferences
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Change Preferences"
        End If
    End Sub
End Class
