<%@ WebHandler Language="VB" Class="GetCountryRangeFile" %>

Imports System
Imports System.Web
Imports System.IO

Public Class GetCountryRangeFile : Implements IHttpHandler, IRequiresSessionState

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/xml"
        Dim fileID As String = ""
        If context.Request.QueryString("fileID") IsNot Nothing Then
            fileID = context.Request.QueryString("fileID")
        End If
        context.Response.Clear()
        context.Response.Flush()
        Dim clsSelectedFile As ClsFile = ClsSessionHelper.countryRangeFiles.FirstOrDefault(Function(fc) (Not String.IsNullOrEmpty(fileID)) And fc.ID = fileID)
        If clsSelectedFile IsNot Nothing Then
            Dim fileInfo As FileInfo = clsSelectedFile.FileInfo
            context.Response.WriteFile(fileInfo.FullName)
        Else
            Dim content As String = "<?xml version=""1.0"" encoding=""UTF-8""?><INFORMATION_MESSAGE>No file can be loaded</INFORMATION_MESSAGE>"
            context.Response.Write(content)
        End If

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class