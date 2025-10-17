
Imports System.Diagnostics
Imports System.Threading

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub LoginButton_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsHelper.ValidateUser(UserName.Text, Password.Text)
        Dim strRedirect As String = "Home.aspx"
        Dim GlobalID As String = Guid.NewGuid().ToString()
        If user IsNot Nothing Then
            Dim Actions As List(Of ClsHelper.ActionDesignation) = Nothing
            Dim Tools As List(Of ClsHelper.Tool) = Nothing
            Dim Links As List(Of ClsHelper.Link) = Nothing
            user.Applications = ClsHelper.GetUserInformation(user.ID, Tools, Actions, Links)
            user.Actions = Actions
            user.Tools = Tools
            user.Links = Links
            Dim defaultTool As ClsHelper.Tool = ClsHelper.FindToolByID(Tools, user.HomePageToolID)
            If defaultTool IsNot Nothing Then
                strRedirect = defaultTool.Url
            End If
            If Tools IsNot Nothing Then
                ' Preload Insights Webservice if user has right to access it
                If Tools.Where(Function(fc) fc.ToolID = 15 And fc.TypeID = 1).Count() = 1 Then
                    Dim preLoadThread As Thread = New Thread(Function(t) ClsInsightsHelper.Preload())
                    preLoadThread.Start()
                End If
            End If
            ClsSessionHelper.LogonUser = user
            FormsAuthentication.RedirectFromLoginPage(UserName.Text, True)
            GlobalID = user.GlobalID.ToString()
            If Not String.IsNullOrEmpty(Request("ReturnURL")) Then
                strRedirect = Request("ReturnURL")
            End If
        End If
        InvalidCredentialsMessage.Visible = True
        watch.Stop()
        ClsHelper.Log("Sign in", GlobalID, String.Format("<b>Username:</b> {0}", UserName.Text), watch.ElapsedMilliseconds, user Is Nothing, IIf(user Is Nothing, "Unable to connect using password " + Password.Text, Nothing))
        If user IsNot Nothing Then
            Response.Redirect(strRedirect, True)
        End If
    End Sub

    Protected Function GetBackgroundImageUrl() As String
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("LoginImageUrl")) Then
            Return ConfigurationManager.AppSettings("LoginImageUrl")
        Else
            Return "Images/loginbg.png"
        End If
    End Function

    Protected Function GetOpacityAttribute() As String
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("LoginBackgroundOpacity")) Then
            Return ConfigurationManager.AppSettings("LoginBackgroundOpacity")
        Else
            Return "1"
        End If
    End Function
End Class
