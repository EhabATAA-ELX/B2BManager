Imports Microsoft.VisualBasic

Public Class ClsTestCaseUserControl
    Inherits System.Web.UI.UserControl
    Private _testCaseID As Integer

    Public Property TestCaseID As Integer
        Get
            Return _testCaseID
        End Get
        Set(value As Integer)
            _testCaseID = value
        End Set
    End Property
End Class
