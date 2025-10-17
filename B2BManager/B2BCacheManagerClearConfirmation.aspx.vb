
Imports System.Data

Partial Class B2BCacheManagerClearConfirmation
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Clear cache value(s) confirmation"
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If
            Dim RequestID As String = Request.QueryString("RequestID")
            Dim requestDt As DataTable = ClsDataAccessHelper.FillDataTable("[CacheManager].GetCacheActionsByRequestID", New List(Of SqlClient.SqlParameter)((New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@RequestID", RequestID)})))
            Dim index As Integer = 0
            For Each row As DataRow In requestDt.Rows
                index += 1
                infoDiv.InnerHtml += "<span data-request-id='" + Request.QueryString("RequestID") + "' id='clear-" + index.ToString() + "-" + row("InstanceID").ToString() + "' data-key-name='" + ClsDataAccessHelper.GetText(row, "KeyName") + "' data-instance-path='" _
                + ClsDataAccessHelper.GetText(row, "InstancePathUrl") + "'data-key-env='" + EnvironmentID.ToString() + "'data-key-sop='" + ClsDataAccessHelper.GetText(row, "SOP_ID") + "'></span><i>" + ClsDataAccessHelper.GetText(row, "KeyName") + "</i> on <b>" + ClsDataAccessHelper.GetText(row, "ServerName") _
                + "</b> instance linked to <img src=""Images/FlagsSop/" + ClsDataAccessHelper.GetText(row, "SOP_ID") + ".png"" width=""20"" height=""16"" style=""border-radius:1px""/> " _
                + ClsDataAccessHelper.GetText(row, "Name") + " (<a>" + ClsDataAccessHelper.GetText(row, "InstancePathUrl") + "</a>)</br>"
            Next
        End If
    End Sub

End Class
