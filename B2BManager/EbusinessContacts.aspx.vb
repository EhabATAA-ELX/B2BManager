
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class EbusinessContacts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Then
            BindData()
        Else
            If Not String.IsNullOrEmpty(target) Then
                If target.Contains("tvContacts") Then
                    Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
                    DisplayCard(__EVENTARGUMENT)
                End If
            End If
        End If
    End Sub

    Private Const PREVIEW_ACTION_STRING As String = "<span class='action-button' title='Preview Contact Card' onclick='PreviewContactCard(""{0}"")'>Preview</span>"

    Private Sub BindData()
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If

        If EnvironmentID > 0 AndAlso (cid <> Guid.Empty Or Uid <> Guid.Empty) Then
            BindContactsTree(EnvironmentID, cid, Uid)
        End If
    End Sub
    Private Sub BindContactsTree(environmentID As Integer, cid As Guid, uid As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        If cid = Guid.Empty Then
            parameters.Add(New SqlParameter("@UID", uid))
        Else
            parameters.Add(New SqlParameter("@cid", cid))
        End If
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetContactsByID]", parameters)
        Dim hasAtLeastOneContactToManage As Boolean = False
        Dim hasContacts As Boolean = False
        tvContacts.Nodes.Clear()
        If dataTable IsNot Nothing Then
            hasContacts = dataTable.Rows.Count > 0
            For Each row As DataRow In dataTable.Rows
                Dim contactNode As RadTreeNode = New RadTreeNode(ClsDataAccessHelper.GetText(row, "Name") + " " + String.Format(PREVIEW_ACTION_STRING, row("GlobalID")), row("GlobalID").ToString())
                contactNode.ImageUrl = "Images/contact-book.png"
                contactNode.Checked = row("Checked")
                contactNode.Enabled = row("Enabled")
                If Not row("Enabled") Then
                    contactNode.ToolTip = "Contact is assigned at customer level"
                Else
                    hasAtLeastOneContactToManage = True
                End If
                tvContacts.Nodes.Add(contactNode)
            Next
        End If

        If Not hasContacts Then
            lblNothingToChange.Text = "No contacts can be found"
            lblNothingToChange.Visible = True
            actionButtonsPlaceHolder.Visible = False
        Else
            If Not hasAtLeastOneContactToManage Then
                lblNothingToChange.Text = "All contacts are assigned at customer level"
                lblNothingToChange.Visible = True
                actionButtonsPlaceHolder.Visible = False
            End If
        End If
    End Sub

    Protected Sub DisplayCard(contactId As String)
        Dim EnvironmentID As Integer = 0
        Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@ContactID", Guid.Parse(contactId)))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[CtcMgmt_GetContactByID]", parameters)
        ContactPreview.DataBindSource(dataTable)
        ContactPreview.Visible = True
    End Sub
    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        BindData()
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If cid = Guid.Empty Then
            parameters.Add(New SqlParameter("@UID", Uid))
        Else
            parameters.Add(New SqlParameter("@CID", cid))
        End If
        Dim checkedNodes As String = String.Join(",", tvContacts.CheckedNodes.Where(Function(f) f.Enabled).Select(Function(fc) fc.Value))
        If Not String.IsNullOrEmpty(checkedNodes) Then
            parameters.Add(New SqlParameter("@ContactIDs", checkedNodes))
        End If
        If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_SetContactsByID]", parameters) Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "ConfirmAssignment();", True)
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to update contacts, please try again later');", True)
        End If
    End Sub
End Class
