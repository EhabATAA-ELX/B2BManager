' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.vb at the Solution Explorer and start debugging.
Imports System.Data.SqlClient
Imports System.Runtime.Caching
Imports InsightsDataService

Public Class InsightsDataService
    Implements IInsightsDataService

    Public Sub New()
    End Sub

    Public Function GetData(uid As String, query As String) As DataTable Implements IInsightsDataService.GetData
        Dim data As DataTable = New DataTable()
        data.TableName = "EmptyChartData"
        Try
            Dim cache As ObjectCache = MemoryCache.Default
            If cache.Contains(uid) Then
                data = DirectCast(cache.Get(uid), DataTable)
            Else
                If Not String.IsNullOrEmpty(query) Then
                    data = FillDataTable(query, Nothing, CommandType.Text)
                End If
                If data IsNot Nothing Then
                    Dim cacheItemPolicy As CacheItemPolicy = New CacheItemPolicy()
                    If uid.StartsWith("TEMP") Then
                        cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("TEMPCacheItemsAbsoluteExpirationValueInMinutes")))
                    Else
                        cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(CInt(ConfigurationManager.AppSettings("CacheItemsAbsoluteExpirationValueInHours")))
                    End If
                    cache.Add(uid, data, cacheItemPolicy)
                End If
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            SendErrorEmail(errorMsg)
        End Try

        Return data
    End Function
    Public Function CheckItemInCache(uid As String) As Boolean Implements IInsightsDataService.CheckItemInCache
        Dim cache As ObjectCache = MemoryCache.Default
        Return cache.Contains(uid)
    End Function

    Private Shared Function FillDataTable(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing, Optional ByVal commandType As CommandType = CommandType.StoredProcedure) As DataTable
        Dim dataTable As DataTable = New DataTable()
        Try
            Using cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                Dim cmd As New SqlCommand(storedProcedureName, cnx)
                cnx.Open()
                cmd.CommandTimeout = 600
                cmd.CommandType = commandType
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                adapter.Fill(dataTable)
                dataTable.TableName = "ChartData"
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            SendErrorEmail(errorMsg)
        End Try
        Return dataTable
    End Function

    Private Shared Sub SendErrorEmail(ByVal body As String)
        Try
            Dim oMail As Net.Mail.MailMessage
            oMail = New Net.Mail.MailMessage(ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString())
            oMail.BodyEncoding = Encoding.UTF8
            oMail.IsBodyHtml = True
            oMail.Body = body
            oMail.Subject = "Logger catched an error while logging"
            Dim objSmtp As Net.Mail.SmtpClient = New Net.Mail.SmtpClient(ConfigurationManager.AppSettings("SMTPServer").ToString())
            objSmtp.Send(oMail)
        Catch ex As Exception
            ' Error while sending email
        End Try
    End Sub

End Class
