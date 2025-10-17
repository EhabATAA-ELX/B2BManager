Imports System.ServiceModel

' NOTE: You can use the "Rename" command on the context menu to change the interface name "ILogger" in both code and config file together.
<ServiceContract()>
Public Interface ILogger

    <OperationContract()>
    Sub Log(ApplicationName As String, _
            Environment As String, _
            ActionName As String, _
            MachineName As String, _
            UserID As String, _
            CountryID As String, _
            MachineDetails As String, _
            ActionDetails As String, _
            ElapsedTime As Integer, _
            MSG_XML As String, _
            HasError As Boolean, _
            ErrorMessage As String, _
            ErrorStackTrace As String
            )

End Interface
