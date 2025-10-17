
Imports System.Diagnostics

Partial Class _Default
    Inherits System.Web.UI.Page

    Private stopWatch As Stopwatch
    Private elapsedMillisecondsAtStart As Long
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        stopWatch = Stopwatch.StartNew()
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("Page_PreInit", ConfigurationManager.AppSettings("Environment"), "PreInit, elapsed: " & stopWatch.ElapsedMilliseconds.ToString() & " ms", stopWatch.ElapsedMilliseconds)
            LblName.Text = LblName.Text & "<br/>*******************************************<br/>" & "PreInit, elapsed: " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("Page_Init", ConfigurationManager.AppSettings("Environment"), "Init, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "Init, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub

    Protected Sub Page_InitComplete(ByVal sender As Object, ByVal e As EventArgs) Handles Me.InitComplete
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("Page_InitComplete", ConfigurationManager.AppSettings("Environment"), "InitComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "InitComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub

    Protected Overrides Sub OnPreLoad(ByVal e As EventArgs)
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("PreLoad", ConfigurationManager.AppSettings("Environment"), "PreLoad, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>*******************************************<br/>" & "PreLoad, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        Dim randomNumber As Integer = ClsHelper.GetRandomNumber()
        Threading.Thread.Sleep(randomNumber)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("Load", ConfigurationManager.AppSettings("Environment"), "Load, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "Load, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added  " & randomNumber.ToString() & " ms sleep from page)"
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        Dim randomNumber As Integer = ClsHelper.GetRandomNumber()
        Threading.Thread.Sleep(randomNumber)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("btnSubmit_Click", ConfigurationManager.AppSettings("Environment"), "btnSubmit_Click, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "btnSubmit_Click, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)"
        End If
    End Sub

    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As EventArgs) Handles Me.LoadComplete
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        Dim randomNumber As Integer = ClsHelper.GetRandomNumber()
        Threading.Thread.Sleep(randomNumber)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("LoadComplete", ConfigurationManager.AppSettings("Environment"), "LoadComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "LoadComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)"
        End If
    End Sub

    Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            LblName.Text = LblName.Text & "<br/>" & "PreRender, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub

    Protected Overrides Sub OnSaveStateComplete(ByVal e As EventArgs)
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim randomNumber As Integer = ClsHelper.GetRandomNumber()
        Threading.Thread.Sleep(randomNumber)
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("SaveStateComplete", ConfigurationManager.AppSettings("Environment"), "SaveStateComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from page)", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "SaveStateComplete, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms (Added " & randomNumber.ToString() & " ms sleep from SaveStateComplete)"
        End If
    End Sub

    Protected Sub Page_UnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Unload
        elapsedMillisecondsAtStart = stopWatch.ElapsedMilliseconds()
        Dim LblName As Label = CType(Master.FindControl("lblName"), Label)
        If LblName IsNot Nothing Then
            ClsLogger.LogWithEnvironment("UnLoad", ConfigurationManager.AppSettings("Environment"), "UnLoad, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms", (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart))
            LblName.Text = LblName.Text & "<br/>" & "UnLoad, elapsed: " & (stopWatch.ElapsedMilliseconds - elapsedMillisecondsAtStart).ToString() & " ms from " & stopWatch.ElapsedMilliseconds.ToString() & " ms"
        End If
    End Sub
End Class
