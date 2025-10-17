Imports System.Data
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports InsightsDataService

<ServiceContract(Namespace:="")>
<ServiceBehaviorAttribute(IncludeExceptionDetailInFaults:=True)>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class MasterData

    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml"
    <OperationContract()>
    <WebGet()>
    Public Function GetLineChartData(UID As String) As List(Of ClsInsightLineChartData)
        Dim data As List(Of ClsInsightLineChartData) = Nothing
        If Not String.IsNullOrEmpty(UID) Then
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If client.CheckItemInCache(UID) Then
                    Dim objectData As DataTable = New DataTable()
                    Try
                        objectData = client.GetData(UID, "")
                    Catch ex As Exception
                        Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                        If Not ex.Message Is Nothing Then
                            exceptionMessage = ex.Message
                        End If
                        If Not ex.StackTrace Is Nothing Then
                            exceptionStackTrace = ex.StackTrace
                        End If
                        Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetLineChartData</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                        ClsSendEmailHelper.SendErrorEmail(errorMsg)
                    End Try
                    data = ClsInsightsHelper.ConvertDataTableToClsInsightChartDataList(objectData)
                End If
            End Using
        End If
        Return data
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function GetPieChartData(UID As String) As List(Of ClsInsightPieChartData)
        Dim data As List(Of ClsInsightPieChartData) = Nothing
        If Not String.IsNullOrEmpty(UID) Then
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If client.CheckItemInCache(UID) Then
                    Dim objectData As DataTable = New DataTable()
                    Try
                        objectData = client.GetData(UID, "")
                    Catch ex As Exception
                        Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                        If Not ex.Message Is Nothing Then
                            exceptionMessage = ex.Message
                        End If
                        If Not ex.StackTrace Is Nothing Then
                            exceptionStackTrace = ex.StackTrace
                        End If
                        Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetPieChartData</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                        ClsSendEmailHelper.SendErrorEmail(errorMsg)
                    End Try
                    data = ClsInsightsHelper.ConvertDataTableToClsInsightPieChartData(objectData)
                End If
            End Using
        End If
        Return data
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function GetHorizontalBarChartData(UID As String) As List(Of ClsInsightBarStackChartData)
        Dim data As List(Of ClsInsightBarStackChartData) = Nothing
        If Not String.IsNullOrEmpty(UID) Then
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If client.CheckItemInCache(UID) Then
                    Dim objectData As DataTable = New DataTable()
                    Try
                        objectData = client.GetData(UID, "")
                    Catch ex As Exception
                        Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                        If Not ex.Message Is Nothing Then
                            exceptionMessage = ex.Message
                        End If
                        If Not ex.StackTrace Is Nothing Then
                            exceptionStackTrace = ex.StackTrace
                        End If
                        Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetHorizontalBarChartData</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                        ClsSendEmailHelper.SendErrorEmail(errorMsg)
                    End Try
                    data = ClsInsightsHelper.ConvertDataTableToClsInsightBarStackChartData(objectData)
                End If
            End Using
        End If
        Return data
    End Function

    <OperationContract()>
    <WebGet()>
    Public Function GetCustomHTML(UID As String, ChartID As Integer) As String
        Dim data As String = Nothing
        If Not String.IsNullOrEmpty(UID) Then
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If client.CheckItemInCache(UID) Then
                    Dim objectData As DataTable = New DataTable()
                    Try
                        objectData = client.GetData(UID, "")
                    Catch ex As Exception
                        Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                        If Not ex.Message Is Nothing Then
                            exceptionMessage = ex.Message
                        End If
                        If Not ex.StackTrace Is Nothing Then
                            exceptionStackTrace = ex.StackTrace
                        End If
                        Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetCustomHTML</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                        ClsSendEmailHelper.SendErrorEmail(errorMsg)
                    End Try
                    data = ClsInsightsHelper.ConvertDataTableToHTMLString(objectData, ChartID)
                End If
            End Using
        End If
        Return data
    End Function


    <OperationContract()>
    <WebGet()>
    Public Function SaveDashboardDesign(dashboardDesign As ClsDashboardDesignInfo) As Boolean
        Dim saveResults As Boolean = False
        If dashboardDesign IsNot Nothing Then
            If dashboardDesign.Charts IsNot Nothing Then
                If dashboardDesign.Charts.Count > 0 Then
                    saveResults = ClsInsightsHelper.ImportDataTableWithBulkAndApplyDesign(ClsDataAccessHelper.ConvertToDataTable(dashboardDesign.Charts), dashboardDesign.DashboardID)
                End If
            End If
        End If
        Return saveResults
    End Function

    ' Add more operations here and mark them with <OperationContract()>

End Class
