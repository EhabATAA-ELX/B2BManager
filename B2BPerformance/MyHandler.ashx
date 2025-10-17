<%@ WebHandler Language="VB" Class="MyHandler" %>
Imports System.Diagnostics
Imports System
Imports System.Web

Public Class MyHandler : Implements IHttpHandler, IRequiresSessionState

    Private stopWatch As Stopwatch
    Private elapsedMillisecondsAtStart As Long
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        stopWatch = Stopwatch.StartNew()
        context.Response.ContentType = "text/plain"
        If HttpContext.Current.Session("IsProductLoadedFromASHX") Is Nothing Then
            Threading.Thread.Sleep(5000)
            HttpContext.Current.Session("IsProductLoadedFromASHX") = True
            ClsLogger.LogWithEnvironment("Load Product (ASHX)", ConfigurationManager.AppSettings("Environment"), "Load Product, elapsed: " & stopWatch.ElapsedMilliseconds.ToString(), stopWatch.ElapsedMilliseconds)
        End If
        context.Response.Write("Load Product completed after " & (stopWatch.ElapsedMilliseconds).ToString() & " ms, Session variable name: IsProductLoadedFromASHX")
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class