
Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Web

Partial Class UserControls_WebCacheManagerControl
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

        Dim URL As String = "http://localhost:65003/WebCacheManager.asmx"
        Dim webservice As WebCacheManager.WebCacheManagerExtended = New WebCacheManager.WebCacheManagerExtended()
        webservice.Url = URL
        Dim keys As String() = DirectCast(webservice.GetCacheKeyNames(), String())
        For Each key As String In keys
            keysDiv.InnerHtml += key + "</br>"
        Next

    End Sub

End Class
