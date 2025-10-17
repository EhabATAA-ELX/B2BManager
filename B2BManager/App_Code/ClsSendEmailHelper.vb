Imports Microsoft.VisualBasic

Public Class ClsSendEmailHelper
    Public Shared Sub SendErrorEmail(ByVal body As String)
        Try
            Dim oMail As Net.Mail.MailMessage
            oMail = New Net.Mail.MailMessage(ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(), ConfigurationManager.AppSettings("ErrorEmailTo").ToString())
            oMail.BodyEncoding = Encoding.UTF8
            oMail.IsBodyHtml = True
            oMail.Body = body
            oMail.Subject = "B2B Manager catched an error"
            Dim objSmtp As Net.Mail.SmtpClient = New Net.Mail.SmtpClient(ConfigurationManager.AppSettings("SMTPServer").ToString())
            objSmtp.Send(oMail)
        Catch ex As Exception
            ' Error while sending email
        End Try
    End Sub
End Class
