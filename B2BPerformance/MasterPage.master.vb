Imports System.Diagnostics

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

    Private stopWatch As Stopwatch
    Private elapsedMillisecondsAtStart As Long
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        stopWatch = Stopwatch.StartNew()
        lblName.Text = lblName.Text & "<br/>" & "Master Init, elapsed:" & stopWatch.ElapsedMilliseconds.ToString() & " ms"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Threading.Thread.Sleep(250)
        ClsLogger.LogWithEnvironment("Master Load", ConfigurationManager.AppSettings("Environment"), "Master Load, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
        lblName.Text = lblName.Text & "<br/>" & "Master Load, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added 250 ms sleep from master page)"
    End Sub
    Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        ClsLogger.LogWithEnvironment("Master PreRender", ConfigurationManager.AppSettings("Environment"), "Master PreRender, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
        lblName.Text = lblName.Text & "<br/>" & "Master PreRender, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
    End Sub

    Protected Sub Page_UnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Unload
        ClsLogger.LogWithEnvironment("Master UnLoad", ConfigurationManager.AppSettings("Environment"), "Master UnLoad, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", stopWatch.ElapsedMilliseconds)
    End Sub

End Class

