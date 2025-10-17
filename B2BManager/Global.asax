<%@ Application Language="VB" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim Url As String = ""
        Dim CurrentError As Exception = Nothing
        Dim ErrorGuid As Guid = Guid.NewGuid
        Dim UserID As Guid = Guid.Empty
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim EnvironmentName As String = "NOT APPLICABLE"
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Environment")) Then
            EnvironmentName = ConfigurationManager.AppSettings("Environment")
        End If
        Try
            Dim errorMessage As String = "Generic Error"
            Dim stackTrace As String = Nothing
            CurrentError = Server.GetLastError()

            If CurrentError IsNot Nothing Then
                errorMessage = CurrentError.Message
                If CurrentError.InnerException IsNot Nothing Then
                    If CurrentError.InnerException.Message IsNot Nothing Then
                        stackTrace = CurrentError.InnerException.Message
                    End If
                End If
            End If

            If user IsNot Nothing Then
                UserID = user.GlobalID
            End If

            Url = CType(CType(CType(sender, System.Web.HttpApplication).Request, System.Web.HttpRequest).Url, System.Uri).AbsoluteUri
            Dim logger As Logger = New Logger()
            logger.Log("Log Viewer", EnvironmentName, "Application Error", My.Computer.Name, UserID.ToString(), Guid.Empty.ToString(), ClsHelper.GetMachineInformation(), Nothing, 0, Nothing, True, errorMessage, stackTrace)
        Catch ex As Exception
        Finally
            Url = Nothing
            user = Nothing
            CurrentError = Nothing
            EnvironmentName = Nothing
        End Try
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub

</script>