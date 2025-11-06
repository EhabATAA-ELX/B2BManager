Imports System
Imports System.Collections.Concurrent
Imports System.Threading.Tasks
Imports System.IO
Imports System.Web
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration
Imports System.Data.SqlClient

Public Module LongRunningJobs

    Public Enum JobStatus
        Pending
        Running
        Completed
        Failed
    End Enum

    Public Class JobInfo
        Public Property Id As Guid
        Public Property Status As JobStatus
        Public Property Result As String
        Public Property ErrorMessage As String
        Public Property StartedAt As DateTime?
        Public Property CompletedAt As DateTime?
    End Class

    Private ReadOnly _jobs As New ConcurrentDictionary(Of Guid, JobInfo)()
    Private ReadOnly _storagePath As String = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "jobs.json")
    Private ReadOnly _locker As New Object()
    Private ReadOnly _connectionString As String = If(ConfigurationManager.ConnectionStrings("LogDB") IsNot Nothing, ConfigurationManager.ConnectionStrings("LogDB").ConnectionString, Nothing)

    Sub New()
        ' load persisted jobs from DB if available; otherwise from file
        Try
            If Not String.IsNullOrEmpty(_connectionString) Then
                LoadFromDatabase()
            ElseIf File.Exists(_storagePath) Then
                Dim txt = File.ReadAllText(_storagePath, Encoding.UTF8)
                If Not String.IsNullOrWhiteSpace(txt) Then
                    Dim ser As New JavaScriptSerializer()
                    Dim list = ser.Deserialize(Of List(Of JobInfo))(txt)
                    If list IsNot Nothing Then
                        For Each j In list
                            _jobs(j.Id) = j
                        Next
                    End If
                End If
            End If
        Catch
        End Try
    End Sub

    Private Sub PersistToFile()
        Try
            SyncLock _locker
                Dim ser As New JavaScriptSerializer()
                Dim list = New List(Of JobInfo)(_jobs.Values)
                Dim txt = ser.Serialize(list)
                Directory.CreateDirectory(Path.GetDirectoryName(_storagePath))
                File.WriteAllText(_storagePath, txt, Encoding.UTF8)
            End SyncLock
        Catch
        End Try
    End Sub

    Private Function TableExists(cn As SqlConnection) As Boolean
        Try
            Dim cmd As New SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Jobs'", cn)
            Dim cnt = Convert.ToInt32(cmd.ExecuteScalar())
            Return cnt > 0
        Catch
            Return False
        End Try
    End Function

    Private Sub PersistToDatabase(job As JobInfo)
        Try
            If String.IsNullOrEmpty(_connectionString) Then Return
            Using cn As New SqlConnection(_connectionString)
                cn.Open()
                If Not TableExists(cn) Then
                    ' Do not create tables automatically; fall back to file persistence
                    PersistToFile()
                    Return
                End If
                Dim sql As String = String.Concat(
                    "IF EXISTS (SELECT 1 FROM [dbo].[Jobs] WHERE Id = @Id) ",
                    "UPDATE [dbo].[Jobs] SET [Status]=@Status, [Result]=@Result, [ErrorMessage]=@ErrorMessage, [StartedAt]=@StartedAt, [CompletedAt]=@CompletedAt WHERE Id=@Id ",
                    "ELSE ",
                    "INSERT INTO [dbo].[Jobs]([Id],[Status],[Result],[ErrorMessage],[StartedAt],[CompletedAt]) VALUES(@Id,@Status,@Result,@ErrorMessage,@StartedAt,@CompletedAt)"
                )
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@Id", job.Id)
                cmd.Parameters.AddWithValue("@Status", job.Status.ToString())
                cmd.Parameters.AddWithValue("@Result", If(job.Result, String.Empty))
                cmd.Parameters.AddWithValue("@ErrorMessage", If(job.ErrorMessage, String.Empty))
                cmd.Parameters.AddWithValue("@StartedAt", If(job.StartedAt.HasValue, CType(job.StartedAt.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@CompletedAt", If(job.CompletedAt.HasValue, CType(job.CompletedAt.Value, Object), DBNull.Value))
                cmd.ExecuteNonQuery()
            End Using
        Catch
        End Try
    End Sub

    Private Sub LoadFromDatabase()
        Try
            Using cn As New SqlConnection(_connectionString)
                cn.Open()
                If Not TableExists(cn) Then Return
                Dim cmd As New SqlCommand("SELECT [Id],[Status],[Result],[ErrorMessage],[StartedAt],[CompletedAt] FROM [dbo].[Jobs]", cn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Dim j As New JobInfo()
                        j.Id = r.GetGuid(0)
                        j.Status = [Enum].Parse(GetType(JobStatus), r.GetString(1))
                        j.Result = If(r.IsDBNull(2), String.Empty, r.GetString(2))
                        j.ErrorMessage = If(r.IsDBNull(3), String.Empty, r.GetString(3))
                        j.StartedAt = If(r.IsDBNull(4), Nothing, CType(r.GetDateTime(4), DateTime?))
                        j.CompletedAt = If(r.IsDBNull(5), Nothing, CType(r.GetDateTime(5), DateTime?))
                        _jobs(j.Id) = j
                    End While
                End Using
            End Using
        Catch
        End Try
    End Sub

    Private Sub Persist(job As JobInfo)
        ' Persist to DB if available and table exists, else to file
        Try
            If Not String.IsNullOrEmpty(_connectionString) Then
                Using cn As New SqlConnection(_connectionString)
                    cn.Open()
                    If TableExists(cn) Then
                        PersistToDatabase(job)
                        Return
                    End If
                End Using
            End If
        Catch
        End Try
        PersistToFile()
    End Sub

    Public Function CreateAndRun(jobFunc As Func(Of String)) As Guid
        Dim id = Guid.NewGuid()
        Dim info As New JobInfo With {
            .Id = id,
            .Status = JobStatus.Pending,
            .Result = String.Empty,
            .ErrorMessage = String.Empty,
            .StartedAt = DateTime.UtcNow
        }
        _jobs(id) = info
        Persist(info)

        Task.Run(Sub()
                     info.Status = JobStatus.Running
                     Persist(info)
                     Try
                         Dim res = jobFunc()
                         info.Result = res
                         info.Status = JobStatus.Completed
                         info.CompletedAt = DateTime.UtcNow
                     Catch ex As Exception
                         info.ErrorMessage = ex.ToString()
                         info.Status = JobStatus.Failed
                         info.CompletedAt = DateTime.UtcNow
                     Finally
                         Persist(info)
                     End Try
                 End Sub)

        Return id
    End Function

    Public Function CreateAndRunAsync(jobFuncAsync As Func(Of Task(Of String))) As Guid
        Dim id = Guid.NewGuid()
        Dim info As New JobInfo With {
            .Id = id,
            .Status = JobStatus.Pending,
            .Result = String.Empty,
            .ErrorMessage = String.Empty,
            .StartedAt = DateTime.UtcNow
        }
        _jobs(id) = info
        Persist(info)

        Task.Run(Async Function()
                     info.Status = JobStatus.Running
                     Persist(info)
                     Try
                         Dim res = Await jobFuncAsync()
                         info.Result = res
                         info.Status = JobStatus.Completed
                         info.CompletedAt = DateTime.UtcNow
                     Catch ex As Exception
                         info.ErrorMessage = ex.ToString()
                         info.Status = JobStatus.Failed
                         info.CompletedAt = DateTime.UtcNow
                     Finally
                         Persist(info)
                     End Try
                 End Function)

        Return id
    End Function

    Public Function GetJob(id As Guid) As JobInfo
        Dim info As JobInfo = Nothing
        If _jobs.TryGetValue(id, info) Then
            Return info
        End If
        Return Nothing
    End Function

    Public Function GetAllJobs() As List(Of JobInfo)
        Return New List(Of JobInfo)(_jobs.Values)
    End Function

    Public Function RemoveJob(id As Guid) As Boolean
        Dim removed As JobInfo = Nothing
        If _jobs.TryRemove(id, removed) Then
            ' Persist current state (file or DB)
            PersistToFile()
            Try
                If Not String.IsNullOrEmpty(_connectionString) Then
                    Using cn As New SqlConnection(_connectionString)
                        cn.Open()
                        If TableExists(cn) Then
                            Dim cmd As New SqlCommand("DELETE FROM [dbo].[Jobs] WHERE Id=@Id", cn)
                            cmd.Parameters.AddWithValue("@Id", id)
                            cmd.ExecuteNonQuery()
                        End If
                    End Using
                End If
            Catch
            End Try
            Return True
        End If
        Return False
    End Function

End Module
