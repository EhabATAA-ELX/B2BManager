
Imports System.Data

Partial Class UserControls_ContactPreview
    Inherits System.Web.UI.UserControl

    Private _DataSource As DataTable

    Public Property DataSource As DataTable
        Get
            Return _DataSource
        End Get
        Set(value As DataTable)
            _DataSource = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If DataSource IsNot Nothing Then
            rptContact.DataSource = DataSource
            rptContact.DataBind()
        End If
    End Sub

    Public Sub DataBindSource(dt As DataTable)
        rptContact.DataSource = dt
        rptContact.DataBind()
    End Sub

End Class
