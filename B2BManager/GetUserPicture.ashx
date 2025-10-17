<%@ WebHandler Language="VB" Class="GetUserPicture" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Data

Public Class GetUserPicture : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim uglobalid As Guid
        Dim userPicture As ClsEbusinessHelper.CustomerLogo = Nothing
        If Not String.IsNullOrEmpty(context.Request.QueryString("uid")) Then
            Guid.TryParse(context.Request.QueryString("uid"), uglobalid)
        End If

        If (uglobalid <> Guid.Empty) Then
            If context.Cache("UserPicture_" + uglobalid.ToString()) IsNot Nothing Then
                userPicture = context.Cache("UserPicture_" + uglobalid.ToString())
            Else
                Dim fileDataTable As DataTable = Nothing
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@U_GlobalID", uglobalid))
                fileDataTable = ClsDataAccessHelper.FillDataTable("[Administration].[GetUserPicture]", parameters)
                If fileDataTable IsNot Nothing Then
                    If fileDataTable.Rows.Count = 1 Then
                        Dim logoRow As DataRow = fileDataTable.Rows(0)
                        userPicture = New ClsEbusinessHelper.CustomerLogo(ClsDataAccessHelper.GetText(logoRow, "PictureName"),
                                                                           ClsDataAccessHelper.GetText(logoRow, "ContentType"),
                                                                           logoRow("FileLength"),
                                                                           logoRow("Data"))
                        context.Cache.Insert("UserPicture_" + uglobalid.ToString(), userPicture)
                    End If
                End If
            End If
        End If
        If userPicture IsNot Nothing Then
            context.Response.Headers.Add("Cotent-Disposition", "attachment; filename=" + userPicture.FileName)
            context.Response.ContentType = userPicture.ContentType
            context.Response.OutputStream.Write(userPicture.Data, 0, userPicture.FileLength)
        Else
            context.Response.Headers.Add("Cotent-Disposition", "attachment; filename=DefaultImage.png")
            Dim filePath As String = HttpContext.Current.Server.MapPath("~/Images/ContactImage.png")
            context.Response.TransmitFile(filePath)
        End If

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class