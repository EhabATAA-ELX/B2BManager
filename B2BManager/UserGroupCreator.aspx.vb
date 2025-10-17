
Imports Telerik.Web.UI

Partial Class UserGroupCreator
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "New Group"
            CType(GroupInformation.FindControl("btnCancel"), HtmlInputButton).Value = "Cancel"
            CType(GroupInformation.FindControl("btnSubmit"), LinkButton).Text = "Submit"
            ClsUsersManagementHelper.FillToolsAndActionsTree(CType(GroupInformation.FindControl("treeToolsAndActions"), RadTreeView))
        End If
    End Sub

End Class
