Imports Telerik.Web.UI

Public Class RankedRadTreeNode
    Inherits RadTreeNode

    'Rank for children of the same parentNode
    Public Rank As Integer

    Public Sub New(ByVal text As String, ByVal value As String, Optional rank As Integer = 1)
        MyBase.New(text, value)
        Me.Rank = rank
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

End Class