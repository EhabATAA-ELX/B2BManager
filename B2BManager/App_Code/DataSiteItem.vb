
Public Class SiteDataItem
    Private text1 As String
    Private id1 As Integer
    Private parentId1 As Integer

    Public Property Text() As String
        Get
            Return text1
        End Get
        Set(ByVal value As String)
            text1 = value
        End Set
    End Property


    Public Property ID() As Integer
        Get
            Return id1
        End Get
        Set(ByVal value As Integer)
            id1 = value
        End Set
    End Property

    Public Property ParentID() As Integer
        Get
            Return parentId1
        End Get
        Set(ByVal value As Integer)
            parentId1 = value
        End Set
    End Property

    Public Sub New(ByVal id As Integer, ByVal parentId As Integer, ByVal text As String)
        Me.id1 = id
        Me.parentId1 = parentId
        Me.text1 = text
    End Sub
End Class