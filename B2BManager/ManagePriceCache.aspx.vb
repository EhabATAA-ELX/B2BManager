Imports System.Web.Services
Imports System.Text
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization
Imports System.Net
Imports System
Imports System.IO

Partial Class ManagePriceCache
    Inherits System.Web.UI.Page

    ' Base URL for the Price Management API
    Private Shared ReadOnly ApiBaseUrl As String = "https://b2btest.electrolux.net/APIPriceManagement/"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Pre-populate the customer number fields from the query string
            Dim customerNumber As String = Request.QueryString("CustomerNumber")
            If Not String.IsNullOrEmpty(customerNumber) Then
                txtGetPrice_CustomerNumber.Text = customerNumber
                txtRenew_CustomerNumber.Text = customerNumber
                txtRenewRange_CustomerNumber.Text = customerNumber
                txtDelete_CustomerNumber.Text = customerNumber
            End If
        End If
    End Sub

    ' Async helper using HttpWebRequest (compatible with .NET 4.5.1)
    Private Shared Async Function CallApiAsync(ByVal apiKey As String, ByVal endpoint As String, ByVal jsonBody As String) As Task(Of String)
        Try
            Dim url As String = ApiBaseUrl.TrimEnd("/"c) & "/" & endpoint.TrimStart("/"c)
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Headers.Add("x-api-key", apiKey)

            Dim bytes = Encoding.UTF8.GetBytes(jsonBody)
            Using reqStream = Await request.GetRequestStreamAsync()
                Await reqStream.WriteAsync(bytes, 0, bytes.Length)
            End Using

            Using resp = CType(Await request.GetResponseAsync(), HttpWebResponse)
                Using sr As New StreamReader(resp.GetResponseStream())
                    Dim respText = Await sr.ReadToEndAsync()
                    If resp.StatusCode >= HttpStatusCode.OK AndAlso resp.StatusCode < HttpStatusCode.Ambiguous Then
                        Return respText
                    Else
                        Dim ser As New JavaScriptSerializer()
                        Return ser.Serialize(New With {Key .error = "API call failed", Key .status = CType(resp.StatusCode, Integer), Key .body = respText})
                    End If
                End Using
            End Using
        Catch ex As WebException
            Dim responseBody As String = String.Empty
            Try
                If ex.Response IsNot Nothing Then
                    Using sr As New StreamReader(ex.Response.GetResponseStream())
                        responseBody = sr.ReadToEnd()
                    End Using
                End If
            Catch
            End Try
            Return (New JavaScriptSerializer()).Serialize(New With {Key .error = ex.Message, Key .ResponseBody = responseBody})
        Catch ex As Exception
            Return (New JavaScriptSerializer()).Serialize(New With {Key .error = ex.ToString()})
        End Try
    End Function

    ' Synchronous wrapper for compatibility (internal only)
    Private Shared Function CallApiSync(ByVal apiKey As String, ByVal endpoint As String, ByVal jsonBody As String) As String
        Return CallApiAsync(apiKey, endpoint, jsonBody).Result
    End Function

    ' --- WebMethods for long-running operations using persistent jobs ---
    <System.Web.Services.WebMethod()>
    Public Shared Function StartLongRunningCall(apiKey As String, endpoint As String, jsonBody As String) As String
        Dim jobId = LongRunningJobs.CreateAndRunAsync(Function()
                                                          Return CallApiAsync(apiKey, endpoint, jsonBody)
                                                      End Function)
        Return jobId.ToString()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetJobStatus(jobId As String) As String
        Dim serializer As New JavaScriptSerializer()
        Try
            Dim id = Guid.Parse(jobId)
            Dim info = LongRunningJobs.GetJob(id)
            If info Is Nothing Then
                Return serializer.Serialize(New With {Key .error = "Job not found"})
            End If
            Return serializer.Serialize(New With {Key .id = info.Id, Key .status = info.Status.ToString(), Key .result = info.Result, Key .error = info.ErrorMessage, Key .startedAt = info.StartedAt, Key .completedAt = info.CompletedAt})
        Catch ex As Exception
            Return serializer.Serialize(New With {Key .error = ex.Message})
        End Try
    End Function

End Class