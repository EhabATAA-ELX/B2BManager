
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class B2BUsersWithSameEmail
    Inherits System.Web.UI.Page


    Dim EnvironmentID As String
    Dim Email As String
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "B2B Users With Same Notifications Email"
        End If
        EnvironmentID = Request.QueryString("EnvironmentID")
        Email = Request.QueryString("Email")
        EmailLbl.Text = Email
    End Sub

    Protected Sub gridResult_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)

        gridResult.DataSource = GetValue()
    End Sub

    Private Function GetValue() As DataTable
        Dim parametersEmail As List(Of SqlParameter) = New List(Of SqlParameter)()
        parametersEmail.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parametersEmail.Add(New SqlParameter("@Email", Email))
        Dim dataSetEMAIL As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_PS_GET USER_WithSameEmail]", parametersEmail)
        Return dataSetEMAIL.Tables(0)
    End Function
End Class
