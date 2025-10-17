Imports Microsoft.VisualBasic
Imports ClsHelper

<Serializable>
Public Class ClsUser

    Private _globalID As Guid = Guid.Empty
    Private _previewedBy As Guid = Guid.Empty

    Public ID As Integer
    Public Property GlobalID As Guid
        Set(value As Guid)
            _globalID = value
        End Set
        Get
            Return _globalID
        End Get
    End Property

    Public Property PreviewedBy As Guid
        Set(value As Guid)
            _previewedBy = value
        End Set
        Get
            Return _previewedBy
        End Get
    End Property


    Public Title As Integer
    Public Login As String
    Public Password As String
    Public FirstName As String
    Public LastName As String
    Public FullName As String
    Public NickName As String
    Public Email As String
    Public HomePageToolID As Integer
    Public DefaultDashboardID As Integer
    Public HomePageChartID As Integer
    Public DefaultEnvironmentID As Integer
    Public DefaultSOPIDs As String
    Public DefaultCountrtySplitStatus As Boolean
    Public DefaultEbusinessEnvironmentID As Integer
    Public DefaultEbusinessSopID As String
    Public DefaultEbusinessManagementType As Integer
    Public ExpandRowsOnSearchByDefault As Boolean
    Public ActivateWindowModeByDefault As Boolean
    Public IsAscendingSotring As Boolean
    Public DefaultSortingFieldAlias As String

    Public Applications As List(Of Application)
    Public Tools As List(Of Tool)
    Public Actions As List(Of ActionDesignation)
    Public Links As List(Of Link)

End Class
