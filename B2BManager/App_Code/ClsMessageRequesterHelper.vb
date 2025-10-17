Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class ClsMessageRequesterHelper

    <Serializable>
    Public Class MessageType

        Private _ID As Integer
        Private _doesRequireSOPID As Boolean
        Private _doesSupportMassGeneration As Boolean
        Private _hasHybrisInterface As Boolean
        Private _webserviceSpecificKeyName As String
        Private _methodSpecificKeyName As String
        Private _name As String
        Private _correspondentActionID As Integer
        Private _correspondentTableName As String
        Private _doesRequireValidation As Boolean

        Public Sub New()

        End Sub
        Public Sub New(iD As Integer, doesRequireSOPID As Boolean, doesSupportMassGeneration As Boolean, hasHybrisInterface As Boolean, webserviceSpecificKeyName As String, methodSpecificKeyName As String, name As String, correspondentActionID As Integer, correspondentTableName As String, doesRequireValidation As Boolean)
            Me.ID = iD
            Me.DoesRequireSOPID = doesRequireSOPID
            Me.DoesSupportMassGeneration = doesSupportMassGeneration
            Me.HasHybrisInterface = hasHybrisInterface
            Me.WebserviceSpecificKeyName = webserviceSpecificKeyName
            Me.MethodSpecificKeyName = methodSpecificKeyName
            Me.Name = name
            Me.CorrespondentActionID = correspondentActionID
            Me.CorrespondentTableName = correspondentTableName
            Me.DoesRequireValidation = doesRequireValidation
        End Sub

        Public Property ID As Integer
            Get
                Return _ID
            End Get
            Set(value As Integer)
                _ID = value
            End Set
        End Property

        Public Property DoesRequireSOPID As Boolean
            Get
                Return _doesRequireSOPID
            End Get
            Set(value As Boolean)
                _doesRequireSOPID = value
            End Set
        End Property

        Public Property DoesSupportMassGeneration As Boolean
            Get
                Return _doesSupportMassGeneration
            End Get
            Set(value As Boolean)
                _doesSupportMassGeneration = value
            End Set
        End Property

        Public Property HasHybrisInterface As Boolean
            Get
                Return _hasHybrisInterface
            End Get
            Set(value As Boolean)
                _hasHybrisInterface = value
            End Set
        End Property

        Public Property WebserviceSpecificKeyName As String
            Get
                Return _webserviceSpecificKeyName
            End Get
            Set(value As String)
                _webserviceSpecificKeyName = value
            End Set
        End Property

        Public Property MethodSpecificKeyName As String
            Get
                Return _methodSpecificKeyName
            End Get
            Set(value As String)
                _methodSpecificKeyName = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Public Property CorrespondentActionID As Integer
            Get
                Return _correspondentActionID
            End Get
            Set(value As Integer)
                _correspondentActionID = value
            End Set
        End Property

        Public Property CorrespondentTableName As String
            Get
                Return _correspondentTableName
            End Get
            Set(value As String)
                _correspondentTableName = value
            End Set
        End Property

        Public Property DoesRequireValidation As Boolean
            Get
                Return _doesRequireValidation
            End Get
            Set(value As Boolean)
                _doesRequireValidation = value
            End Set
        End Property
    End Class

    Public Shared Sub LogXMLReply(CorrelID As String, LinkedCorrelID As String, Reply_XML_MSG As String, Request_XML_MSG As String, EnvironmentID As Integer, MSG_TYPE As String, DATE_REQUESTED As DateTime, DATE_RECIEVED As DateTime?, WCF_URL As String, HasError As Boolean, ErrorMessage As String)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@Reply_XML_MSG", Reply_XML_MSG))
        parameters.Add(New SqlParameter("@Request_XML_MSG", Request_XML_MSG))
        parameters.Add(New SqlParameter("@CorrelID", CorrelID))
        parameters.Add(New SqlParameter("@LinkedCorrelID", LinkedCorrelID))
        parameters.Add(New SqlParameter("@RequesterUserID", ClsSessionHelper.LogonUser.GlobalID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@MSG_TYPE", MSG_TYPE))
        parameters.Add(New SqlParameter("@DATE_RECIEVED", DATE_RECIEVED))
        parameters.Add(New SqlParameter("@DATE_REQUESTED", DATE_REQUESTED))
        parameters.Add(New SqlParameter("@WCF_URL", WCF_URL))
        parameters.Add(New SqlParameter("@HasError", HasError))
        parameters.Add(New SqlParameter("ErrorMessage", ErrorMessage))
        ClsDataAccessHelper.ExecuteNonQuery("[Logger].[LogXMLReply]", parameters)
    End Sub

    <Serializable>
    Public Class MessageInformation
        Private _sAPWCFURL As String
        Private _sAPWCFMETHODNAME As String
        Private _sAPWCFMETHOD As String
        Private _sAPWCFUSERNAME As String
        Private _sAPWCFPSSWORD As String

        Public Sub New()

        End Sub


        Public Property SAPWCFURL As String
            Get
                Return _sAPWCFURL
            End Get
            Set(value As String)
                _sAPWCFURL = value
            End Set
        End Property

        Public Property SAPWCFMETHODNAME As String
            Get
                Return _sAPWCFMETHODNAME
            End Get
            Set(value As String)
                _sAPWCFMETHODNAME = value
            End Set
        End Property

        Public Property SAPWCFMETHOD As String
            Get
                Return _sAPWCFMETHOD
            End Get
            Set(value As String)
                _sAPWCFMETHOD = value
            End Set
        End Property

        Public Property SAPWCFUSERNAME As String
            Get
                Return _sAPWCFUSERNAME
            End Get
            Set(value As String)
                _sAPWCFUSERNAME = value
            End Set
        End Property

        Public Property SAPWCFPSSWORD As String
            Get
                Return _sAPWCFPSSWORD
            End Get
            Set(value As String)
                _sAPWCFPSSWORD = value
            End Set
        End Property
    End Class

    Public Shared Function GetMessageInformationByEnvironmentID(EnvironmentID As Integer) As List(Of MessageInformation)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        Dim messageInformationDT As DataTable = ClsDataAccessHelper.FillDataTable("[MESSAGEREQUESTER].[GetXMLParameters]", parameters)
        Return ClsDataAccessHelper.CreateListFromTable(Of MessageInformation)(messageInformationDT)
    End Function

    Public Shared Function GetMessageTypes() As List(Of MessageType)
        Dim messageTypeDT As DataTable = ClsDataAccessHelper.FillDataTable("[MessageRequester].[GetMessageTypes]")
        Return ClsDataAccessHelper.CreateListFromTable(Of MessageType)(messageTypeDT)
    End Function

    Public Shared Sub RenderDropDownList(ByVal DropDownList As DropDownList, ByVal messageTypes As List(Of MessageType))
        DropDownList.Items.Clear()
        If messageTypes IsNot Nothing Then
            For Each item As MessageType In messageTypes
                DropDownList.Items.Add(New ListItem(item.Name, item.ID.ToString()))
            Next
            If messageTypes.Count > 0 Then
                DropDownList.SelectedIndex = 0
            End If
        End If
    End Sub

End Class
