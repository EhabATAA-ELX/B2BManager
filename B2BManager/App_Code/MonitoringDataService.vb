Imports System.Data
Imports System.Data.SqlClient
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class MonitoringDataService

    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml"
    <OperationContract()>
    <WebGet()>
    Public Function GetData(ChartLineTime As Integer) As List(Of ClsChartData)
        Return GetDataFromDB(ChartLineTime)
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function ChangeActivationStatus(MessageID As String) As String
        Dim result As String = "error"
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@MessageID", New Guid(MessageID)))
        Dim DataTable As DataTable = ClsDataAccessHelper.FillDataTable("Monitoring.ChangeActivationStatus", parameters)
        If DataTable.Rows.Count = 1 Then
            result = DataTable.Rows(0)("Result")
        End If
        Return result
    End Function


    Private Function GetDataFromDB(ChartLineTime As Integer) As List(Of ClsChartData)
        Dim chartData As List(Of ClsChartData) = New List(Of ClsChartData)()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ChartLineTime", ChartLineTime))
        Dim DataTable As DataTable = ClsDataAccessHelper.FillDataTable("Monitoring.GetChartData", parameters)
        If DataTable.Rows.Count > 1 Then
            For Each row As DataRow In DataTable.Rows
                chartData.Add(New ClsChartData(row("Name").ToString(), row("SentOn"), row("ExecutionTime"), row("MessageID").ToString(), DirectCast(row("ExpectedResponseTimeInMilliseconds"), Integer), DirectCast(row("WorstAcceptableResponseTimeInMilliseconds"), Integer)))
            Next
        End If
        Return chartData
    End Function

    ' Add more operations here and mark them with <OperationContract()>

End Class
