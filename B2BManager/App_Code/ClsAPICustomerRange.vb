Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports B2BExtentions
Imports Microsoft.VisualBasic

Public Class ClsAPICustomerRange


    Public Shared Function GetCustomerRangeAPIUrl(EnvironmentID As String) As String

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))

        Dim returnValue As String = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_GetCustomerRangeApiUrl]", parameters).ToString()
        Return returnValue

    End Function



    Public Shared Function RequestRange(EnvironmentID As String,
                                           CustomerCode As String, SOPID As String,
                                           EmailLogin As String, Language As String
                                           ) As Boolean

        Dim serviceEndpoint As String = GetCustomerRangeAPIUrl(EnvironmentID)

        If String.IsNullOrEmpty(serviceEndpoint) Then
            Return False
        End If

        serviceEndpoint = serviceEndpoint & "?EmailLogin=" & EmailLogin & "&SopIndicator=" & SOPID & "&CustomerNumber=" & CustomerCode & "&Language=" & Language


        Dim req As HttpWebRequest
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls _
                                                   Or SecurityProtocolType.Tls11 _
                                                   Or SecurityProtocolType.Tls12 _
                                                   Or SecurityProtocolType.Ssl3

        req = HttpWebRequest.Create(serviceEndpoint)

        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

        Dim receivedContent As String
        Dim ResponseStream As Stream = response.GetResponseStream()
        Using streamReader = New StreamReader(ResponseStream)
            receivedContent = streamReader.ReadToEnd()
        End Using

        Dim statusCode As HttpStatusCode = response.StatusCode
        If (statusCode = HttpStatusCode.OK OrElse statusCode = HttpStatusCode.NotFound) Then
            Return True
        End If
        Return False

    End Function
End Class
