
Partial Class UserControls_XMLPageView
    Inherits System.Web.UI.UserControl

    Private Property _html As String
    Private Property _sessionName As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(_html) And Not String.IsNullOrEmpty(_sessionName) Then
            divXMLReply.InnerHtml = _html
            BtnViewReplyInBrowser.Attributes.Add("onclick", "popup('GetXMLFile.ashx?file=" + _sessionName + "')")
            BtnDownloadReplyXML.Attributes.Add("onclick", "donwloadXML('" + _sessionName + "')")
            BtnCopyReply.Attributes.Add("data-clipboard-target", "#" & divXMLReply.UniqueID.Replace("$", "_").Replace("ctl00_", ""))
            BtnCopyReply.Visible = True
            BtnDownloadReplyXML.Visible = True
            BtnViewReplyInBrowser.Visible = True
        End If
    End Sub

    Public Sub SetParameters(ByVal html As String, ByVal sessionName As String)
        _html = html
        _sessionName = sessionName
    End Sub

End Class
