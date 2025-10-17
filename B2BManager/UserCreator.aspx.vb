
Imports Telerik.Web.UI

Partial Class UserCreator
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "New User"
            CType(UserInformation.FindControl("btnCancel"), HtmlInputButton).Value = "Cancel"
            CType(UserInformation.FindControl("btnSubmit"), LinkButton).Text = "Submit"
            'TODO #userregistration
            '''CType(UserInformation.FindControl("changePassword"), HtmlGenericControl).Visible = False
            '''CType(UserInformation.FindControl("txtBoxPassword"), TextBox).Visible = True
            ClsUsersManagementHelper.FillUserCountriesTree(CType(UserInformation.FindControl("treeCountries"), RadTreeView), False)
        End If
    End Sub

End Class
