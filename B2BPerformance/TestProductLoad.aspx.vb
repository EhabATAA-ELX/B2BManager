
Imports System.Diagnostics

Partial Class TestProductLoad
    Inherits System.Web.UI.Page

    Private stopWatch As Stopwatch
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            stopWatch = Stopwatch.StartNew()
            If Session("IsProductLoaded") Is Nothing Then
                Threading.Thread.Sleep(5000)
                Session("IsProductLoaded") = True
                ClsLogger.LogWithEnvironment("Load Product", ConfigurationManager.AppSettings("Environment"), "Load Product, elapsed: " & stopWatch.ElapsedMilliseconds.ToString(), stopWatch.ElapsedMilliseconds)
            End If
            lblInfo.Text = "Load Product completed after " & (stopWatch.ElapsedMilliseconds).ToString() & " ms, Session variable name: IsProductLoaded"
        End If
    End Sub

End Class
