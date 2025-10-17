
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class UserControls_EbusinessPreferences
    Inherits System.Web.UI.UserControl

    Private _ShowCancelButton As Boolean

    Public Property ShowCancelButton As Boolean
        Get
            Return _ShowCancelButton
        End Get
        Set
            _ShowCancelButton = Value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Then
            RenderControls(True)
        End If
    End Sub
    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, clsUser.DefaultEbusinessEnvironmentID)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, clsUser.DefaultEbusinessSopID, False, String.IsNullOrEmpty(clsUser.DefaultEbusinessSopID))
        chkBoxExpandOnSearch.Checked = clsUser.ExpandRowsOnSearchByDefault
        chkBoxDisplayMode.Checked = clsUser.ActivateWindowModeByDefault
        ddlManagementType.SelectedValue = clsUser.DefaultEbusinessManagementType
        RenderFields(ClsHelper.GetCustomerAndUserFields(clsUser.GlobalID))
        ddlSortDirection.SelectedValue = IIf(clsUser.IsAscendingSotring, 0, 1)
        btnCancelPanel.Visible = ShowCancelButton
    End Sub

    Private Sub RenderFields(Fields As List(Of ClsHelper.Field))
        tvFields.Nodes.Clear()
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim item As ListItem = New ListItem("Users Count", "Users Count")
        If clsUser.DefaultSortingFieldAlias.Equals("Users Count") Then
            item.Selected = True
        End If
        ddlCustomersDefaultSortingBy.Items.Add(item)

        If Fields.Count > 0 Then
            For Each field As ClsHelper.Field In Fields.Where(Function(Fc) Fc.ParentID Is Nothing)
                Dim parentNode As RadTreeNode = New RadTreeNode(field.Name, field.ID.ToString())
                parentNode.ImageUrl = "../" + field.imageUrl
                Dim children As List(Of ClsHelper.Field) = Fields.Where(Function(Fc) Fc.ParentID IsNot Nothing And Fc.ParentID = field.ID).ToList()
                If children.Count > 0 Then
                    For Each childField As ClsHelper.Field In children
                        Dim child As RadTreeNode = New RadTreeNode(childField.Name, childField.ID.ToString())
                        child.ImageUrl = "../" + childField.imageUrl
                        child.Checked = childField.Checked
                        child.Enabled = childField.Enabled
                        parentNode.Nodes.Add(child)
                        If field.ID = 1000 Then
                            item = New ListItem(childField.Name, childField.Name)
                            If clsUser.DefaultSortingFieldAlias = childField.Name Then
                                item.Selected = True
                            End If
                            ddlCustomersDefaultSortingBy.Items.Add(item)
                        End If
                    Next

                End If
                parentNode.Checkable = field.Enabled
                parentNode.Expanded = False
                If children.Count > 0 Then
                    tvFields.Nodes.Add(parentNode)
                End If
            Next
        End If
    End Sub
    Protected Sub btnSavePreferences_Click(sender As Object, e As EventArgs)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If logonUser Is Nothing Then
            Return
        End If
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@UserID", logonUser.ID))
        parameters.Add(New SqlParameter("@UserGlobalID", logonUser.GlobalID))
        parameters.Add(New SqlParameter("@DefaultEbusinessEnvironmentID", ddlEnvironment.SelectedValue))
        parameters.Add(New SqlParameter("@DefaultEbusinessSopID", ddlCountry.SelectedValue))
        parameters.Add(New SqlParameter("@DefaultEbusinessManagementType", ddlManagementType.SelectedValue))
        parameters.Add(New SqlParameter("@ExpandRowsOnSearchByDefault", chkBoxExpandOnSearch.Checked))
        parameters.Add(New SqlParameter("@ActivateWindowModeByDefault", chkBoxDisplayMode.Checked))
        parameters.Add(New SqlParameter("@DefaultSortingFieldAlias", ddlCustomersDefaultSortingBy.SelectedValue))
        parameters.Add(New SqlParameter("@IsAscendingSotring", ddlSortDirection.SelectedValue = 0))
        parameters.Add(New SqlParameter("@FieldIDs", String.Join(",", tvFields.CheckedNodes.ToList().Select(Function(fc) fc.Value))))
        If (ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_SavePreferences]", parameters)) Then
            logonUser.DefaultEbusinessEnvironmentID = ddlEnvironment.SelectedValue
            logonUser.DefaultEbusinessSopID = ddlCountry.SelectedValue
            logonUser.DefaultEbusinessManagementType = ddlManagementType.SelectedValue
            logonUser.ExpandRowsOnSearchByDefault = chkBoxExpandOnSearch.Checked
            logonUser.ActivateWindowModeByDefault = chkBoxDisplayMode.Checked
            logonUser.DefaultSortingFieldAlias = ddlCustomersDefaultSortingBy.SelectedValue
            logonUser.IsAscendingSotring = ddlSortDirection.SelectedValue = 0
            ClsSessionHelper.LogonUser = logonUser
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "window.parent.__doPostBack('imageBtnRefresh','RenderControls');window.parent.CloseChangePreferencesWindow();", True)
            ClsHelper.Log("Ebusiness Update Preferences", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
        Else
            lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
            ClsHelper.Log("Ebusiness Update Preferences", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "Failed to update preferences")
        End If
    End Sub

End Class
