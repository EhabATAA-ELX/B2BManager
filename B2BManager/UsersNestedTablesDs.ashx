<%@ WebHandler Language="VB" Class="UsersNestedTablesDs" %>

Imports System
Imports System.Web
Imports System.Data.SqlClient
Imports System.Data

Public Class UsersNestedTablesDs : Implements IHttpHandler, IRequiresSessionState

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest


        Dim response As String = Nothing
        Dim utype As Integer = 0

        If context.Request.QueryString("utype") IsNot Nothing Then
            Integer.TryParse(context.Request.QueryString("utype"), utype)
        End If

        If utype = 0 Then
            Dim EnvironmentID As Integer
            Dim IsSuperUserView As Integer = 0
            Dim CID As Guid = Guid.Empty
            Dim SopID As String = Nothing
            Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
            If context.Request.QueryString("EnvironmentID") IsNot Nothing Then
                Integer.TryParse(context.Request.QueryString("EnvironmentID"), EnvironmentID)
            End If
            If context.Request.QueryString("su") IsNot Nothing Then
                Integer.TryParse(context.Request.QueryString("su"), IsSuperUserView)
            End If
            If context.Request.QueryString("SopID") IsNot Nothing Then
                SopID = context.Request.QueryString("SopID")
            End If

            If context.Request.QueryString("CID") IsNot Nothing Then
                Guid.TryParse(context.Request.QueryString("CID"), CID)
            End If


            If (EnvironmentID > 0 And Not String.IsNullOrEmpty(SopID) AndAlso clsUser IsNot Nothing) Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
                parameters.Add(New SqlParameter("@SOPID", SopID))
                parameters.Add(New SqlParameter("@UserID", clsUser.GlobalID))
                parameters.Add(New SqlParameter("@IsSuperUserView", IsSuperUserView))
                If CID <> Guid.Empty Then
                    parameters.Add(New SqlParameter("@CID", CID))
                End If
                If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_USER) _
                   AndAlso Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER) Then
                    If clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CUSTOMER_DETAILS) Then
                        parameters.Add(New SqlParameter("@ActionsCustomerHeader", "Edit"))
                    Else
                        parameters.Add(New SqlParameter("@ActionsCustomerHeader", "Display"))
                    End If
                End If
                If IsSuperUserView = 0 Then
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_USER) Then
                        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_CUSTOMER_LIST_TAB_IN_SUPER_USER_PROFILE) Then
                            If clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_USER_SUPER_USER_DETAILS) Then
                                parameters.Add(New SqlParameter("@ActionsUserHeader", "Edit"))
                            Else
                                parameters.Add(New SqlParameter("@ActionsUserHeader", "Display"))
                            End If
                        End If
                    End If
                Else
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SUPER_USER) Then
                        If clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_USER_SUPER_USER_DETAILS) Then
                            parameters.Add(New SqlParameter("@ActionsUserHeader", "Edit"))
                        Else
                            parameters.Add(New SqlParameter("@ActionsUserHeader", "Display"))
                        End If
                    End If
                End If
                response = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_GetUsers]", parameters)
            End If
        Else
            Dim groupid As Integer = 0
            Dim showallselected As Boolean = False
            Dim selectedCountries As String = Nothing
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            If context.Request.QueryString("groupid") IsNot Nothing Then
                Integer.TryParse(context.Request.QueryString("groupid"), groupid)
                parameters.Add(New SqlParameter("@groupid", groupid))
            End If
            If context.Request.QueryString("showallselected") IsNot Nothing Then
                Boolean.TryParse(context.Request.QueryString("showallselected"), showallselected)
                parameters.Add(New SqlParameter("@showallselected", showallselected))
            End If
            If context.Request.QueryString("countries") IsNot Nothing Then
                selectedCountries = context.Request.QueryString("countries")
            End If
            parameters.Add(New SqlParameter("@selectedCountries", selectedCountries))
            Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("[Administration].[GetUsers]", parameters)
            response = "{ ""data"": " + ClsHelper.GetJson(dt) + "}"
        End If

        If String.IsNullOrEmpty(response) Then
            response = "[]"
        End If
        context.Response.ContentType = "application/json; charset=utf-8"
        context.Response.Write(response)
        context.Response.Headers.Add("Cotent-Disposition", "attachment; filename=nesteddatatablesource.json")
        response = Nothing
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class