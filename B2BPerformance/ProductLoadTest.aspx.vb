
Partial Class ProductLoadTest
    Inherits System.Web.UI.Page

    Protected Sub CleanUpSessionButton_Click(sender As Object, e As EventArgs)
        Session.Clear()
    End Sub
End Class
