<ServiceContract()>
Public Interface IInsightsDataService

    <OperationContract()>
    Function GetData(ByVal uid As String, ByVal query As String) As DataTable

    <OperationContract()>
    Function CheckItemInCache(ByVal uid As String) As Boolean

End Interface
