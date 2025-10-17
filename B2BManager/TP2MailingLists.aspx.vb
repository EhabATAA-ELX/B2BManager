
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class TP2MailingLists
    Inherits System.Web.UI.Page

    Private clsUser As ClsUser
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        clsUser = ClsSessionHelper.LogonUser
        If Not IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID
            RenderControls()
            PopulateSearchGrid(IIf(ddlListType.SelectedValue = "0", gridSearchCustomer, gridSearchCountry))
        End If
    End Sub

    Private Sub RenderControls()
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.TP2EnvironmentID)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.TP2CountryCode, True, False, True)
        tdLevelDropDown.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.ACCESS_COUNTRY_LEVEL)
        tdLevelLabel.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.ACCESS_COUNTRY_LEVEL)
        ManageActionsColumnDisplay(gridSearchCountry, "1", clsUser)
        ManageActionsColumnDisplay(gridSearchCustomer, "0", clsUser)
        DeleteCountryEmailScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_COUNTRY_EMAIL_SETTINGS)
        DeleteCustomerEmailScript.Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER_EMAIL_SETTINGS)
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.TP2EnvironmentID = ddlEnvironment.SelectedValue
                ClsSessionHelper.TP2CountryCode = ddlCountry.SelectedValue
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

    Protected Sub handleRefreshEvent(o As Object, e As EventArgs)
        PopulateSearchGrid(IIf(ddlListType.SelectedValue = "0", gridSearchCustomer, gridSearchCountry))
    End Sub

    Protected Sub gridSearch_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        If searchDs.Rows.Count > 0 Then
            UpdateLabel()
            If ddlListType.SelectedValue = "1" Then
                gridSearchCountry.DataSource = searchDs
            Else
                gridSearchCustomer.DataSource = searchDs
            End If
        End If
    End Sub

    Private Sub UpdateLabel()
        If searchDs.Rows.Count = 0 Then
            lblInformation.InnerHtml = "No results found, please check your filters"
            lblInformationContainer.Attributes.CssStyle.Add("left", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("right")
        Else
            lblInformation.InnerHtml = "Showing <span class='informationlabel-text'>" & searchDs.Rows.Count &
                                        "</span> row" & IIf(searchDs.Rows.Count > 1, "s", "") & " in <span class='information-label-text'>" + ddlEnvironment.SelectedItem.Text + "</span> environment" &
                                        " at <span class='information-label-text'>" + ddlListType.SelectedItem.Text + " level</span>"
            lblInformationContainer.Attributes.CssStyle.Add("right", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("left")
        End If
    End Sub

    Public Property searchDs As DataTable
        Get

            If Session("mailingList") Is Nothing Then
                Session("mailingList") = ClsDataAccessHelper.FillDataTable("TP2MailingList.GetMailingList", GetSearchParameters(), CommandType.StoredProcedure)
            End If
            Return Session("mailingList")
        End Get
        Set(value As DataTable)
            Session("mailingList") = value
        End Set
    End Property


    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
        End If

        If ddlCountry.SelectedValue <> "All" Then
            parameters.Add(New SqlParameter("CountryIsoCodes", ddlCountry.SelectedValue))
        End If
        If Not String.IsNullOrWhiteSpace(txtBoxSearchInDetails.Text) Then
            parameters.Add(New SqlParameter("SearchText", txtBoxSearchInDetails.Text.Trim()))
        End If
        If ddlListType.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("ListType", CInt(ddlListType.SelectedValue)))
        End If
        Return parameters
    End Function

    Private Sub PopulateSearchGrid(gridSearch As RadGrid)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        searchDs = Nothing
        If ddlListType.SelectedValue = "0" Then
            gridSearchCountry.Visible = False
        Else
            gridSearchCustomer.Visible = False
        End If
        If searchDs.Rows.Count > 0 Then
            gridSearch.DataSource = searchDs
            gridSearch.DataBind()
        End If
        newCustomerEmailSetting.Visible = ddlListType.SelectedValue = "0" And clsUser.Actions.Contains(ClsHelper.ActionDesignation.ADD_EDIT_CUSTOMER_EMAIL_SETTINGS)
        newCountryEmailSetting.Visible = ddlListType.SelectedValue = "1" And clsUser.Actions.Contains(ClsHelper.ActionDesignation.ADD_EDIT_COUNTRY_EMAIL_SETTINGS)

        If searchDs IsNot Nothing AndAlso ddlEnvironment.SelectedItem IsNot Nothing Then
            UpdateLabel()
            If searchDs.Rows.Count = 0 Then
                gridSearch.Visible = False
            Else
                gridSearch.MasterTableView.GetColumn("ID").Visible = False
                If gridSearch.Equals(gridSearchCountry) Then
                    gridSearch.MasterTableView.GetColumn("Country").Visible = False
                    gridSearch.MasterTableView.GetColumn("CY_NAME_ISOCODE").Visible = False
                End If
                gridSearch.Visible = True
            End If
        Else
            lblInformation.InnerHtml = ""
        End If
        ClsHelper.Log("Load " + IIf(ddlListType.SelectedValue = "0", "Customer", "Country") + " TP2 Mailing", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub
    Protected Sub searchGridColumn_created(ByVal sender As Object, ByVal e As GridColumnCreatedEventArgs)
        e.Column.HeaderText = Replace(e.Column.HeaderText, "_", " ")
    End Sub

    Protected Sub ManageActionsColumnDisplay(gridSearch As RadGrid, display As String, clsUser As ClsUser)
        If Not clsUser.Actions.Contains(IIf(display = "0", ClsHelper.ActionDesignation.ADD_EDIT_CUSTOMER_EMAIL_SETTINGS, ClsHelper.ActionDesignation.ADD_EDIT_COUNTRY_EMAIL_SETTINGS)) Then
            If Not clsUser.Actions.Contains(IIf(display = "0", ClsHelper.ActionDesignation.DELETE_CUSTOMER_EMAIL_SETTINGS, ClsHelper.ActionDesignation.DELETE_COUNTRY_EMAIL_SETTINGS)) Then
                gridSearch.MasterTableView.GetColumn("Actions").Visible = False
                gridSearch.MasterTableView.GetColumn("Actions").Display = False
            Else
                gridSearch.MasterTableView.GetColumn("Actions").HeaderText = "Delete"
            End If
        Else
            If Not clsUser.Actions.Contains(IIf(display = "0", ClsHelper.ActionDesignation.DELETE_CUSTOMER_EMAIL_SETTINGS, ClsHelper.ActionDesignation.DELETE_COUNTRY_EMAIL_SETTINGS)) Then
                gridSearch.MasterTableView.GetColumn("Actions").HeaderText = "Edit"
            End If
        End If
    End Sub



    Protected Sub gridSearchCountry_ItemDataBound(sender As Object, e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            CType(e.Item, GridDataItem)("Actions").FindControl("tdEdit").Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.ADD_EDIT_COUNTRY_EMAIL_SETTINGS)
            CType(e.Item, GridDataItem)("Actions").FindControl("tdDelete").Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_COUNTRY_EMAIL_SETTINGS)
        End If
    End Sub

    Protected Sub gridSearchCustomer_ItemDataBound(sender As Object, e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            CType(e.Item, GridDataItem)("Actions").FindControl("tdEdit").Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.ADD_EDIT_CUSTOMER_EMAIL_SETTINGS)
            CType(e.Item, GridDataItem)("Actions").FindControl("tdDelete").Visible = clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER_EMAIL_SETTINGS)
        End If
    End Sub
End Class
