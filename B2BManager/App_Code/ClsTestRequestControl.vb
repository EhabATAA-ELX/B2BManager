Imports Microsoft.VisualBasic

Public Class ClsTestRequestControl
    Inherits System.Web.UI.UserControl

    Private _Request As ClsAutomatedTestsHelper.TestRequest
    Private _IsTestRequest As Boolean

    Public Property TestRequest As ClsAutomatedTestsHelper.TestRequest
        Get
            Return _Request
        End Get
        Set(value As ClsAutomatedTestsHelper.TestRequest)
            _Request = value
        End Set
    End Property

    Public Property IsTestRequest As Boolean
        Get
            Return _IsTestRequest
        End Get
        Set(value As Boolean)
            _IsTestRequest = value
        End Set
    End Property

End Class
