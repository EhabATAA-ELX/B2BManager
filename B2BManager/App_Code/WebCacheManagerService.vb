Imports System.Data
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Script.Services
Imports Newtonsoft.Json

<ServiceContract(Namespace:="")>
<ServiceBehaviorAttribute(IncludeExceptionDetailInFaults:=True)>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class WebCacheManagerService

    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml"
    <OperationContract()>
    <WebGet()>
    Public Function GetKeysByInstanceUrl(url As String, id As String) As Instance
        Dim result As Instance = New Instance(New List(Of String)(), id)
        Try
            Dim webservice As WebCacheManager.WebCacheManagerExtended = New WebCacheManager.WebCacheManagerExtended()
            webservice.Url = url + "/WebCacheManager.asmx"
            Dim keys As String() = webservice.GetCacheKeyNames()
            Array.Sort(keys)
            For Each key As String In keys
                result.keys.Add(key)
            Next
        Catch ex As Exception
            result.HasError = True
            result.ErrorMessage = ex.Message
        End Try
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function ClearPriceCache(keyName As String, id As String, Sop As String, Env As String) As Instance
        Dim result As Instance = New Instance(New List(Of String)(), id)
        Try
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_Remove_PriceCache]",
                             New List(Of SqlClient.SqlParameter)(New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@SOPNAME", Sop),
                                                                                               New SqlClient.SqlParameter("@EnvironmentID", Env)})) Then
                result.HasError = False
            Else
                result.HasError = True
            End If


            result.keys.Add(keyName)
        Catch ex As Exception
            result.HasError = True
            result.ErrorMessage = ex.Message
        End Try
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function ClearKey(url As String, keyName As String, id As String, requestId As String) As Instance
        Dim result As Instance = New Instance(New List(Of String)(), id)
        Try
            Dim webservice As WebCacheManager.WebCacheManagerExtended = New WebCacheManager.WebCacheManagerExtended()
            webservice.Url = url + "/WebCacheManager.asmx"
            result.HasError = Not webservice.RemoveCacheValueByKeyName(keyName)
            result.keys.Add(keyName)
        Catch ex As Exception
            result.HasError = True
            result.ErrorMessage = ex.Message
        End Try
        Return result
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GenerateConfirmationRequestKey(actions As List(Of ActionInfo)) As String
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim guid As Guid = ClsDataAccessHelper.ExecuteScalar("[CacheManager].[GenerateConfirmationActions]",
                           New List(Of SqlClient.SqlParameter)(New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@ActionsString", JsonConvert.SerializeObject(actions, Formatting.Indented)),
                                                                                             New SqlClient.SqlParameter("@UserID", user.GlobalID)}))
        Return guid.ToString()
    End Function

    <Serializable>
    Public Class Instance
        Private _id As String
        Private _keys As List(Of String)
        Private _HasError As Boolean
        Private _ErrorMessage As String

        Public Sub New(keys As List(Of String), id As String)
            Me.keys = keys
            Me.id = id
            Me._HasError = False
        End Sub

        Public Property ErrorMessage As String
            Get
                Return _ErrorMessage
            End Get
            Set
                _ErrorMessage = Value
            End Set
        End Property

        Public Property HasError As Boolean
            Get
                Return _HasError
            End Get
            Set
                _HasError = Value
            End Set
        End Property

        Public Property keys As List(Of String)
            Get
                Return _keys
            End Get
            Set
                _keys = Value
            End Set
        End Property

        Public Property id As String
            Get
                Return _id
            End Get
            Set
                _id = Value
            End Set
        End Property
    End Class

    <Serializable>
    <DataContract>
    Public Class ActionInfo
        Private _id As String
        Private _keyname As String
        Private _action As String

        <DataMember>
        Public Property id As String
            Get
                Return _id
            End Get
            Set
                _id = Value
            End Set
        End Property

        <DataMember>
        Public Property keyname As String
            Get
                Return _keyname
            End Get
            Set
                _keyname = Value
            End Set
        End Property

        <DataMember>
        Public Property action As String
            Get
                Return _action
            End Get
            Set
                _action = Value
            End Set
        End Property

        Public Sub New()
        End Sub
    End Class

End Class
