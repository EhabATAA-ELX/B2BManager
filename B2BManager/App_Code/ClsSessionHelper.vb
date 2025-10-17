Imports Microsoft.VisualBasic

Public Class ClsSessionHelper
    Public Shared Property LogonUser() As ClsUser
        Get
            Dim value As Object = HttpContext.Current.Session("LogonUser")

            If Not value Is Nothing And TypeOf (value) Is ClsUser Then
                Return CType(value, ClsUser)
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As ClsUser)
            HttpContext.Current.Session("LogonUser") = value
        End Set
    End Property

    Public Shared Property EbusinessEnvironmentID As Integer?
        Get
            Dim value As Object = HttpContext.Current.Session("EbusinessEnvironmentID")

            If Not value Is Nothing And TypeOf (value) Is Integer Then
                Return value
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As Integer?)
            HttpContext.Current.Session("EbusinessEnvironmentID") = value
        End Set
    End Property

    Public Shared Property EbusinessSopID As String
        Get
            Dim value As Object = HttpContext.Current.Session("EbusinessSopID")

            If Not value Is Nothing And TypeOf (value) Is String Then
                Return value
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session("EbusinessSopID") = value
        End Set
    End Property

    Public Shared Property TP2EnvironmentID As String
        Get
            Dim value As Object = HttpContext.Current.Session("TP2EnvironmentID")

            If Not value Is Nothing And TypeOf (value) Is String Then
                Return value
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session("TP2EnvironmentID") = value
        End Set
    End Property


    Public Shared Property TP2CountryCode As String
        Get
            Dim value As Object = HttpContext.Current.Session("TP2CountryCode")

            If Not value Is Nothing And TypeOf (value) Is String Then
                Return value
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session("TP2CountryCode") = value
        End Set
    End Property



    Public Shared Property countryRangeFiles As List(Of ClsFile)
        Get
            If HttpContext.Current.Session("countryRangeFiles") Is Nothing Then
                HttpContext.Current.Session("countryRangeFiles") = New List(Of ClsFile)
            End If
            Return CType(HttpContext.Current.Session("countryRangeFiles"), List(Of ClsFile))
        End Get
        Set(value As List(Of ClsFile))
            HttpContext.Current.Session("countryRangeFiles") = value
        End Set
    End Property

    Public Shared Property chatBots As List(Of ClsHelper.Chatbot)
        Get
            If HttpContext.Current.Session("chatBots") Is Nothing Then
                HttpContext.Current.Session("chatBots") = ClsHelper.GetChatbotsInformation(LogonUser.ID)
            End If
            Return CType(HttpContext.Current.Session("chatBots"), List(Of ClsHelper.Chatbot))
        End Get
        Set(value As List(Of ClsHelper.Chatbot))
            HttpContext.Current.Session("chatBots") = value
        End Set
    End Property

    Public Shared Property ActiveDashboard() As ClsInsightsHelper.Dashobard
        Get
            Dim value As Object = HttpContext.Current.Session("ActiveDashboard")

            If Not value Is Nothing And TypeOf (value) Is ClsInsightsHelper.Dashobard Then
                Return CType(value, ClsInsightsHelper.Dashobard)
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As ClsInsightsHelper.Dashobard)
            HttpContext.Current.Session("ActiveDashboard") = value
        End Set
    End Property


End Class
