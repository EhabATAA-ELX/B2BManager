
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class EbusinessAttachCustomerToSuperUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        CType(Master.FindControl("title"), HtmlTitle).Text = "Attach Customer to Super User"
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If

        If EnvironmentID > 0 AndAlso (cid <> Guid.Empty) Then
            BindSuperUsersTree(EnvironmentID, cid)
        End If
    End Sub

    Private Sub BindSuperUsersTree(environmentID As Integer, cid As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@cid", cid))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetSuperUsersAttachedToCustomer]", parameters)
        Dim hasAtLeastOneContactToManage As Boolean = False
        Dim hasContacts As Boolean = False
        tvSuperUsers.Nodes.Clear()
        Dim checkedCount As Integer = 0
        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim dataSearch As String = ClsDataAccessHelper.GetText(row, "varName").ToLower() + "#" + ClsDataAccessHelper.GetText(row, "varUserName").ToLower() + "#" + ClsDataAccessHelper.GetText(row, "varEmail").ToLower()
                Dim superUserNode As RadTreeNode = New RadTreeNode("<span data-search=""" + dataSearch + """ >" + ClsDataAccessHelper.GetText(row, "varName") + " (<b>" + ClsDataAccessHelper.GetText(row, "varUserName") + "</b>)</span>", row("U_GLOBALID").ToString())
                superUserNode.ImageUrl = "Images\Ebusiness\UserType\" + row("UserType").ToString() + ".png"
                superUserNode.Checked = row("Checked")
                tvSuperUsers.Nodes.Add(superUserNode)
                If row("Checked") Then
                    checkedCount += 1
                End If
            Next
        End If
        infoLabel.InnerHtml = dataTable.Rows.Count.ToString() + " super user" + IIf(dataTable.Rows.Count <= 1, "", "s")
        If checkedCount > 0 Then
            infoLabel.InnerHtml += " (<b>" + checkedCount.ToString() + " assigned</b>)"
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If
        Dim runStatus As String = ClsEbusinessHelper.MaintainSuperUserList(IIf(tvSuperUsers.CheckedNodes.Count > 0, 5, 6), String.Join(",", tvSuperUsers.CheckedNodes.ToList().Select(Function(fc) fc.Value)), EnvironmentID, Nothing, cid)

        If runStatus = "Success" Then
            infoLabel.InnerHtml = tvSuperUsers.Nodes.Count.ToString() + " super user" + IIf(tvSuperUsers.Nodes.Count.ToString() <= 1, "", "s")
            If tvSuperUsers.CheckedNodes.Count > 0 Then
                infoLabel.InnerHtml += " (<b>" + tvSuperUsers.CheckedNodes.Count.ToString() + " assigned</b>)"
            End If
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShowConfirmation", "ConfirmAssignment();", True)
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", String.Format("ErrorPopup('{0}');", runStatus), True)
        End If
    End Sub
End Class
