Imports System.Net
Imports Microsoft.VisualBasic

Namespace WebCacheManager
    Public Partial Class WebCacheManagerExtended
        Inherits WebCacheManager

#Region "Authentication Attributes"
        Private Const NETWORK_USERNAME As String = "webcachemanageruser"
        Private Const NETWORK_PASSWORD As String = "V2S3ox8Rxr45Fn44k9ha/kPYOuaxD8YdxRHAZt$j1w2991l"
#End Region
        Protected Overloads Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest

            Dim request As HttpWebRequest
            request = DirectCast(MyBase.GetWebRequest(uri), HttpWebRequest)
            Dim networkUserName As String = NETWORK_USERNAME
            Dim networkPassword As String = NETWORK_PASSWORD

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("WebCacheManager_UserName")) Then
                networkUserName = ConfigurationManager.AppSettings("WebCacheManager_UserName")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("WebCacheManager_Password")) Then
                networkPassword = ConfigurationManager.AppSettings("WebCacheManager_Password")
            End If
            Dim NetCred As New NetworkCredential(networkUserName, networkPassword)
            Dim networkCredentials As NetworkCredential = NetCred.GetCredential(uri, "Basic")
            If networkCredentials IsNot Nothing Then
                Dim credentialBuffer As Byte() = New UTF8Encoding().GetBytes(networkCredentials.UserName + ":" + networkCredentials.Password)
                request.Headers("Authorization") = "Basic " + Convert.ToBase64String(credentialBuffer)
            Else
                Throw New ApplicationException("No network credentials")
            End If

            Return request
        End Function


    End Class

End Namespace
