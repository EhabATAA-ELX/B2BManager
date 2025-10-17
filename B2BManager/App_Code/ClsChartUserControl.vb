Imports Microsoft.VisualBasic

Public Class ClsChartUserControl
    Inherits System.Web.UI.UserControl
    Implements ClsChartInterface

    Private _Name As String
    Private _EnvironmentID As Integer
    Private _ChartID As Integer
    Private _SOPIDs As String
    Private _SubTitle As String
    Private _uid As String
    Private _ChartUID As Guid
    Private _Type As ClsInsightsHelper.InsightChartType
    Public Property Name As String Implements ClsChartInterface.Name
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Public Property SubTitle As String Implements ClsChartInterface.SubTitle
        Get
            Return _SubTitle
        End Get
        Set(value As String)
            _SubTitle = value
        End Set
    End Property

    Public Property EnvironmentID As Integer Implements ClsChartInterface.EnvironmentID
        Get
            Return _EnvironmentID
        End Get
        Set(value As Integer)
            _EnvironmentID = value
        End Set
    End Property

    Public Property SOPIDs As String Implements ClsChartInterface.SOPIDs
        Get
            Return _SOPIDs
        End Get
        Set(value As String)
            _SOPIDs = value
        End Set
    End Property

    Public Property ChartID As Integer Implements ClsChartInterface.ChartID
        Get
            Return _ChartID
        End Get
        Set(value As Integer)
            _ChartID = value
        End Set
    End Property

    Public Property UID As String Implements ClsChartInterface.UID
        Get
            Return _uid
        End Get
        Set(value As String)
            _uid = value
        End Set
    End Property


    Public Property ChartUID As Guid
        Get
            Return _ChartUID
        End Get
        Set(value As Guid)
            _ChartUID = value
        End Set
    End Property

    Public Property Type As ClsInsightsHelper.InsightChartType Implements ClsChartInterface.Type
        Get
            Return _Type
        End Get
        Set(value As ClsInsightsHelper.InsightChartType)
            _Type = value
        End Set
    End Property
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Init" & ChartUID.ToString(), GetScript(), True)
    End Sub

    Public Function GetScript() As String
        Dim script As String
        script = "var chart = { " &
        " ChartID:'" & ChartUID.ToString() & "'," &
        " ChartIntID:'" & ChartID.ToString() & "'," &
        " Title: '" & Name.Replace("'", "\\'") & "'," &
        " SubTitle: '" & SubTitle.Replace("'", "\\'") & "'," &
        " Loaded: false, " &
           " Trials: 0, " &
           " type:'" & Type.ToString() & "' " &
        " }; RenderChart(chart, '" & UID & "');"
        Return script
    End Function
End Class
