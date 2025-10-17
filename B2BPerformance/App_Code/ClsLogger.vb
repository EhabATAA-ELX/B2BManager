Imports System.Web
Imports System.Text
Imports System.Text.RegularExpressions

Public Class ClsLogger

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Log action
    ''' </summary>
    ''' <param name="ApplicationName">Define the application name</param>
    ''' <param name="Environment">The work environement (e.g. PROD, TEST)</param>
    ''' <param name="ActionName">The action name or designation - use friendly name</param>
    ''' <param name="ActionDetails" remark="Optional" >The action details</param>
    ''' <param name="UserID" remark="Optional" >The user identifier who originally invoked the action</param>
    ''' <param name="CountryID" remark="Optional" >The country identifier which is affected by the action</param>
    ''' <param name="MachineDetails" remark="Optional">The details of machine from where the action was invoked</param>
    ''' <param name="MachineName" remark="Optional">The machine name from where the action was invoked</param>
    ''' <param name="ElapsedTime" remark="Optional" >Can be used to log the elapsed time during the excution of an operation</param>
    ''' <param name="MSG_XML" remark="Optional" >Can be used to log an XML msg. Example: a request or a reply</param>
    ''' <param name="Exception" remark="Optional" >In case of error you can log your exception details by passing your exception object</param>
    ''' <history>
    ''' 	[KharrHam]	09/01/2018 Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Log(ApplicationName As String, _
                   Environment As String, _
                   ActionName As String, _
                   Optional MachineName As String = Nothing, _
                   Optional UserID As Guid = Nothing, _
                   Optional CountryID As Guid = Nothing, _
                   Optional MachineDetails As String = Nothing, _
                   Optional ActionDetails As String = Nothing, _
                   Optional ElapsedTime As Integer = 0, _
                   Optional MSG_XML As String = Nothing,
                   Optional Exception As Exception = Nothing)
        Dim HasError As Boolean = False
        Dim ErrorMessage As String = Nothing
        Dim ErrorStackTrace As String = Nothing
        If Not Exception Is Nothing Then
            HasError = True
            ErrorMessage = Exception.Message
            ErrorStackTrace = IIf(Exception.StackTrace Is Nothing, "", Exception.StackTrace)
        End If
        Try
            ' First trial to log asynchronously
            Using WebLogger As WebLogger.LoggerClient = New WebLogger.LoggerClient()
                ' Guid is passed as String in WCF needs to be converted to String and it should be converted back to Guid in the WCF service side.
                WebLogger.LogAsync(ApplicationName, Environment, ActionName, MachineName, UserID.ToString(), CountryID.ToString(), MachineDetails, ActionDetails, ElapsedTime, MSG_XML, HasError, ErrorMessage, ErrorStackTrace)
            End Using
        Catch ex As System.InvalidOperationException 'in case asynchronous call is not accepted 
            Try
                Using WebLogger As WebLogger.LoggerClient = New WebLogger.LoggerClient()
                    WebLogger.Log(ApplicationName, Environment, ActionName, MachineName, UserID.ToString(), CountryID.ToString(), MachineDetails, ActionDetails, ElapsedTime, MSG_XML, HasError, ErrorMessage, ErrorStackTrace)
                End Using
            Catch err As Exception
                'Nothing to do
            End Try
        Catch ex As Exception
            'Nothing to do
        End Try

    End Sub

    Public Shared Sub LogWithEnvironment(ActionName As String,
                   Environment As String,
                   Optional ActionDetails As String = Nothing,
                   Optional ElapsedTime As Integer = 0,
                   Optional ApplicationName As String = "B2B Performances")
        Try
            Using WebLogger As WebLogger.LoggerClient = New WebLogger.LoggerClient()
                Dim MachineName As String = My.Computer.Name
                Dim MachineDetails As String = GetMachineInformation()
                Dim UserID As Guid = Guid.Parse("8446d257-0b04-45e5-b1d7-9a83ebbe7490")
                Dim CountryID As Guid = Guid.Empty
                Log(ApplicationName, Environment, ActionName, MachineName, UserID, CountryID, MachineDetails, ActionDetails, ElapsedTime, Nothing, Nothing)
            End Using
        Catch ex As Exception
            'Nothing to do 
        End Try

    End Sub

    Public Shared Function GetMachineInformation() As String
        Return String.Format("Computer OSFullName: {0} OSPlatform: {1} OSVersion: {2} ",
                             IIf(Not My.Computer.Info.OSFullName Is Nothing, My.Computer.Info.OSFullName, ""),
                             IIf(Not My.Computer.Info.OSPlatform Is Nothing, My.Computer.Info.OSPlatform, ""),
                             IIf(Not My.Computer.Info.OSVersion Is Nothing, My.Computer.Info.OSVersion, ""))
    End Function

    Public Shared Function CleanInvalidXmlChars(ByVal text As String) As String
        Dim re As String = "[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]"
        Return Regex.Replace(text, re, "")
    End Function

End Class
