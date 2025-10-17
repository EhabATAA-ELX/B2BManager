<%@ WebHandler Language="VB" Class="GetJSONFile" %>

Imports System
Imports System.Web
Imports System.Diagnostics

Public Class GetJSONFile : Implements IHttpHandler, IRequiresSessionState

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim watch As Stopwatch = Stopwatch.StartNew()
        context.Response.ContentType = "text/plain"
        Dim content As String = "No file can be loaded"
        Dim fileName As String = "Empty"
        If context.Request.QueryString("file") IsNot Nothing Then
            fileName = context.Request.QueryString("file")
            If Not String.IsNullOrEmpty(context.Session(fileName)) Then
                content = context.Session(fileName).ToString()
            End If
        End If
        Dim type As String = "View In Browser"
        If context.Request.QueryString("type") IsNot Nothing Then
            type = context.Request.QueryString("type")
            If type.ToLower().Equals("download") Then
                context.Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName & ".json")
            End If
        End If
        watch.Stop()
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            ClsHelper.Log(IIf(type.ToLower().Equals("download"), "Download JSON", "View JSON"), ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>FileName:</b> {0}</br><b>Type:</b> {1}", fileName, type), watch.ElapsedMilliseconds, fileName.Equals("Empty"), IIf(fileName.Equals("Empty"), "No file can be loaded", Nothing))
        End If
        context.Response.Write(content)
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class