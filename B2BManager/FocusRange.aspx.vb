Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class FocusRange
    Inherits System.Web.UI.Page

    Private clsUser As ClsUser
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        clsUser = ClsSessionHelper.LogonUser
        If Not IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID
            RenderControls()
            PopulateSearchGrid()
        End If
    End Sub

    Private Sub RenderControls()
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)
        hdSelectedSopid.Value = ddlCountry.SelectedValue.ToString()

        Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
        btnAddFocusRange.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_FOCUS_RANGE)
        If Not actions.Contains(ClsHelper.ActionDesignation.DELETE_FOCUS_RANGE) Then
            WindowManagement.Visible = False
        End If
        If Not actions.Contains(ClsHelper.ActionDesignation.ASSIGN_FOCUS_RANGE) Then
            WindowAssignment.Visible = False
        End If
    End Sub

    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
        End If
        If ddlCountry.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("SopID", ddlCountry.SelectedValue))
        End If
        Return parameters
    End Function

    Private Sub UpdateLabel()
        If searchDs.Rows.Count = 0 Then
            lblInformation.InnerHtml = "No results found, please check your filters"
            lblInformationContainer.Attributes.CssStyle.Add("left", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("right")
        Else
            lblInformation.InnerHtml = "Showing <span class='informationlabel-text'>" & searchDs.Rows.Count &
                                        "</span> row" & IIf(searchDs.Rows.Count > 1, "s", "") & " in <span class='information-label-text'>" + ddlEnvironment.SelectedItem.Text + "</span> environment"
            lblInformationContainer.Attributes.CssStyle.Add("right", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("left")
        End If
    End Sub


    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateSearchGrid()
    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        hdSelectedSopid.Value = ddlCountry.SelectedValue.ToString()
        PopulateSearchGrid()
    End Sub

    Public ReadOnly Property searchDs As DataTable
        Get
            Return ClsDataAccessHelper.FillDataTable("[FocusRange_GetAll]", GetSearchParameters(), CommandType.StoredProcedure)
        End Get

        'Get
        '    If Session("FocusRanges") Is Nothing Then
        '        Session("FocusRanges") = ClsDataAccessHelper.FillDataTable("[FocusRange_GetAll]", GetSearchParameters(), CommandType.StoredProcedure)
        '    End If
        '    Return Session("FocusRanges")
        'End Get
        'Set(value As DataTable)
        '    Session("FocusRanges") = value
        'End Set

    End Property

    Private Sub PopulateSearchGrid()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        '''Dim searchDs = Nothing
        If searchDs.Rows.Count > 0 Then
            gridSearch.DataSource = searchDs
            gridSearch.DataBind()
        End If

        If searchDs IsNot Nothing AndAlso ddlEnvironment.SelectedItem IsNot Nothing Then
            UpdateLabel()
            If searchDs.Rows.Count = 0 Then
                gridSearch.Visible = False
            Else
                gridSearch.Visible = True
            End If
        Else
            lblInformation.InnerHtml = ""
        End If
        ClsHelper.Log("Load Focus range records", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub gridSearch_PreRender(sender As Object, e As EventArgs)
        For Each dataItem As GridDataItem In gridSearch.MasterTableView.Items
            If Not ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_FOCUS_RANGE) Then
                dataItem.FindControl("tdDelete").Visible = False
            End If
            If Not ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_FOCUS_RANGE) Then
                dataItem.FindControl("tdAssign").Visible = False
            End If
        Next
    End Sub


    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
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
    Protected Sub gridSearch_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If searchDs.Rows.Count > 0 Then
            UpdateLabel()
            gridSearch.DataSource = searchDs
        End If
    End Sub

    Protected Sub handleRefreshEvent(sender As Object, e As EventArgs)
        PopulateSearchGrid()
    End Sub


End Class
