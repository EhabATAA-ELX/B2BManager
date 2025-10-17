
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class CustomersManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        If Not IsPostBack Or "RenderControls".Equals(__EVENTARGUMENT) Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            RenderControls()
            ClsHelper.Log("Access B2B Accounts", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Private Sub RenderControls()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, clsUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, clsUser.DefaultEbusinessSopID), False)
        chkBoxExpandOnSearch.Checked = clsUser.ExpandRowsOnSearchByDefault
        chkBoxDisplayMode.Checked = clsUser.ActivateWindowModeByDefault
        ddlManagementType.SelectedValue = clsUser.DefaultEbusinessManagementType

        Dim showNewCustomerButtonCondition As Boolean = False
        Boolean.TryParse(ConfigurationManager.AppSettings("DisplayNewCustomerButton"), showNewCustomerButtonCondition)
        If showNewCustomerButtonCondition Then
            newCustomerPnl.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_CUSTOMER)
        End If
        newSuperUserPnl.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_SUPER_USER)
        CreateCustomerScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_CUSTOMER)
        WindowNewCustomerAccount.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_CUSTOMER)
        CreateUserOrSuperUserScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_SUPER_USER) Or clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_USER)
        WindowNewUserAccount.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_SUPER_USER) Or clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_USER)
        DeleteCustomerScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER)
        DeleteUserScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SUPER_USER) Or clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_USER)
    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        ClsSessionHelper.EbusinessSopID = ddlCountry.SelectedValue
        LoadData()
    End Sub

    Protected Sub imageBtnRefresh_Click(sender As Object, e As ImageClickEventArgs)
        LoadData()
    End Sub

    Protected Function GetSortingColumn() As String
        Dim sortingColumn As String = "Customer Name"
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            sortingColumn = ClsSessionHelper.LogonUser.DefaultSortingFieldAlias
        End If
        Return sortingColumn
    End Function

    Protected Function IsAscendingSotring() As String
        Dim _isAscendingSotring As String = "true"
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            _isAscendingSotring = ClsSessionHelper.LogonUser.IsAscendingSotring.ToString().ToLower()
        End If
        Return _isAscendingSotring
    End Function

    Private Sub LoadData()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadData", "sotringColumn = '" + GetSortingColumn() + "';isAscendingSotring = " + IsAscendingSotring() + ";LoadData();", True)
    End Sub
    Protected Sub ddlManagementType_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadData()
    End Sub
    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        If Not String.IsNullOrEmpty(ddlEnvironment.SelectedValue) Then
            ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
        End If
        LoadData()
    End Sub
End Class
