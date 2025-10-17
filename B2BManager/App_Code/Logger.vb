Imports System.Data.SqlClient

' NOTE: You can use the "Rename" command on the context menu to change the class name "Logger" in code, svc and config file together.
Public Class Logger
    Implements ILogger


    Public Sub Log(ApplicationName As String, Environment As String, ActionName As String, MachineName As String, UserID As String, CountryID As String, MachineDetails As String, ActionDetails As String, ElapsedTime As Integer, MSG_XML As String, HasError As Boolean, ErrorMessage As String, ErrorStackTrace As String) Implements ILogger.Log
        Try
            Using Cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDB").ConnectionString)
                Cnx.Open()
                Dim Cmd As SqlCommand = New SqlCommand("[Logger].[LogActivity]", Cnx)
                Cmd.CommandType = Data.CommandType.StoredProcedure
                Cmd.CommandTimeout = 6000
                ClsDataAccessHelper.AddParameter(Cmd, "ApplicationName", ApplicationName)
                ClsDataAccessHelper.AddParameter(Cmd, "MachineName", MachineName)
                ClsDataAccessHelper.AddParameter(Cmd, "MachineDetails", MachineDetails)
                ClsDataAccessHelper.AddParameter(Cmd, "UserID", New Guid(UserID), Data.SqlDbType.UniqueIdentifier)
                ClsDataAccessHelper.AddParameter(Cmd, "CountryID", New Guid(CountryID), Data.SqlDbType.UniqueIdentifier)
                ClsDataAccessHelper.AddParameter(Cmd, "Environment", Environment)
                ClsDataAccessHelper.AddParameter(Cmd, "ActionName", ActionName)
                ClsDataAccessHelper.AddParameter(Cmd, "ActionDetails", ActionDetails)
                ClsDataAccessHelper.AddParameter(Cmd, "ElapsedTime", ElapsedTime, Data.SqlDbType.Int)
                ClsDataAccessHelper.AddParameter(Cmd, "MSG_XML", MSG_XML)
                ClsDataAccessHelper.AddParameter(Cmd, "HasError", HasError, Data.SqlDbType.Bit)
                ClsDataAccessHelper.AddParameter(Cmd, "ErrorMessage", ErrorMessage)
                ClsDataAccessHelper.AddParameter(Cmd, "ErrorStackTrace", ErrorStackTrace)
                Cmd.ExecuteNonQuery()
                Cnx.Close()
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Method name:</b>Log</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>Application Name: {2}</br>Environment: {3}</br>Action Name: {4}", exceptionMessage _
                        , exceptionStackTrace _
                        , ApplicationName _
                        , Environment _
                        , ActionName
                        )
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try

    End Sub
End Class
