
Imports System.IO
Imports System.Net

Partial Class TPSendMessage
    Inherits System.Web.UI.Page

    Private Shared Function ProceedRequest(ByVal requestXml As String, ByVal url As String, ByVal hubspanRequest As Boolean, ByVal login As String, ByVal password As String) As String
        Try
            Dim request As WebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "text/xml"

            If (hubspanRequest) Then
                Dim encoded As String = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(login + ":" + password))
                request.Headers.Add("Authorization", "Basic " + encoded)
            End If

            Dim data As String = requestXml
            Dim dataB As Byte() = Encoding.UTF8.GetBytes(data)
            request.ContentLength = dataB.Length

            Dim stream As Stream = request.GetRequestStream()
            stream.Write(dataB, 0, dataB.Length)
            stream.Close()

            Dim response As WebResponse = request.GetResponse()
            stream = response.GetResponseStream()
            Dim streamReader As StreamReader = New StreamReader(stream)
            Dim resultat As String = streamReader.ReadToEnd()
            stream.Close()
            streamReader.Close()
            response.Close()
            Return ClsHelper.PrettyXml(resultat)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetResponse(request As String, url As String, hubspanRequest As Boolean, login As String, password As String) As String
        Return ProceedRequest(request, url, hubspanRequest, login, password)
    End Function

    Public Sub Radios_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If hubspan.Checked Then
            If staging.Checked Then
                url.Text = "https://integration.hubspan.net/mh/server/tp=ELUXTEST/ssid=B2040674AC100055044658804007829E"
                login.Text = "eluxtest"
                password.Text = "ferr15whee75"
            ElseIf prod.Checked Then
                url.Text = "https://connect.hubspan.net/mh/server/tp=ELUXTEST/ssid=7E1C6EBDAC100061032A56114B2BD9FC"
                login.Text = "eluxtest"
                password.Text = "ferr15whee75"
            End If
        ElseIf Bili.Checked Then
            If staging.Checked Then
                url.Text = "http://euws1462/TP2CompassV2/index.aspx"
                login.Text = ""
                password.Text = ""
            ElseIf prod.Checked Then
                url.Text = "http://euws0791/Bili/index.aspx"
                login.Text = ""
                password.Text = ""
            End If

        End If
        EnvironmentPanel.Update()
    End Sub
End Class
