<%@ WebHandler Language="VB" Class="GetCustomerLogo" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Data

Public Class GetCustomerLogo : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim logoID As Guid
        Dim customerLogo As ClsEbusinessHelper.CustomerLogo = Nothing
        If Not String.IsNullOrEmpty(context.Request.QueryString("logoID")) Then
            Guid.TryParse(context.Request.QueryString("logoID"), logoID)
        End If

        If (logoID <> Guid.Empty) Then
            If context.Cache("CustomerLogo_" + logoID.ToString()) IsNot Nothing Then
                customerLogo = context.Cache("CustomerLogo_" + logoID.ToString())
            Else
                Dim fileDataTable As DataTable = Nothing
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@logoID", logoID))
                fileDataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetCustomerPicture]", parameters)
                If fileDataTable IsNot Nothing Then
                    If fileDataTable.Rows.Count = 1 Then
                        Dim logoRow As DataRow = fileDataTable.Rows(0)
                        customerLogo = New ClsEbusinessHelper.CustomerLogo(ClsDataAccessHelper.GetText(logoRow, "LogoName"),
                                                                           ClsDataAccessHelper.GetText(logoRow, "ContentType"),
                                                                           logoRow("FileLength"),
                                                                           logoRow("Data"))
                        context.Cache.Insert("CustomerLogo_" + logoID.ToString(), customerLogo)
                    End If
                End If
            End If
        End If
        If customerLogo IsNot Nothing Then
            context.Response.Headers.Add("Cotent-Disposition", "attachment; filename=" + customerLogo.FileName)
            context.Response.ContentType = customerLogo.ContentType
            context.Response.OutputStream.Write(customerLogo.Data, 0, customerLogo.FileLength)
        Else
            context.Response.Headers.Add("Cotent-Disposition", "attachment; filename=DefaultImage.png")
            Dim filePath As String = HttpContext.Current.Server.MapPath("~/Images/Ebusiness/CustomersManagement/company.png")
            context.Response.TransmitFile(filePath)
        End If

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class