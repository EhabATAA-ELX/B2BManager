Imports Microsoft.VisualBasic

Public Class ClsHelper

    Public Shared Function GetRandomNumber() As Integer
        Dim myRandom As New Random
        Dim RandomNumber As Integer = myRandom.Next(1, 150) * 10
        Return RandomNumber
    End Function

End Class
